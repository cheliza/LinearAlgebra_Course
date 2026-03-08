using System;
using System.Text;

namespace MatrixCalculator
{
    /// <summary>
    /// Розв'язувач СЛАР методом Гауса з вибором головного елемента.
    /// </summary>
    public class GaussianSolver
    {
        private readonly Matrix originalA;
        private readonly Vector originalB;
        private Matrix lu;
        private int[] perm;
        private bool singular;
        private bool inconsistent;
        private SolutionType solutionType;
        private Vector solution;

        // Для покрокового виведення
        private StringBuilder stepLog;
        public string StepLog => stepLog?.ToString();

        public enum SolutionType
        {
            Unique,      // єдиний розв'язок
            None,        // немає розв'язків
            Infinite,    // безліч розв'язків
            Unknown      // ще не розв'язано
        }

        // Властивості для доступу до результатів
        public SolutionType Status => solutionType;
        public Vector Solution => solution?.Clone();
        public bool IsSingular => singular;
        public bool IsInconsistent => inconsistent;
        public Matrix LU => lu?.Clone();
        public int[] Permutation => perm?.Clone() as int[];

        public GaussianSolver(Matrix A, Vector b)
        {
            if (A == null) throw new ArgumentNullException(nameof(A));
            if (b == null) throw new ArgumentNullException(nameof(b));
            if (!A.IsSquare()) throw new ArgumentException("Матриця A має бути квадратною.");
            if (A.Rows != b.Size) throw new ArgumentException("Розміри A та b не збігаються.");

            originalA = new Matrix(A);
            originalB = new Vector(b);
            singular = false;
            inconsistent = false;
            solutionType = SolutionType.Unknown;
            solution = null;
            stepLog = new StringBuilder();
        }

        private void Log(string message)
        {
            stepLog.AppendLine(message);
        }

        private void LogMatrix(string title, Matrix m)
        {
            stepLog.AppendLine(title);
            stepLog.AppendLine(m.ToString());
        }

        private void LogVector(string title, Vector v)
        {
            stepLog.AppendLine(title);
            stepLog.AppendLine(v.ToString());
        }

        /// <summary>
        /// Виконати LU-розклад з вибором головного елемента.
        /// </summary>
        public void Decompose()
        {
            stepLog.Clear();
            Log("=== LU-РОЗКЛАД МЕТОДОМ ГАУСА ===");
            Log($"Початкова матриця A:");
            Log(originalA.ToString());
            Log("");

            int n = originalA.Rows;
            lu = new Matrix(originalA);
            perm = new int[n];
            for (int i = 0; i < n; i++) perm[i] = i;

            const double epsilon = 1e-12;

            for (int k = 0; k < n; k++)
            {
                Log($"\n--- Крок {k + 1} (стовпець {k + 1}) ---");
                Log($"Поточна матриця:");
                Log(lu.ToString());

                // Вибір головного елемента в стовпці k
                int maxRow = k;
                double maxVal = Math.Abs(lu[k, k]);
                for (int i = k + 1; i < n; i++)
                {
                    if (Math.Abs(lu[i, k]) > maxVal)
                    {
                        maxVal = Math.Abs(lu[i, k]);
                        maxRow = i;
                    }
                }

                Log($"Максимальний елемент у стовпці {k + 1}: |{lu[maxRow, k]:F4}| в рядку {maxRow + 1}");

                if (maxVal < epsilon)
                {
                    Log($"❗️ Елемент близький до нуля - матриця вироджена");
                    singular = true;
                }

                // Переставляємо рядки, якщо потрібно
                if (maxRow != k)
                {
                    Log($"\n👉 Міняємо місцями рядки {k + 1} та {maxRow + 1}:");
                    for (int j = 0; j < n; j++)
                    {
                        double tmp = lu[k, j];
                        lu[k, j] = lu[maxRow, j];
                        lu[maxRow, j] = tmp;
                    }
                    // Зберігаємо перестановку
                    int t = perm[k];
                    perm[k] = perm[maxRow];
                    perm[maxRow] = t;

                    Log($"Після перестановки:");
                    Log(lu.ToString());
                }

                if (singular && Math.Abs(lu[k, k]) < epsilon)
                {
                    Log($"Пропускаємо крок через виродженість");
                    continue;
                }

                // Масштабуємо нижню частину стовпця (заповнюємо L)
                Log($"\n👉 Обчислюємо множники для рядків {k + 2} - {n}:");
                for (int i = k + 1; i < n; i++)
                {
                    double factor = lu[i, k] / lu[k, k];
                    Log($"  множник m_{i + 1},{k + 1} = {lu[i, k]:F4} / {lu[k, k]:F4} = {factor:F4}");
                    lu[i, k] = factor;

                    for (int j = k + 1; j < n; j++)
                    {
                        double oldValue = lu[i, j];
                        lu[i, j] -= factor * lu[k, j];
                        Log($"    a_{i + 1},{j + 1}: {oldValue:F4} - {factor:F4} * {lu[k, j]:F4} = {lu[i, j]:F4}");
                    }
                }

                Log($"\nМатриця після кроку {k + 1}:");
                Log(lu.ToString());
            }

            Log("\n=== LU-РОЗКЛАД ЗАВЕРШЕНО ===");
            Log("Результуюча матриця (L на діагоналі - одиниці, U - верхній трикутник):");
            Log(lu.ToString());

            Log("\nВектор перестановок P:");
            for (int i = 0; i < n; i++)
                Log($"  p[{i}] = {perm[i]}");

            // Перевірка на виродженість
            for (int i = 0; i < n; i++)
            {
                if (Math.Abs(lu[i, i]) < epsilon)
                {
                    singular = true;
                    Log($"\n❗️ Елемент на діагоналі U[{i + 1},{i + 1}] = {lu[i, i]:F4} - матриця вироджена");
                    break;
                }
            }

            if (!singular)
                Log("\n✅ Матриця невироджена");
        }

