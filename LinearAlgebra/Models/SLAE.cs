using System;
using System.IO;

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
                throw new ArgumentException("Matrix A must be square to solve SLAE");
            if (a.Rows != b.Size)
                throw new ArgumentException($"Matrix A size ({a.Rows}x{a.Columns}) does not match vector b size ({b.Size})");

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

        public Vector CalculateResidual()
        {
            if (X == null || X.Size == 0)
                throw new InvalidOperationException("The system solution must be computed first");

            Vector residual = new Vector(A.Rows);
            for (int i = 0; i < A.Rows; i++)
            {
                double sum = 0;
                for (int j = 0; j < A.Columns; j++)
                    sum += A[i, j] * X[j];
                residual[i] = B[i] - sum;
            }
            return residual;
        }

        public double CheckAccuracy()
        {
            Vector residual = CalculateResidual();
            double maxResidual = 0;
            for (int i = 0; i < residual.Size; i++)
                maxResidual = Math.Max(maxResidual, Math.Abs(residual[i]));
            return maxResidual;
        }

        public void SaveToFile(string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                writer.WriteLine("# Matrix A:");
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

                writer.WriteLine("# Vector b:");
                for (int i = 0; i < B.Size; i++)
                    writer.WriteLine(B[i].ToString("F4"));

                if (X != null && X.Size > 0)
                {
                    writer.WriteLine("# Solution x:");
                    for (int i = 0; i < X.Size; i++)
                        writer.WriteLine(X[i].ToString("F4"));

                    Vector residual = CalculateResidual();
                    writer.WriteLine("# Residual:");
                    for (int i = 0; i < residual.Size; i++)
                        writer.WriteLine(residual[i].ToString("E4"));
                }
            }
        }

        public override string ToString()
        {
            string result = "Linear system Ax = b\n\n";
            result += "Matrix A:\n" + A.ToString() + "\n";
            result += "Vector b:\n" + B.ToString();
            if (X != null && X.Size > 0)
            {
                result += "\nSolution x:\n" + X.ToString();

                Vector residual = CalculateResidual();
                result += "Residual:\n" + residual.ToString();
            }
            return result;
        }
    }
}