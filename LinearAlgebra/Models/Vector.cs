using System;

namespace MatrixCalculator
{
    public class Vector
    {
        private Matrix matrix;

        public int Size { get { return matrix.Rows; } }

        public Vector(int size)
        {
            if (size <= 0)
                throw new ArgumentException("Розмір вектора має бути додатним числом");

            matrix = new Matrix(size, 1);
        }

        public Vector(Matrix m)
        {
            if (m == null)
                throw new ArgumentNullException(nameof(m));

            if (m.Columns != 1)
                throw new ArgumentException("Матриця має бути стовпцем (розмір n×1)");

            matrix = new Matrix(m);
        }

        public Vector(Vector other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            matrix = new Matrix(other.matrix);
        }

        public double this[int i]
        {
            get
            {
                if (i < 0 || i >= Size)
                    throw new IndexOutOfRangeException("Індекс виходить за межі вектора");
                return matrix[i, 0];
            }
            set
            {
                if (i < 0 || i >= Size)
                    throw new IndexOutOfRangeException("Індекс виходить за межі вектора");
                matrix[i, 0] = value;
            }
        }

        public void RandomInitialize(int minValue, int maxValue)
        {
            matrix.RandomInitialize(minValue, maxValue);
        }

        public static Vector ReadFromFile(string filename)
        {
            Matrix m = Matrix.ReadFromFile(filename);
            if (m.Columns != 1)
                throw new Exception("Файл має містити вектор-стовпець (один стовпець)");
            return new Vector(m);
        }

        public void WriteToFile(string filename)
        {
            matrix.WriteToFile(filename);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Вектори не можуть бути null");

            if (a.Size != b.Size)
                throw new InvalidOperationException($"Неможливо додати вектори: різна довжина ({a.Size} та {b.Size})");

            Vector result = new Vector(a.Size);
            for (int i = 0; i < a.Size; i++)
                result[i] = a[i] + b[i];
            return result;
        }

        public static Vector operator -(Vector a, Vector b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Вектори не можуть бути null");

            if (a.Size != b.Size)
                throw new InvalidOperationException($"Неможливо відняти вектори: різна довжина ({a.Size} та {b.Size})");

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

        public static Vector operator *(double k, Vector v)
        {
            return v * k;
        }

        public double Dot(Vector other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (Size != other.Size)
                throw new InvalidOperationException($"Неможливо обчислити скалярний добуток: різна довжина ({Size} та {other.Size})");

            double sum = 0;
            for (int i = 0; i < Size; i++)
                sum += this[i] * other[i];
            return sum;
        }

        public double Norm()
        {
            return Math.Sqrt(Dot(this));
        }

        public Matrix ToMatrix()
        {
            return new Matrix(matrix);
        }

        public Vector Clone()
        {
            return new Vector(this);
        }

        public override string ToString()
        {
            return matrix.ToString();
        }
    }
}