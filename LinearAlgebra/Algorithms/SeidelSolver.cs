using System;
using System.Text;

namespace MatrixCalculator
{
    public class SeidelSolver
    {
        private readonly SLAE system;
        private readonly double tolerance;
        private readonly int maxIterations;
        private readonly StringBuilder log = new StringBuilder();

        public int Iterations { get; private set; }
        public bool Converged { get; private set; }
        public string Log => log.ToString();

        public SeidelSolver(SLAE slae, double tolerance, int maxIterations)
        {
            system = slae ?? throw new ArgumentNullException(nameof(slae));
            if (tolerance <= 0)
                throw new ArgumentException("Tolerance must be positive.");
            if (maxIterations <= 0)
                throw new ArgumentException("Max iterations must be positive.");
            this.tolerance = tolerance;
            this.maxIterations = maxIterations;
        }

        public static bool CheckDiagonalDominance(Matrix A)
        {
            for (int i = 0; i < A.Rows; i++)
            {
                double diag = Math.Abs(A[i, i]);
                double offSum = 0;
                for (int j = 0; j < A.Columns; j++)
                    if (j != i) offSum += Math.Abs(A[i, j]);
                if (diag <= offSum)
                    return false;
            }
            return true;
        }

        public void Solve()
        {
            log.Clear();
            int n = system.A.Rows;

            bool dominated = CheckDiagonalDominance(system.A);
            log.AppendLine("=== GAUSS-SEIDEL ITERATIVE METHOD ===");
            log.AppendLine($"Sufficient convergence condition (strict diagonal dominance): " +
                           $"{(dominated ? "SATISFIED — convergence guaranteed" : "NOT SATISFIED — may diverge")}");
            log.AppendLine($"Tolerance ε = {tolerance:E2},  Max iterations = {maxIterations}");
            log.AppendLine();

            for (int i = 0; i < n; i++)
                if (Math.Abs(system.A[i, i]) < 1e-15)
                    throw new Exception($"Zero diagonal element at row {i + 1}. " +
                                        "Rearrange equations so all diagonal elements are non-zero.");

            Vector x = new Vector(n);
            Converged = false;
            for (Iterations = 1; Iterations <= maxIterations; Iterations++)
            {
                Vector xPrev = new Vector(x);

                for (int i = 0; i < n; i++)
                {
                    double s = system.B[i];
                    for (int j = 0; j < n; j++)
                        if (j != i)
                            s -= system.A[i, j] * x[j]; 
                    x[i] = s / system.A[i, i];
                }

                double maxDiff = 0;
                for (int i = 0; i < n; i++)
                    maxDiff = Math.Max(maxDiff, Math.Abs(x[i] - xPrev[i]));

                if (Iterations <= 5 || Iterations % 50 == 0)
                    log.AppendLine($"  iter {Iterations,5}:  max|Δx| = {maxDiff:E4}");

                if (maxDiff < tolerance)
                {
                    Converged = true;
                    if (Iterations > 5 && Iterations % 50 != 0)
                        log.AppendLine($"  iter {Iterations,5}:  max|Δx| = {maxDiff:E4}");
                    log.AppendLine($"\nCONVERGED after {Iterations} iteration(s),  max|Δx| = {maxDiff:E4}");
                    break;
                }
            }

            if (!Converged)
                log.AppendLine($"\nNOT CONVERGED after {maxIterations} iterations.");

            system.X = x;

            log.AppendLine("\nSolution vector x:");
            for (int i = 0; i < n; i++)
                log.AppendLine($"  x{i + 1} = {x[i]:F8}");

            Vector residual = system.CalculateResidual();
            double maxR = 0;
            for (int i = 0; i < residual.Size; i++)
                maxR = Math.Max(maxR, Math.Abs(residual[i]));
            log.AppendLine($"\nResidual  ||Ax - b||∞ = {maxR:E6}");
        }
    }
}
