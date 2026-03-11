using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace MatrixCalculator
{
    public class Vector
    {
        private double[] data;

        public int Size => data.Length;

        public Vector(int size)
        {
            if (size <= 0)
                throw new ArgumentException("Vector size must be positive");

            data = new double[size];
        }

        public Vector(Vector other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            data = new double[other.Size];
            Array.Copy(other.data, data, other.Size);
        }

        public double this[int i]
        {
            get
            {
                if (i < 0 || i >= Size)
                    throw new IndexOutOfRangeException("Index out of range");
                return data[i];
            }
            set
            {
                if (i < 0 || i >= Size)
                    throw new IndexOutOfRangeException("Index out of range");
                data[i] = value;
            }
        }

        public void RandomInitialize(int minValue, int maxValue)
        {
            Random rnd = new Random();
            for (int i = 0; i < Size; i++)
                data[i] = rnd.Next(minValue, maxValue + 1);
        }

        public static Vector ReadFromFile(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException("File not found", filename);

            string[] lines = File.ReadAllLines(filename);
            var nonEmpty = new List<double>();
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (!double.TryParse(line.Trim(), out double val))
                        throw new Exception($"Cannot parse number: '{line}'");
                    nonEmpty.Add(val);
                }
            }

            Vector v = new Vector(nonEmpty.Count);
            for (int i = 0; i < nonEmpty.Count; i++)
                v[i] = nonEmpty[i];

            return v;
        }

        public void WriteToFile(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                for (int i = 0; i < Size; i++)
                    writer.WriteLine(data[i].ToString("F2"));
            }
        }

        public static Vector operator +(Vector a, Vector b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Vectors cannot be null");

            if (a.Size != b.Size)
                throw new InvalidOperationException("Vectors have different lengths");

            Vector result = new Vector(a.Size);
            for (int i = 0; i < a.Size; i++)
                result[i] = a[i] + b[i];
            return result;
        }

        public static Vector operator -(Vector a, Vector b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Vectors cannot be null");

            if (a.Size != b.Size)
                throw new InvalidOperationException("Vectors have different lengths");

            Vector result = new Vector(a.Size);
            for (int i = 0; i < a.Size; i++)
                result[i] = a[i] - b[i];
            return result;
        }

        public static Vector operator *(Vector v, double k)
        {
            if (v == null)
                throw new ArgumentNullException(nameof(v));

            Vector result = new Vector(v.Size);
            for (int i = 0; i < v.Size; i++)
                result[i] = v[i] * k;
            return result;
        }

        public static Vector operator *(double k, Vector v) => v * k;

        public double Dot(Vector other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (Size != other.Size)
                throw new InvalidOperationException("Vectors have different lengths");

            double sum = 0;
            for (int i = 0; i < Size; i++)
                sum += this[i] * other[i];
            return sum;
        }

        public double Norm() => Math.Sqrt(this.Dot(this));

        public double MaxNorm()
        {
            if (Size == 0)
                throw new InvalidOperationException("Cannot compute norm of empty vector");

            double max = Math.Abs(data[0]);
            for (int i = 1; i < Size; i++)
                max = Math.Max(max, Math.Abs(data[i]));
            return max;
        }

        public double FrobeniusNorm() => Norm();

        public Vector Clone() => new Vector(this);

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Size; i++)
                sb.AppendLine(data[i].ToString("F2"));
            return sb.ToString();
        }
    }
}