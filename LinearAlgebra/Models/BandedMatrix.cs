using System;
using System.Text;

namespace MatrixCalculator
{
    public class BandedMatrix
    {
        private readonly double[] band;

        public int N { get; }
        public int P { get; }
        public int Q { get; }
        public int DiagCount => P + Q + 1;
        public int BandVectorLength => N * DiagCount;

        public BandedMatrix(int n, int p, int q)
        {
            if (n <= 0) throw new ArgumentException("Matrix size must be positive.");
            if (p < 0 || q < 0) throw new ArgumentException("Bandwidths must be non-negative.");
            N = n; P = p; Q = q;
            band = new double[n * (p + q + 1)];
        }

        public double this[int i, int j]
        {
            get
            {
                if (i < 0 || i >= N || j < 0 || j >= N)
                    throw new IndexOutOfRangeException($"Index ({i},{j}) out of matrix bounds.");
                int k = j - i + P;
                if (k < 0 || k >= DiagCount) return 0.0;
                return band[i * DiagCount + k];
            }
            set
            {
                if (i < 0 || i >= N || j < 0 || j >= N)
                    throw new IndexOutOfRangeException($"Index ({i},{j}) out of matrix bounds.");
                int k = j - i + P;
                if (k < 0 || k >= DiagCount)
                    throw new ArgumentException(
                        $"Element ({i},{j}) is outside the band (P={P}, Q={Q}). " +
                        "Only elements with |j-i| <= bandwidth can be stored.");
                band[i * DiagCount + k] = value;
            }
        }

        public double[] GetBandVector() => (double[])band.Clone();

        public static BandedMatrix FromMatrix(Matrix A, int p, int q)
        {
            if (!A.IsSquare()) throw new ArgumentException("Matrix must be square.");
            var bm = new BandedMatrix(A.Rows, p, q);
            for (int i = 0; i < A.Rows; i++)
                for (int j = Math.Max(0, i - p); j <= Math.Min(A.Rows - 1, i + q); j++)
                    bm[i, j] = A[i, j];
            return bm;
        }

        public Matrix ToFullMatrix()
        {
            var A = new Matrix(N, N);
            for (int i = 0; i < N; i++)
                for (int j = Math.Max(0, i - P); j <= Math.Min(N - 1, i + Q); j++)
                    A[i, j] = this[i, j];
            return A;
        }

        public string FormatBandVector()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Compact band vector  (N={N}, P={P}, Q={Q})");
            sb.AppendLine($"Vector length = N×(P+Q+1) = {N}×{DiagCount} = {BandVectorLength}");
            sb.AppendLine($"(Full matrix would need {N * N} elements; saves {N * N - BandVectorLength} zeros)");
            sb.AppendLine($"Mapping:  band[ i*(P+Q+1) + (j-i+P) ] = A[i,j]");
            sb.AppendLine();

            sb.Append("     | ");
            for (int d = -P; d <= Q; d++)
                sb.Append($" d={d,2}  ");
            sb.AppendLine($"  | row sum");
            sb.AppendLine(new string('-', 9 + 7 * DiagCount + 10));

            for (int i = 0; i < N; i++)
            {
                sb.Append($" r{i + 1,2} | ");
                for (int d = -P; d <= Q; d++)
                {
                    int j = i + d;
                    if (j < 0 || j >= N)
                        sb.Append("  --- ");
                    else
                        sb.Append($"{this[i, j],6:F2}");
                }

                int idxStart = i * DiagCount;
                int idxEnd = idxStart + DiagCount - 1;
                sb.AppendLine($"  | band[{idxStart}..{idxEnd}]");
            }

            sb.AppendLine();
            sb.AppendLine("Raw band vector (row-major):");
            sb.Append("  [ ");
            for (int k = 0; k < BandVectorLength; k++)
            {
                sb.Append($"{band[k]:F2}");
                if (k < BandVectorLength - 1) sb.Append(", ");
                if ((k + 1) % DiagCount == 0 && k < BandVectorLength - 1)
                    sb.Append("| ");
            }
            sb.AppendLine(" ]");

            return sb.ToString();
        }
    }
}