        /// <summary>
        /// Розв'язати систему з використанням виконаного раніше LU-розкладу.
        /// </summary>
        public void Solve()
        {
            stepLog.Clear();
            Log("=== РОЗВ'ЯЗУВАННЯ СЛАР МЕТОДОМ ГАУСА ===");
            Log($"Початкова матриця A:");
            Log(originalA.ToString());
            Log($"\nПочатковий вектор b:");
            Log(originalB.ToString());
            Log("");

            if (lu == null) Decompose();

            int n = originalA.Rows;

            // Пряма підстановка з урахуванням перестановок
            Log("\n--- ЕТАП 1: Пряма підстановка (з урахуванням перестановок) ---");
            Log($"Вектор перестановок P: [{string.Join(", ", perm)}]");

            Vector bPerm = new Vector(n);
            Log("\nПереставляємо праву частину відповідно до перестановок:");
            for (int i = 0; i < n; i++)
            {
                bPerm[i] = originalB[perm[i]];
                Log($"  b'[{i}] = b[{perm[i]}] = {originalB[perm[i]]:F4}");
            }
            Log($"Переставлений вектор b':");
            Log(bPerm.ToString());

            // Розв'язуємо Ly = b (пряма підстановка)
            Log("\n--- ЕТАП 2: Розв'язуємо Ly = b' (пряма підстановка) ---");
            Vector y = new Vector(n);
            for (int i = 0; i < n; i++)
            {
                Log($"\nРівняння для y[{i}]:");
                Log($"  y[{i}] = b'[{i}]");
                double sum = 0;
                for (int j = 0; j < i; j++)
                {
                    Log($"  - L[{i},{j}] * y[{j}] = - {lu[i, j]:F4} * {y[j]:F4}");
                    sum += lu[i, j] * y[j];
                }
                y[i] = bPerm[i] - sum;
                Log($"  y[{i}] = {bPerm[i]:F4} - {sum:F4} = {y[i]:F4}");
            }
            Log($"\nВектор y після прямої підстановки:");
            Log(y.ToString());

            // Розв'язуємо Ux = y (зворотна підстановка)
            Log("\n--- ЕТАП 3: Розв'язуємо Ux = y (зворотна підстановка) ---");
            solution = new Vector(n);
            for (int i = n - 1; i >= 0; i--)
            {
                if (singular && Math.Abs(lu[i, i]) < 1e-12)
                {
                    // Особливий випадок – перевіряємо сумісність
                    bool allZero = true;
                    for (int j = i; j < n; j++)
                        if (Math.Abs(lu[i, j]) > 1e-12) { allZero = false; break; }

                    if (allZero)
                    {
                        if (Math.Abs(y[i]) > 1e-12)
                        {
                            Log($"\n❗️ Рядок {i + 1}: всі коефіцієнти U нульові, а права частина = {y[i]:F4} ≠ 0");
                            Log("   → СИСТЕМА НЕСУМІСНА (немає розв'язків)");
                            inconsistent = true;
                            solutionType = SolutionType.None;
                            solution = null;
                            return;
                        }
                        else
                        {
                            Log($"\n⚠️ Рядок {i + 1}: всі коефіцієнти U нульові, права частина = 0");
                            Log("   → Змінна вільна (безліч розв'язків)");
                            solutionType = SolutionType.Infinite;
                            solution[i] = 0;
                        }
                        continue;
                    }
                }

                Log($"\nРівняння для x[{i}]:");
                Log($"  {lu[i, i]:F4} * x[{i}] = y[{i}]");
                double sum = 0;
                for (int j = i + 1; j < n; j++)
                {
                    Log($"  - U[{i},{j}] * x[{j}] = - {lu[i, j]:F4} * {solution[j]:F4}");
                    sum += lu[i, j] * solution[j];
                }
                solution[i] = (y[i] - sum) / lu[i, i];
                Log($"  x[{i}] = ({y[i]:F4} - {sum:F4}) / {lu[i, i]:F4} = {solution[i]:F4}");
            }

            if (singular && !inconsistent)
            {
                solutionType = SolutionType.Infinite;
                Log("\n⚠️ Матриця вироджена, але система сумісна → БЕЗЛІЧ РОЗВ'ЯЗКІВ");
            }
            else if (!singular && !inconsistent)
            {
                solutionType = SolutionType.Unique;
                Log("\n✅ Матриця невироджена → ЄДИНИЙ РОЗВ'ЯЗОК");
            }

            Log("\n=== РОЗВ'ЯЗОК ===");
            if (solution != null)
                Log(solution.ToString());

            // Обчислюємо нев'язку для перевірки
            if (solutionType == SolutionType.Unique)
            {
                Log("\n=== ПЕРЕВІРКА (нев'язка) ===");
                Vector residual = new Vector(n);
                double maxResidual = 0;
                for (int i = 0; i < n; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < n; j++)
                        sum += originalA[i, j] * solution[j];
                    residual[i] = originalB[i] - sum;
                    maxResidual = Math.Max(maxResidual, Math.Abs(residual[i]));
                    Log($"  r[{i}] = {originalB[i]:F4} - {sum:F4} = {residual[i]:E6}");
                }
                Log($"\nМаксимальна нев'язка: {maxResidual:E6}");
            }
        }

