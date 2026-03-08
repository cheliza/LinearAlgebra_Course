using System;

namespace MatrixCalculator
{
    public class SLAE
    {
        public Matrix A { get; private set; }
        public Vector X { get; set; }
        public Vector B { get; private set; }

        public SLAE(Matrix a, Vector b)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            if (!a.IsSquare())
                throw new ArgumentException("Матриця A має бути квадратною для розв'язання СЛАР");
            if (a.Rows != b.Size)
                throw new ArgumentException($"Розміри матриці A ({a.Rows}x{a.Columns}) та вектора b ({b.Size}) не збігаються");

            A = new Matrix(a);
            B = new Vector(b);
            X = new Vector(a.Rows);
        }

        public SLAE(SLAE other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            A = new Matrix(other.A);
            B = new Vector(other.B);
            X = other.X != null ? new Vector(other.X) : new Vector(A.Rows);
        }

        public void SolveGaussian()
        {
            int n = A.Rows;

            Matrix a = new Matrix(A);
            Vector b = new Vector(B);
            X = new Vector(n);

            const double epsilon = 1e-10;

            for (int i = 0; i < n; i++)
            {
                int maxRow = i;
                double maxVal = Math.Abs(a[i, i]);
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(a[k, i]) > maxVal)
                    {
                        maxVal = Math.Abs(a[k, i]);
                        maxRow = k;
                    }
                }

                if (maxVal < epsilon)
                    throw new InvalidOperationException("Матриця системи вироджена (визначник дорівнює нулю)");

                if (maxRow != i)
                {
                    for (int j = i; j < n; j++)
                    {
                        double temp = a[i, j];
                        a[i, j] = a[maxRow, j];
                        a[maxRow, j] = temp;
                    }
                    double tempB = b[i];
                    b[i] = b[maxRow];
                    b[maxRow] = tempB;
                }

                double pivot = a[i, i];
                for (int j = i; j < n; j++)
                {
                    a[i, j] /= pivot;
                }
                b[i] /= pivot;

                for (int k = i + 1; k < n; k++)
                {
                    double factor = a[k, i];
                    if (Math.Abs(factor) > epsilon)
                    {
                        for (int j = i; j < n; j++)
                        {
                            a[k, j] -= factor * a[i, j];
                        }
                        b[k] -= factor * b[i];
                    }
                }
            }

            for (int i = n - 1; i >= 0; i--)
            {
                X[i] = b[i];
                for (int j = i + 1; j < n; j++)
                {
                    X[i] -= a[i, j] * X[j];
                }
            }
        }

        public void SolveGaussJordan()
        {
            int n = A.Rows;

            Matrix augmented = new Matrix(n, n + 1);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    augmented[i, j] = A[i, j];
                }
                augmented[i, n] = B[i];
            }

            const double epsilon = 1e-10;

            for (int i = 0; i < n; i++)
            {
                int maxRow = i;
                double maxVal = Math.Abs(augmented[i, i]);
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(augmented[k, i]) > maxVal)
                    {
                        maxVal = Math.Abs(augmented[k, i]);
                        maxRow = k;
                    }
                }

                if (maxVal < epsilon)
                    throw new InvalidOperationException("Матриця системи вироджена");

                if (maxRow != i)
                {
                    for (int j = i; j <= n; j++)
                    {
                        double temp = augmented[i, j];
                        augmented[i, j] = augmented[maxRow, j];
                        augmented[maxRow, j] = temp;
                    }
                }

                double pivot = augmented[i, i];
                for (int j = i; j <= n; j++)
                {
                    augmented[i, j] /= pivot;
                }

                for (int k = 0; k < n; k++)
                {
                    if (k != i && Math.Abs(augmented[k, i]) > epsilon)
                    {
                        double factor = augmented[k, i];
                        for (int j = i; j <= n; j++)
                        {
                            augmented[k, j] -= factor * augmented[i, j];
                        }
                    }
                }
            }

            X = new Vector(n);
            for (int i = 0; i < n; i++)
            {
                X[i] = augmented[i, n];
            }
        }

        public Vector CalculateResidual()
        {
            if (X == null || X.Size == 0)
                throw new InvalidOperationException("Спочатку потрібно знайти розв'язок системи");

            Vector residual = new Vector(A.Rows);
            for (int i = 0; i < A.Rows; i++)
            {
                double sum = 0;
                for (int j = 0; j < A.Columns; j++)
                {
                    sum += A[i, j] * X[j];
                }
                residual[i] = B[i] - sum;
            }
            return residual;
        }

        public double CheckAccuracy()
        {
            Vector residual = CalculateResidual();
            double maxResidual = 0;
            for (int i = 0; i < residual.Size; i++)
            {
                maxResidual = Math.Max(maxResidual, Math.Abs(residual[i]));
            }
            return maxResidual;
        }

        public void SaveToFile(string filename)
        {
            using (var writer = new System.IO.StreamWriter(filename))
            {
                writer.WriteLine("# Матриця A:");
                for (int i = 0; i < A.Rows; i++)
                {
                    for (int j = 0; j < A.Columns; j++)
                    {
                        writer.Write(A[i, j].ToString("F4"));
                        if (j < A.Columns - 1)
                            writer.Write(" ");
                    }
                    writer.WriteLine();
                }

                writer.WriteLine("# Вектор b:");
                for (int i = 0; i < B.Size; i++)
                {
                    writer.WriteLine(B[i].ToString("F4"));
                }

                if (X != null && X.Size > 0)
                {
                    writer.WriteLine("# Розв'язок x:");
                    for (int i = 0; i < X.Size; i++)
                    {
                        writer.WriteLine(X[i].ToString("F4"));
                    }

                    Vector residual = CalculateResidual();
                    writer.WriteLine("# Нев'язка:");
                    for (int i = 0; i < residual.Size; i++)
                    {
                        writer.WriteLine(residual[i].ToString("E4"));
                    }
                }
            }
        }

        public override string ToString()
        {
            string result = "Система лінійних рівнянь Ax = b\n\n";
            result += "Матриця A:\n" + A.ToString() + "\n";
            result += "Вектор b:\n" + B.ToString();
            if (X != null && X.Size > 0)
            {
                result += "\nРозв'язок x:\n" + X.ToString();

                Vector residual = CalculateResidual();
                result += "Нев'язка:\n" + residual.ToString();
            }
            return result;
        }
    }
}