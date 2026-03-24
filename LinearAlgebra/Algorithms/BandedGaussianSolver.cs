using System;
using System.Text;

namespace MatrixCalculator
{
    public class BandedGaussianSolver
    {
        private readonly BandedMatrix original;
        private readonly Vector rhs;
        private readonly StringBuilder log = new StringBuilder();

        public Vector Solution { get; private set; }
        public string Log => log.ToString();

        public BandedGaussianSolver(BandedMatrix a, Vector b)
        {
            original = a ?? throw new ArgumentNullException(nameof(a));
            rhs = b ?? throw new ArgumentNullException(nameof(b));
            if (a.N != b.Size)
                throw new ArgumentException("Matrix size and vector B size must match.");
        }

        public void Solve()
        {
            log.Clear();
            int n = original.N;
            int p = original.P;
            int q = original.Q;

            log.AppendLine("=== GAUSSIAN ELIMINATION FOR BAND (RIBBON) MATRIX ===");
            log.AppendLine($"Matrix size: {n}×{n}");
            log.AppendLine($"Lower bandwidth P = {p}  (sub-diagonals)");
            log.AppendLine($"Upper bandwidth Q = {q}  (super-diagonals)");
            log.AppendLine($"Band vector length: N×(P+Q+1) = {n}×{p + q + 1} = {n * (p + q + 1)}");
            log.AppendLine($"Full matrix size:   N² = {n * n}   (saved {n * n - n * (p + q + 1)} zero-element slots)");
            log.AppendLine();

            var work = new BandedMatrix(n, p, q);
            var b = new Vector(rhs);

            for (int i = 0; i < n; i++)
                for (int j = Math.Max(0, i - p); j <= Math.Min(n - 1, i + q); j++)
                    work[i, j] = original[i, j];

            log.AppendLine("--- Forward Elimination ---");
            for (int k = 0; k < n - 1; k++)
            {
                double pivot = work[k, k];
                if (Math.Abs(pivot) < 1e-12)
                    throw new Exception(
                        $"Zero (or near-zero) pivot at step {k + 1}. " +
                        "The matrix may be singular or require row permutation.");

                int rowEnd = Math.Min(n - 1, k + p);
                int colEnd = Math.Min(n - 1, k + q);

                for (int i = k + 1; i <= rowEnd; i++)
                {
                    double m = work[i, k] / pivot;   
                    if (Math.Abs(m) < 1e-15) continue;
                    work[i, k] = 0.0;

                    for (int j = k + 1; j <= colEnd; j++)
                        work[i, j] -= m * work[k, j];

                    b[i] -= m * b[k];
                }

                if (k < 3 || k >= n - 3)
                    log.AppendLine($"  Step {k + 1,3}: pivot = {pivot,10:F4},  " +
                                   $"eliminate rows {k + 2}..{rowEnd + 1},  " +
                                   $"columns {k + 1}..{colEnd + 1}");
                else if (k == 3 && n > 7)
                    log.AppendLine("  ...");
            }
            log.AppendLine();

            log.AppendLine("--- Back Substitution ---");
            Solution = new Vector(n);

            for (int i = n - 1; i >= 0; i--)
            {
                double s = b[i];
                for (int j = i + 1; j <= Math.Min(n - 1, i + q); j++)
                    s -= work[i, j] * Solution[j];

                double diag = work[i, i];
                if (Math.Abs(diag) < 1e-12)
                    throw new Exception($"Zero diagonal at row {i + 1} during back substitution.");

                Solution[i] = s / diag;
            }

            log.AppendLine("Solution vector x:");
            for (int i = 0; i < n; i++)
                log.AppendLine($"  x{i + 1} = {Solution[i]:F8}");
            double maxR = 0;
            for (int i = 0; i < n; i++)
            {
                double r = rhs[i];
                for (int j = Math.Max(0, i - p); j <= Math.Min(n - 1, i + q); j++)
                    r -= original[i, j] * Solution[j];
                maxR = Math.Max(maxR, Math.Abs(r));
            }
            log.AppendLine($"\nResidual  ||Ax - b||∞ = {maxR:E6}");
        }
    }
}