        /// <summary>
        /// Знайти обернену матрицю методом Гауса–Жордана з покроковим виведенням.
        /// </summary>
        public Matrix Inverse()
        {
            stepLog.Clear();
            Log("=== ОБЧИСЛЕННЯ ОБЕРНЕНОЇ МАТРИЦІ МЕТОДОМ ГАУСА-ЖОРДАНА ===");
            Log($"Початкова матриця A:");
            Log(originalA.ToString());
            Log("");

            if (!originalA.IsSquare())
                throw new InvalidOperationException("Обернена матриця існує тільки для квадратних матриць.");

            int n = originalA.Rows;

            // Створюємо розширену матрицю [A | I]
            Matrix augmented = new Matrix(n, 2 * n);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    augmented[i, j] = originalA[i, j];
                augmented[i, n + i] = 1.0;
            }

            Log("Крок 1: Створюємо розширену матрицю [A | I]:");
            Log(augmented.ToString());
            Log("");

            const double epsilon = 1e-12;

            // Прямий хід (приведення до верхньотрикутного вигляду)
            for (int col = 0; col < n; col++)
            {
                Log($"\n--- Крок {col + 1} (стовпець {col + 1}) ---");
                Log("Поточна розширена матриця:");
                Log(augmented.ToString());

                // Вибір головного елемента
                int maxRow = col;
                double maxVal = Math.Abs(augmented[col, col]);
                for (int row = col + 1; row < n; row++)
                {
                    if (Math.Abs(augmented[row, col]) > maxVal)
                    {
                        maxVal = Math.Abs(augmented[row, col]);
                        maxRow = row;
                    }
                }

                Log($"Максимальний елемент у стовпці: |{augmented[maxRow, col]:F4}| в рядку {maxRow + 1}");

                if (maxVal < epsilon)
                    throw new InvalidOperationException("Матриця вироджена, оберненої не існує.");

                // Обмін рядків
                if (maxRow != col)
                {
                    Log($"\n👉 Міняємо місцями рядки {col + 1} та {maxRow + 1}:");
                    for (int j = 0; j < 2 * n; j++)
                    {
                        double tmp = augmented[col, j];
                        augmented[col, j] = augmented[maxRow, j];
                        augmented[maxRow, j] = tmp;
                    }
                    Log("Після перестановки:");
                    Log(augmented.ToString());
                }

                // Нормалізація рядка col
                double pivot = augmented[col, col];
                Log($"\n👉 Ділимо рядок {col + 1} на ведучий елемент {pivot:F4}:");
                for (int j = col; j < 2 * n; j++)
                {
                    double oldValue = augmented[col, j];
                    augmented[col, j] /= pivot;
                    Log($"  a[{col + 1},{j + 1}]: {oldValue:F4} / {pivot:F4} = {augmented[col, j]:F4}");
                }
                Log("Після нормалізації:");
                Log(augmented.ToString());

                // Виключення в інших рядках
                Log($"\n👉 Виключаємо елементи в стовпці {col + 1} з інших рядків:");
                for (int row = 0; row < n; row++)
                {
                    if (row != col && Math.Abs(augmented[row, col]) > epsilon)
                    {
                        double factor = augmented[row, col];
                        Log($"\n   Рядок {row + 1}: віднімаємо {factor:F4} * рядок {col + 1}");
                        for (int j = col; j < 2 * n; j++)
                        {
                            double oldValue = augmented[row, j];
                            augmented[row, j] -= factor * augmented[col, j];
                            Log($"     a[{row + 1},{j + 1}]: {oldValue:F4} - {factor:F4} * {augmented[col, j]:F4} = {augmented[row, j]:F4}");
                        }
                    }
                }
            }

