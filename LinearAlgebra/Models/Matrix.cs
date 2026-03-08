using System;
using System.IO;
using System.Text;

namespace MatrixCalculator
{
    public class Matrix
    {
        private double[,] data;
        public int Rows { get; private set; }
        public int Columns { get; private set; }

        // Конструктор без параметрів
        public Matrix()
        {
            Rows = 0;
            Columns = 0;
            data = new double[0, 0];
        }

        public Matrix(int rows, int columns)
        {
            if (rows <= 0 || columns <= 0)
                throw new ArgumentException("Розміри матриці мають бути додатними числами");

            Rows = rows;
            Columns = columns;
            data = new double[rows, columns];
        }

        public Matrix(Matrix other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            Rows = other.Rows;
            Columns = other.Columns;
            data = new double[Rows, Columns];
            Array.Copy(other.data, data, other.data.Length);
        }

        public double this[int i, int j]
        {
            get
            {
                if (i < 0 || i >= Rows || j < 0 || j >= Columns)
                    throw new IndexOutOfRangeException("Індекс виходить за межі матриці");
                return data[i, j];
            }
            set
            {
                if (i < 0 || i >= Rows || j < 0 || j >= Columns)
                    throw new IndexOutOfRangeException("Індекс виходить за межі матриці");
                data[i, j] = value;
            }
        }

        public void RandomInitialize(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentException("Мінімальне значення не може бути більшим за максимальне");

            Random rand = new Random();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    data[i, j] = rand.Next(minValue, maxValue + 1);
                }
            }
        }

        public static Matrix ReadFromFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("Ім'я файлу не може бути порожнім");

            try
            {
                string[] lines = File.ReadAllLines(filename);
                if (lines.Length == 0)
                    throw new Exception("Файл порожній");

                var nonEmptyLines = new System.Collections.Generic.List<string>();
                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        nonEmptyLines.Add(line);
                }

                if (nonEmptyLines.Count == 0)
                    throw new Exception("Файл не містить даних");

                string[] firstLine = nonEmptyLines[0].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                int rows = nonEmptyLines.Count;
                int cols = firstLine.Length;

                Matrix matrix = new Matrix(rows, cols);

                for (int i = 0; i < rows; i++)
                {
                    string[] values = nonEmptyLines[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length != cols)
                        throw new Exception($"Неправильний формат у рядку {i + 1}: очікувалося {cols} значень, отримано {values.Length}");

                    for (int j = 0; j < cols; j++)
                    {
                        if (!double.TryParse(values[j].Replace('.', ','), out double number))
                        {
                            if (!double.TryParse(values[j].Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out number))
                                throw new Exception($"Неправильне число '{values[j]}' у рядку {i + 1}, стовпці {j + 1}");
                        }
                        matrix[i, j] = number;
                    }
                }
                return matrix;
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка читання файлу: {ex.Message}");
            }
        }

        public void WriteToFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("Ім'я файлу не може бути порожнім");

            try
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    for (int i = 0; i < Rows; i++)
                    {
                        for (int j = 0; j < Columns; j++)
                        {
                            writer.Write(data[i, j].ToString("F2"));
                            if (j < Columns - 1)
                                writer.Write(" ");
                        }
                        if (i < Rows - 1)
                            writer.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка запису у файл: {ex.Message}");
            }
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Матриці не можуть бути null");

            if (a.Rows != b.Rows || a.Columns != b.Columns)
                throw new InvalidOperationException($"Неможливо додати матриці: різні розміри ({a.Rows}x{a.Columns} та {b.Rows}x{b.Columns})");

            Matrix result = new Matrix(a.Rows, a.Columns);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Columns; j++)
                    result[i, j] = a[i, j] + b[i, j];
            return result;
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Матриці не можуть бути null");

            if (a.Rows != b.Rows || a.Columns != b.Columns)
                throw new InvalidOperationException($"Неможливо відняти матриці: різні розміри ({a.Rows}x{a.Columns} та {b.Rows}x{b.Columns})");

            Matrix result = new Matrix(a.Rows, a.Columns);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Columns; j++)
                    result[i, j] = a[i, j] - b[i, j];
            return result;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Матриці не можуть бути null");

            if (a.Columns != b.Rows)
                throw new InvalidOperationException($"Неможливо перемножити матриці: кількість стовпців першої матриці ({a.Columns}) не дорівнює кількості рядків другої ({b.Rows})");

            Matrix result = new Matrix(a.Rows, b.Columns);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < b.Columns; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < a.Columns; k++)
                        sum += a[i, k] * b[k, j];
                    result[i, j] = sum;
                }
            }
            return result;
        }

        public static Matrix operator *(Matrix a, double k)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            Matrix result = new Matrix(a.Rows, a.Columns);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Columns; j++)
                    result[i, j] = a[i, j] * k;
            return result;
        }

        public static Matrix operator *(double k, Matrix a)
        {
            return a * k;
        }

        public double MaxNorm()
        {
            if (Rows == 0 || Columns == 0)
                throw new InvalidOperationException("Неможливо обчислити норму порожньої матриці");

            double max = Math.Abs(data[0, 0]);
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    max = Math.Max(max, Math.Abs(data[i, j]));
            return max;
        }

        public double FrobeniusNorm()
        {
            if (Rows == 0 || Columns == 0)
                throw new InvalidOperationException("Неможливо обчислити норму порожньої матриці");

            double sum = 0;
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    sum += data[i, j] * data[i, j];
            return Math.Sqrt(sum);
        }

        public Matrix Transpose()
        {
            Matrix result = new Matrix(Columns, Rows);
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    result[j, i] = data[i, j];
            return result;
        }

        public bool IsSquare()
        {
            return Rows == Columns;
        }

        public Matrix Clone()
        {
            return new Matrix(this);
        }

        public void Clear()
        {
            Array.Clear(data, 0, data.Length);
        }

        public override string ToString()
        {
            if (Rows == 0 || Columns == 0)
                return "Матриця порожня";

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    sb.Append(data[i, j].ToString("F2").PadLeft(8));
                }
                if (i < Rows - 1)
                    sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}