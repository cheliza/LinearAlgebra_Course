using System;
using System.Text;

namespace MatrixCalculator
{
    public class GaussianSolver
    {
        private readonly SLAE system;    
        private Matrix lu;              
        private int[] perm;              
        private bool singular;           
        private bool inconsistent;       
        private StringBuilder stepLog;   

        public enum SolutionType
        {
            Unique,
            None,
            Infinite,
            Unknown
        }

        public SolutionType Status { get; private set; }
        public string StepLog => stepLog?.ToString();
        public bool IsSingular => singular;
        public bool IsInconsistent => inconsistent;
        public Matrix LU => lu?.Clone();
        public int[] Permutation => perm?.Clone() as int[];
        public Vector Solution => system.X;

        public GaussianSolver(SLAE slae)
        {
            system = slae ?? throw new ArgumentNullException(nameof(slae));
            if (!system.A.IsSquare())
                throw new ArgumentException("Matrix A must be square.");
            if (system.A.Rows != system.B.Size)
                throw new ArgumentException("Matrix A and vector B sizes do not match.");

            singular = false;
            inconsistent = false;
            Status = SolutionType.Unknown;
            stepLog = new StringBuilder();
        }

        private void Log(string message) => stepLog.AppendLine(message);

        public void Decompose()
        {
            stepLog.Clear();
            Log("=== LU DECOMPOSITION USING GAUSSIAN ELIMINATION ===");

            int n = system.A.Rows;
            lu = new Matrix(system.A);   
            perm = new int[n];
            for (int i = 0; i < n; i++) perm[i] = i;

            const double epsilon = 1e-12;

            for (int k = 0; k < n; k++)
            {
                int maxRow = k;
                double maxVal = Math.Abs(lu[k, k]);
                for (int i = k + 1; i < n; i++)
                    if (Math.Abs(lu[i, k]) > maxVal) { 
                        maxVal = Math.Abs(lu[i, k]); 
                        maxRow = i; 
                    }

                if (maxVal < epsilon)
                    singular = true;

                if (maxRow != k)
                {
                    for (int j = 0; j < n; j++)
                    {
                        double tmp = lu[k, j];
                        lu[k, j] = lu[maxRow, j];
                        lu[maxRow, j] = tmp;
                    }
                    int t = perm[k]; 
                    perm[k] = perm[maxRow]; 
                    perm[maxRow] = t;
                }

                if (singular && Math.Abs(lu[k, k]) < epsilon) continue;

                for (int i = k + 1; i < n; i++)
                {
                    double factor = lu[i, k] / lu[k, k];
                    lu[i, k] = factor;  
                    for (int j = k + 1; j < n; j++)
                        lu[i, j] -= factor * lu[k, j]; 
                }
            }
        }

        public void Solve()
        {
            if (lu == null) Decompose();

            int n = system.A.Rows;
            Vector bPerm = new Vector(n);
            for (int i = 0; i < n; i++)
                bPerm[i] = system.B[perm[i]];

            Vector y = new Vector(n);
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < i; j++)
                    sum += lu[i, j] * y[j];
                y[i] = bPerm[i] - sum;
            }

            system.X = new Vector(n);
            for (int i = n - 1; i >= 0; i--)
            {
                double sum = 0;
                for (int j = i + 1; j < n; j++)
                    sum += lu[i, j] * system.X[j];
                system.X[i] = y[i] / lu[i, i];
            }

            Status = singular ? SolutionType.Infinite : SolutionType.Unique;
        }

        public double Determinant()
        {
            if (lu == null) Decompose();
            if (singular) return 0.0;

            double det = 1.0;
            for (int i = 0; i < lu.Rows; i++)
                det *= lu[i, i];

            bool[] visited = new bool[lu.Rows];
            int sign = 1;
            for (int i = 0; i < lu.Rows; i++)
            {
                if (!visited[i])
                {
                    int cycleLen = 0;
                    int j = i;
                    while (!visited[j])
                    {
                        visited[j] = true;
                        j = perm[j];
                        cycleLen++;
                    }
                    if (cycleLen % 2 == 0) sign *= -1;
                }
            }
            return sign * det;
        }

        public Matrix Inverse()
        {
            if (lu == null) Decompose();
            int n = lu.Rows;
            Matrix inv = new Matrix(n, n);

            for (int i = 0; i < n; i++)
            {
                Vector e = new Vector(n);
                e[i] = 1.0;
                SLAE colSystem = new SLAE(system.A.Clone(), e);
                GaussianSolver colSolver = new GaussianSolver(colSystem);
                colSolver.Decompose();
                colSolver.Solve();
                for (int j = 0; j < n; j++)
                    inv[j, i] = colSystem.X[j];
            }
            return inv;
        }

        public Vector Residual()
        {
            if (system.X == null) Solve(); 
            Vector r = new Vector(system.B.Size);
            for (int i = 0; i < system.A.Rows; i++)
            {
                double sum = 0;
                for (int j = 0; j < system.A.Cols; j++)
                    sum += system.A[i, j] * system.X[j];
                r[i] = system.B[i] - sum;
            }
            return r;
        }
    }
}