            Log("\n=== РОЗШИРЕНА МАТРИЦЯ ПІСЛЯ ПЕРЕТВОРЕНЬ ===");
            Log(augmented.ToString());

            // Виділяємо обернену матрицю
            Matrix inverse = new Matrix(n, n);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    inverse[i, j] = augmented[i, n + j];

            Log("\n=== ОБЕРНЕНА МАТРИЦЯ A⁻¹ (права частина) ===");
            Log(inverse.ToString());

            // Перевірка
            Log("\n=== ПЕРЕВІРКА: A * A⁻¹ ===");
            Matrix product = originalA * inverse;
            Log(product.ToString());

            return inverse;
        }

        /// <summary>
        /// Обчислення визначника на основі LU-розкладу.
        /// </summary>
        public double Determinant()
        {
            if (lu == null) Decompose();
            if (singular) return 0.0;

            double det = 1.0;
            int n = lu.Rows;
            for (int i = 0; i < n; i++)
                det *= lu[i, i];

            // Знак перестановки
            bool[] visited = new bool[n];
            int sign = 1;
            for (int i = 0; i < n; i++)
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

        /// <summary>
        /// Оцінка числа обумовленості (за максимум-нормою).
        /// </summary>
        public double ConditionNumber()
        {
            if (singular) return double.PositiveInfinity;
            double normA = originalA.MaxNorm();
            Matrix inv = Inverse();
            double normInv = inv.MaxNorm();
            return normA * normInv;
        }

        /// <summary>
        /// Обчислення нев'язки для знайденого розв'язку.
        /// </summary>
        public Vector Residual()
        {
            if (solution == null || solutionType != SolutionType.Unique)
                throw new InvalidOperationException("Немає єдиного розв'язку для обчислення нев'язки.");
            Vector residual = new Vector(originalA.Rows);
            for (int i = 0; i < originalA.Rows; i++)
            {
                double sum = 0;
                for (int j = 0; j < originalA.Columns; j++)
                    sum += originalA[i, j] * solution[j];
                residual[i] = originalB[i] - sum;
            }
            return residual;
        }
    }
}