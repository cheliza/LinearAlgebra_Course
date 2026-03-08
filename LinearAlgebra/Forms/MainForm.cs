using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace MatrixCalculator
{
    public partial class MainForm : Form
    {
        private Matrix matrixA;
        private Matrix matrixB;
        private Matrix matrixResult;
        private Vector vectorA;
        private Vector vectorB;
        private Vector vectorResult;
        private GaussianSolver gaussSolver;
        private StringBuilder stepLog;

        private TabControl tabControl;
        private TabPage tabMatrices;
        private TabPage tabVectors;
        private TabPage tabSLAE;

        // Controls for "Matrices" tab
        private RichTextBox rtbMatrixA;
        private RichTextBox rtbMatrixB;
        private RichTextBox rtbResult;
        private TextBox txtRowsA, txtColsA, txtRowsB, txtColsB;
        private TextBox txtMinRandom, txtMaxRandom;
        private TextBox txtScalar;
        private ComboBox cmbNormType;
        private Button btnGenerateA, btnGenerateB;
        private Button btnReadA, btnReadB;
        private Button btnClearA, btnClearB;
        private Button btnAdd, btnSubtract, btnMultiply, btnMultiplyScalar;
        private Button btnTranspose, btnInverse, btnNorm, btnWriteResult;
        private Button btnClearResult;
        private Button btnApplySizeA, btnApplySizeB;
        private TextBox txtMatrixFileName;
        private Label lblMatrixFileName;

        // Controls for "Vectors" tab
        private RichTextBox rtbVectorA;
        private RichTextBox rtbVectorB;
        private RichTextBox rtbVectorResult;
        private TextBox txtSizeA, txtSizeB;
        private TextBox txtMinRandomVector, txtMaxRandomVector;
        private TextBox txtVectorScalar;
        private Button btnGenerateVectorA, btnGenerateVectorB;
        private Button btnReadVectorA, btnReadVectorB;
        private Button btnClearVectorA, btnClearVectorB;
        private Button btnApplySizeVectorA, btnApplySizeVectorB;
        private Button btnVectorAdd, btnVectorSubtract;
        private Button btnVectorMultiplyScalar;
        private Button btnVectorDot, btnVectorNorm;
        private Button btnVectorWriteResult;
        private Button btnClearVectorResult;
        private Label lblVectorNorm;
        private Label lblVectorDot;
        private TextBox txtVectorFileName;
        private Label lblVectorFileName;

        // Controls for "SLAE" tab
        private NumericUpDown nudSLARSize;
        private DataGridView dgvSLAEA;
        private DataGridView dgvSLAEB;
        private DataGridView dgvSLAEX;
        private RichTextBox rtbSLAELog;
        private TextBox txtSLAEFileName;
        private Label lblSLAEFileName;
        private Button btnSolveSLAE;
        private Button btnLUSLAE;
        private Button btnInverseSLAE;
        private Button btnConditionSLAE;
        private Button btnRandomSLAE;
        private Button btnReadSLAE;
        private Button btnSaveSLAE;
        private Button btnClearSLAE;
        private Button btnWriteSLAEResult;
        private Label lblStatus;

        // Test buttons
        private Button btnTestWellConditioned;
        private Button btnTestIllConditioned;
        private Button btnTestHilbert;
        private Button btnTestDiagonal;

        public MainForm()
        {
            InitializeComponent();
            InitializeData();
        }

        private void InitializeComponent()
        {
            this.Text = "Matrix Calculator";
            this.Size = new Size(1500, 950);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1300, 750);
            this.BackColor = Color.FromArgb(240, 242, 245); // Light gray background

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White
            };

            tabMatrices = new TabPage("Matrices");
            tabVectors = new TabPage("Vectors");
            tabSLAE = new TabPage("SLAE");

            CreateMatricesTab();
            CreateVectorsTab();
            CreateSLAETab();

            tabControl.TabPages.Add(tabMatrices);
            tabControl.TabPages.Add(tabVectors);
            tabControl.TabPages.Add(tabSLAE);

            this.Controls.Add(tabControl);
        }

        private void CreateMatricesTab()
        {
            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2,
                Padding = new Padding(8)
            };

            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));

            Panel panelA = CreateMatrixPanel("Matrix A", out rtbMatrixA, out txtRowsA, out txtColsA, out btnGenerateA, out btnReadA, out btnClearA, out btnApplySizeA, true);
            Panel panelB = CreateMatrixPanel("Matrix B", out rtbMatrixB, out txtRowsB, out txtColsB, out btnGenerateB, out btnReadB, out btnClearB, out btnApplySizeB, false);
            Panel panelResult = CreateMatrixResultPanel();
            Panel panelOperations = CreateMatrixOperationsPanel();

            mainPanel.Controls.Add(panelA, 0, 0);
            mainPanel.Controls.Add(panelB, 1, 0);
            mainPanel.Controls.Add(panelResult, 2, 0);
            mainPanel.Controls.Add(panelOperations, 0, 1);
            mainPanel.SetColumnSpan(panelOperations, 3);

            tabMatrices.Controls.Add(mainPanel);
        }

        private Panel CreateMatrixPanel(string title, out RichTextBox rtb, out TextBox txtRows, out TextBox txtCols, out Button btnGenerate, out Button btnRead, out Button btnClear, out Button btnApplySize, bool isMatrixA)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8),
                BackColor = Color.White
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94), // Dark blue
                Location = new Point(8, 8),
                AutoSize = true
            };

            Label lblRows = new Label { Text = "Rows:", Location = new Point(8, 35), AutoSize = true, ForeColor = Color.FromArgb(52, 73, 94) };
            txtRows = new TextBox { Text = "3", Location = new Point(60, 32), Width = 45, BorderStyle = BorderStyle.FixedSingle };

            Label lblCols = new Label { Text = "Cols:", Location = new Point(115, 35), AutoSize = true, ForeColor = Color.FromArgb(52, 73, 94) };
            txtCols = new TextBox { Text = "3", Location = new Point(175, 32), Width = 45, BorderStyle = BorderStyle.FixedSingle };

            btnApplySize = new Button
            {
                Text = "Set",
                Location = new Point(230, 31),
                Size = new Size(50, 23),
                BackColor = Color.FromArgb(52, 152, 219), // Blue
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 7, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnApplySize.FlatAppearance.BorderSize = 0;

            // Scroll panel for matrix (reduced)
            Panel scrollPanel = new Panel
            {
                Location = new Point(8, 60),
                Size = new Size(380, 190),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(236, 240, 241), // Light gray
                AutoScroll = true
            };

            rtb = new RichTextBox
            {
                Width = 1000,
                Height = 300,
                Font = new Font("Consolas", 9),
                ReadOnly = false,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            scrollPanel.Controls.Add(rtb);

            btnGenerate = new Button
            {
                Text = "Random",
                Location = new Point(8, 255),
                Size = new Size(85, 30),
                BackColor = Color.FromArgb(46, 204, 113), // Green
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGenerate.FlatAppearance.BorderSize = 0;

            btnRead = new Button
            {
                Text = "From file",
                Location = new Point(100, 255),
                Size = new Size(85, 30),
                BackColor = Color.FromArgb(52, 152, 219), // Blue
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRead.FlatAppearance.BorderSize = 0;

            btnClear = new Button
            {
                Text = "Clear",
                Location = new Point(192, 255),
                Size = new Size(85, 30),
                BackColor = Color.FromArgb(231, 76, 60), // Red
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderSize = 0;

            panel.Controls.AddRange(new Control[] {
                lblTitle, lblRows, txtRows, lblCols, txtCols, btnApplySize,
                scrollPanel, btnGenerate, btnRead, btnClear
            });

            btnGenerate.Click += BtnGenerate_Click;
            btnRead.Click += BtnRead_Click;
            btnClear.Click += BtnClear_Click;

            if (isMatrixA)
                btnApplySize.Click += BtnApplySizeA_Click;
            else
                btnApplySize.Click += BtnApplySizeB_Click;

            return panel;
        }

        private Panel CreateMatrixResultPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8),
                BackColor = Color.White
            };

            Label lblTitle = new Label
            {
                Text = "Result",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(8, 8),
                AutoSize = true
            };

            // Scroll panel for result (reduced)
            Panel scrollPanel = new Panel
            {
                Location = new Point(8, 35),
                Size = new Size(380, 250),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(236, 240, 241),
                AutoScroll = true
            };

            rtbResult = new RichTextBox
            {
                Width = 1000,
                Height = 400,
                Font = new Font("Consolas", 9),
                ReadOnly = true,
                BackColor = Color.FromArgb(245, 247, 250),
                BorderStyle = BorderStyle.None
            };

            scrollPanel.Controls.Add(rtbResult);

            btnClearResult = new Button
            {
                Text = "Clear",
                Location = new Point(300, 290),
                Size = new Size(85, 28),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClearResult.FlatAppearance.BorderSize = 0;

            panel.Controls.AddRange(new Control[] {
                lblTitle, scrollPanel, btnClearResult
            });

            btnClearResult.Click += (s, e) => {
                rtbResult.Clear();
            };

            return panel;
        }

        private Panel CreateMatrixOperationsPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8),
                BackColor = Color.White,
                AutoScroll = true
            };

            // Main operations group (reduced)
            GroupBox gbOperations = new GroupBox
            {
                Text = "Matrix Operations",
                Location = new Point(8, 8),
                Size = new Size(500, 75),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            btnAdd = new Button { Text = "A + B", Location = new Point(8, 20), Size = new Size(70, 25), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnSubtract = new Button { Text = "A - B", Location = new Point(85, 20), Size = new Size(70, 25), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnMultiply = new Button { Text = "A * B", Location = new Point(162, 20), Size = new Size(70, 25), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            Label lblScalar = new Label { Text = "k =", Location = new Point(240, 24), AutoSize = true, ForeColor = Color.FromArgb(52, 73, 94) };
            txtScalar = new TextBox { Text = "2", Location = new Point(265, 21), Width = 50, BorderStyle = BorderStyle.FixedSingle };
            btnMultiplyScalar = new Button { Text = "A * k", Location = new Point(325, 20), Size = new Size(70, 25), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnTranspose = new Button { Text = "Transpose A", Location = new Point(402, 20), Size = new Size(80, 25), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };

            gbOperations.Controls.AddRange(new Control[] {
                btnAdd, btnSubtract, btnMultiply, lblScalar, txtScalar,
                btnMultiplyScalar, btnTranspose
            });

            // Additional operations group (reduced)
            GroupBox gbAdvanced = new GroupBox
            {
                Text = "Additional Operations",
                Location = new Point(518, 8),
                Size = new Size(350, 75),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            btnInverse = new Button { Text = "Inverse A", Location = new Point(8, 20), Size = new Size(90, 25), BackColor = Color.FromArgb(155, 89, 182), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            cmbNormType = new ComboBox
            {
                Location = new Point(105, 22),
                Size = new Size(130, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 8),
                BackColor = Color.White
            };
            cmbNormType.Items.AddRange(new object[] { "Max norm", "Euclidean norm" });
            cmbNormType.SelectedIndex = 0;
            btnNorm = new Button { Text = "Norm A", Location = new Point(242, 20), Size = new Size(80, 25), BackColor = Color.FromArgb(241, 196, 15), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };

            gbAdvanced.Controls.AddRange(new Control[] { btnInverse, cmbNormType, btnNorm });

            // Random range group (reduced)
            GroupBox gbRandom = new GroupBox
            {
                Text = "Range",
                Location = new Point(878, 8),
                Size = new Size(230, 75),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            Label lblMin = new Label { Text = "Min:", Location = new Point(8, 24), AutoSize = true, ForeColor = Color.FromArgb(52, 73, 94) };
            txtMinRandom = new TextBox { Text = "-10", Location = new Point(40, 21), Width = 45, BorderStyle = BorderStyle.FixedSingle };
            Label lblMax = new Label { Text = "Max:", Location = new Point(95, 24), AutoSize = true, ForeColor = Color.FromArgb(52, 73, 94) };
            txtMaxRandom = new TextBox { Text = "10", Location = new Point(135, 21), Width = 45, BorderStyle = BorderStyle.FixedSingle };
            gbRandom.Controls.AddRange(new Control[] { lblMin, txtMinRandom, lblMax, txtMaxRandom });

            // Save panel (reduced)
            Panel savePanel = new Panel
            {
                Location = new Point(8, 90),
                Size = new Size(1100, 40),
                BackColor = Color.FromArgb(236, 240, 241),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblMatrixFileName = new Label
            {
                Text = "File:",
                Location = new Point(8, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            txtMatrixFileName = new TextBox
            {
                Text = "matrix_result.txt",
                Location = new Point(55, 9),
                Width = 250,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle
            };

            btnWriteResult = new Button
            {
                Text = "Save",
                Location = new Point(315, 7),
                Size = new Size(100, 26),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnWriteResult.FlatAppearance.BorderSize = 0;

            savePanel.Controls.AddRange(new Control[] { lblMatrixFileName, txtMatrixFileName, btnWriteResult });

            panel.Controls.AddRange(new Control[] { gbOperations, gbAdvanced, gbRandom, savePanel });

            btnAdd.Click += BtnAdd_Click;
            btnSubtract.Click += BtnSubtract_Click;
            btnMultiply.Click += BtnMultiply_Click;
            btnMultiplyScalar.Click += BtnMultiplyScalar_Click;
            btnTranspose.Click += BtnTranspose_Click;
            btnInverse.Click += BtnMatrixInverse_Click;
            btnNorm.Click += BtnNorm_Click;
            btnWriteResult.Click += BtnWriteResult_Click;

            return panel;
        }

        private void CreateVectorsTab()
        {
            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2,
                Padding = new Padding(8)
            };

            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));

            Panel panelA = CreateVectorPanel("Vector A", out rtbVectorA, out txtSizeA, out btnGenerateVectorA, out btnReadVectorA, out btnClearVectorA, out btnApplySizeVectorA, true);
            Panel panelB = CreateVectorPanel("Vector B", out rtbVectorB, out txtSizeB, out btnGenerateVectorB, out btnReadVectorB, out btnClearVectorB, out btnApplySizeVectorB, false);
            Panel panelResult = CreateVectorResultPanel();
            Panel panelOperations = CreateVectorOperationsPanel();

            mainPanel.Controls.Add(panelA, 0, 0);
            mainPanel.Controls.Add(panelB, 1, 0);
            mainPanel.Controls.Add(panelResult, 2, 0);
            mainPanel.Controls.Add(panelOperations, 0, 1);
            mainPanel.SetColumnSpan(panelOperations, 3);

            tabVectors.Controls.Add(mainPanel);
        }

        private Panel CreateVectorPanel(string title, out RichTextBox rtb, out TextBox txtSize, out Button btnGenerate, out Button btnRead, out Button btnClear, out Button btnApplySize, bool isVectorA)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8),
                BackColor = Color.White
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(8, 8),
                AutoSize = true
            };

            Label lblSize = new Label { Text = "Size:", Location = new Point(8, 35), AutoSize = true, ForeColor = Color.FromArgb(52, 73, 94) };
            txtSize = new TextBox { Text = "3", Location = new Point(60, 32), Width = 45, BorderStyle = BorderStyle.FixedSingle };

            btnApplySize = new Button
            {
                Text = "Set",
                Location = new Point(115, 31),
                Size = new Size(50, 23),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 7, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnApplySize.FlatAppearance.BorderSize = 0;

            // Scroll panel for vector (reduced)
            Panel scrollPanel = new Panel
            {
                Location = new Point(8, 60),
                Size = new Size(380, 210),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(236, 240, 241),
                AutoScroll = true
            };

            rtb = new RichTextBox
            {
                Width = 600,
                Height = 350,
                Font = new Font("Consolas", 9),
                ReadOnly = false,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            scrollPanel.Controls.Add(rtb);

            btnGenerate = new Button
            {
                Text = "Random",
                Location = new Point(8, 275),
                Size = new Size(85, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGenerate.FlatAppearance.BorderSize = 0;

            btnRead = new Button
            {
                Text = "From file",
                Location = new Point(100, 275),
                Size = new Size(85, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRead.FlatAppearance.BorderSize = 0;

            btnClear = new Button
            {
                Text = "Clear",
                Location = new Point(192, 275),
                Size = new Size(85, 30),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderSize = 0;

            panel.Controls.AddRange(new Control[] {
                lblTitle, lblSize, txtSize, btnApplySize,
                scrollPanel, btnGenerate, btnRead, btnClear
            });

            btnGenerate.Click += BtnGenerateVector_Click;
            btnRead.Click += BtnReadVector_Click;
            btnClear.Click += BtnClearVector_Click;

            if (isVectorA)
                btnApplySize.Click += BtnApplySizeVectorA_Click;
            else
                btnApplySize.Click += BtnApplySizeVectorB_Click;

            return panel;
        }

        private Panel CreateVectorResultPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8),
                BackColor = Color.White
            };

            Label lblTitle = new Label
            {
                Text = "Result",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(8, 8),
                AutoSize = true
            };

            // Scroll panel for result (reduced)
            Panel scrollPanel = new Panel
            {
                Location = new Point(8, 35),
                Size = new Size(380, 180),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(236, 240, 241),
                AutoScroll = true
            };

            rtbVectorResult = new RichTextBox
            {
                Width = 600,
                Height = 350,
                Font = new Font("Consolas", 9),
                ReadOnly = true,
                BackColor = Color.FromArgb(245, 247, 250),
                BorderStyle = BorderStyle.None
            };

            scrollPanel.Controls.Add(rtbVectorResult);

            Panel infoPanel = new Panel
            {
                Location = new Point(8, 220),
                Size = new Size(380, 50),
                BackColor = Color.FromArgb(236, 240, 241),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblVectorNorm = new Label
            {
                Text = "Norm: ",
                Location = new Point(5, 5),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            lblVectorDot = new Label
            {
                Text = "Dot product: ",
                Location = new Point(5, 25),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            infoPanel.Controls.AddRange(new Control[] { lblVectorNorm, lblVectorDot });

            btnClearVectorResult = new Button
            {
                Text = "Clear",
                Location = new Point(295, 275),
                Size = new Size(85, 28),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClearVectorResult.FlatAppearance.BorderSize = 0;

            panel.Controls.AddRange(new Control[] { lblTitle, scrollPanel, infoPanel, btnClearVectorResult });
            btnClearVectorResult.Click += (s, e) => {
                rtbVectorResult.Clear();
                lblVectorNorm.Text = "Norm: ";
                lblVectorDot.Text = "Dot product: ";
            };

            return panel;
        }

        private Panel CreateVectorOperationsPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8),
                BackColor = Color.White,
                AutoScroll = true
            };

            // Vector operations group (reduced)
            GroupBox gbOperations = new GroupBox
            {
                Text = "Vector Operations",
                Location = new Point(8, 8),
                Size = new Size(550, 70),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            btnVectorAdd = new Button { Text = "A + B", Location = new Point(8, 20), Size = new Size(70, 25), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnVectorSubtract = new Button { Text = "A - B", Location = new Point(85, 20), Size = new Size(70, 25), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnVectorDot = new Button { Text = "A · B", Location = new Point(162, 20), Size = new Size(70, 25), BackColor = Color.FromArgb(155, 89, 182), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            Label lblScalar = new Label { Text = "k =", Location = new Point(240, 24), AutoSize = true, ForeColor = Color.FromArgb(52, 73, 94) };
            txtVectorScalar = new TextBox { Text = "2", Location = new Point(265, 21), Width = 45, BorderStyle = BorderStyle.FixedSingle };
            btnVectorMultiplyScalar = new Button { Text = "A * k", Location = new Point(320, 20), Size = new Size(70, 25), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnVectorNorm = new Button { Text = "Norm A", Location = new Point(397, 20), Size = new Size(75, 25), BackColor = Color.FromArgb(241, 196, 15), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };

            gbOperations.Controls.AddRange(new Control[] {
                btnVectorAdd, btnVectorSubtract, btnVectorDot,
                lblScalar, txtVectorScalar, btnVectorMultiplyScalar,
                btnVectorNorm
            });

            // Random range group (reduced)
            GroupBox gbRandom = new GroupBox
            {
                Text = "Range",
                Location = new Point(568, 8),
                Size = new Size(220, 70),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            Label lblMin = new Label { Text = "Min:", Location = new Point(8, 24), AutoSize = true, ForeColor = Color.FromArgb(52, 73, 94) };
            txtMinRandomVector = new TextBox { Text = "-10", Location = new Point(40, 21), Width = 45, BorderStyle = BorderStyle.FixedSingle };
            Label lblMax = new Label { Text = "Max:", Location = new Point(95, 24), AutoSize = true, ForeColor = Color.FromArgb(52, 73, 94) };
            txtMaxRandomVector = new TextBox { Text = "10", Location = new Point(135, 21), Width = 45, BorderStyle = BorderStyle.FixedSingle };
            gbRandom.Controls.AddRange(new Control[] { lblMin, txtMinRandomVector, lblMax, txtMaxRandomVector });

            // Save panel (reduced)
            Panel savePanel = new Panel
            {
                Location = new Point(8, 85),
                Size = new Size(780, 40),
                BackColor = Color.FromArgb(236, 240, 241),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblVectorFileName = new Label
            {
                Text = "File:",
                Location = new Point(8, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            txtVectorFileName = new TextBox
            {
                Text = "vector_result.txt",
                Location = new Point(50, 9),
                Width = 250,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle
            };

            btnVectorWriteResult = new Button
            {
                Text = "Save",
                Location = new Point(310, 7),
                Size = new Size(100, 26),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnVectorWriteResult.FlatAppearance.BorderSize = 0;

            savePanel.Controls.AddRange(new Control[] { lblVectorFileName, txtVectorFileName, btnVectorWriteResult });

            panel.Controls.AddRange(new Control[] { gbOperations, gbRandom, savePanel });

            btnVectorAdd.Click += BtnVectorAdd_Click;
            btnVectorSubtract.Click += BtnVectorSubtract_Click;
            btnVectorMultiplyScalar.Click += BtnVectorMultiplyScalar_Click;
            btnVectorDot.Click += BtnVectorDot_Click;
            btnVectorNorm.Click += BtnVectorNorm_Click;
            btnVectorWriteResult.Click += BtnVectorWriteResult_Click;

            return panel;
        }

        private void CreateSLAETab()
        {
            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2,
                Padding = new Padding(8)
            };

            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            // Panel for matrix A (reduced)
            Panel panelA = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8),
                BackColor = Color.White
            };

            Label lblA = new Label
            {
                Text = "Matrix A (square)",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(8, 8),
                AutoSize = true
            };

            Panel sizePanel = new Panel
            {
                Location = new Point(8, 30),
                Size = new Size(350, 25)
            };

            Label lblSize = new Label { Text = "Size n:", Location = new Point(0, 5), AutoSize = true, ForeColor = Color.FromArgb(52, 73, 94) };
            nudSLARSize = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 30,
                Value = 3,
                Location = new Point(60, 2),
                Width = 50,
                BorderStyle = BorderStyle.FixedSingle
            };
            Button btnResize = new Button
            {
                Text = "Resize",
                Location = new Point(115, 0),
                Size = new Size(80, 22),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnResize.FlatAppearance.BorderSize = 0;
            btnResize.Click += BtnResizeSLAR_Click;

            sizePanel.Controls.AddRange(new Control[] { lblSize, nudSLARSize, btnResize });

            // Scroll panel for matrix A (reduced)
            Panel scrollPanelA = new Panel
            {
                Location = new Point(8, 60),
                Size = new Size(380, 180),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(236, 240, 241)
            };

            dgvSLAEA = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = true,
                ColumnHeadersHeight = 25,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                Width = 800,
                Height = 150,
                Font = new Font("Segoe UI", 8),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(189, 195, 199)
            };
            dgvSLAEA.EnableHeadersVisualStyles = false;
            dgvSLAEA.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvSLAEA.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSLAEA.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvSLAEA.RowHeadersDefaultCellStyle.ForeColor = Color.White;

            scrollPanelA.Controls.Add(dgvSLAEA);
            panelA.Controls.AddRange(new Control[] { lblA, sizePanel, scrollPanelA });

            // Panel for vector B (reduced)
            Panel panelB = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8),
                BackColor = Color.White
            };

            Label lblB = new Label
            {
                Text = "Vector B (right side)",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(8, 8),
                AutoSize = true
            };

            // Scroll panel for vector B (reduced)
            Panel scrollPanelB = new Panel
            {
                Location = new Point(8, 35),
                Size = new Size(380, 205),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(236, 240, 241)
            };

            dgvSLAEB = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = true,
                ColumnHeadersHeight = 25,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ColumnCount = 1,
                Width = 300,
                Height = 300,
                Font = new Font("Segoe UI", 8),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(189, 195, 199)
            };
            dgvSLAEB.EnableHeadersVisualStyles = false;
            dgvSLAEB.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvSLAEB.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSLAEB.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvSLAEB.RowHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSLAEB.Columns[0].HeaderText = "b";
            dgvSLAEB.Columns[0].Width = 120;

            scrollPanelB.Controls.Add(dgvSLAEB);
            panelB.Controls.AddRange(new Control[] { lblB, scrollPanelB });

            // Panel for result X (reduced)
            Panel panelX = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8),
                BackColor = Color.White
            };

            Label lblX = new Label
            {
                Text = "Solution X",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(8, 8),
                AutoSize = true
            };

            // Scroll panel for result X (reduced)
            Panel scrollPanelX = new Panel
            {
                Location = new Point(8, 35),
                Size = new Size(380, 205),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(236, 240, 241)
            };

            dgvSLAEX = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = true,
                ColumnHeadersHeight = 25,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ColumnCount = 1,
                BackgroundColor = Color.FromArgb(245, 247, 250),
                Width = 300,
                Height = 300,
                Font = new Font("Segoe UI", 8),
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(189, 195, 199)
            };
            dgvSLAEX.EnableHeadersVisualStyles = false;
            dgvSLAEX.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvSLAEX.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSLAEX.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvSLAEX.RowHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSLAEX.Columns[0].HeaderText = "x";
            dgvSLAEX.Columns[0].Width = 120;

            scrollPanelX.Controls.Add(dgvSLAEX);
            panelX.Controls.AddRange(new Control[] { lblX, scrollPanelX });

            // Bottom panel with single log field and test buttons (reduced)
            Panel panelBottom = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8),
                BackColor = Color.White
            };

            // Top button row (reduced)
            Panel topButtonPanel = new Panel
            {
                Location = new Point(8, 8),
                Size = new Size(1150, 40),
                AutoScroll = true
            };

            btnSolveSLAE = new Button
            {
                Text = "Solve",
                Location = new Point(0, 5),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSolveSLAE.FlatAppearance.BorderSize = 0;

            btnLUSLAE = new Button
            {
                Text = "LU-factor",
                Location = new Point(100, 5),
                Size = new Size(85, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLUSLAE.FlatAppearance.BorderSize = 0;

            btnInverseSLAE = new Button
            {
                Text = "Inverse",
                Location = new Point(195, 5),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnInverseSLAE.FlatAppearance.BorderSize = 0;

            btnConditionSLAE = new Button
            {
                Text = "Condition",
                Location = new Point(285, 5),
                Size = new Size(85, 30),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnConditionSLAE.FlatAppearance.BorderSize = 0;

            btnRandomSLAE = new Button
            {
                Text = "Random",
                Location = new Point(380, 5),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRandomSLAE.FlatAppearance.BorderSize = 0;

            btnReadSLAE = new Button
            {
                Text = "Read",
                Location = new Point(470, 5),
                Size = new Size(75, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnReadSLAE.FlatAppearance.BorderSize = 0;

            btnSaveSLAE = new Button
            {
                Text = "Save",
                Location = new Point(555, 5),
                Size = new Size(85, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSaveSLAE.FlatAppearance.BorderSize = 0;

            btnClearSLAE = new Button
            {
                Text = "Clear",
                Location = new Point(650, 5),
                Size = new Size(75, 30),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClearSLAE.FlatAppearance.BorderSize = 0;

            topButtonPanel.Controls.AddRange(new Control[] {
                btnSolveSLAE, btnLUSLAE, btnInverseSLAE, btnConditionSLAE,
                btnRandomSLAE, btnReadSLAE, btnSaveSLAE, btnClearSLAE
            });

            // Single log field (reduced)
            Label lblLog = new Label
            {
                Text = "Results and intermediate solutions:",
                Location = new Point(8, 53),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            Panel logScrollPanel = new Panel
            {
                Location = new Point(8, 73),
                Size = new Size(1150, 130),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(236, 240, 241),
                AutoScroll = true
            };

            rtbSLAELog = new RichTextBox
            {
                Width = 2500,
                Height = 300,
                ReadOnly = true,
                Font = new Font("Consolas", 8),
                BackColor = Color.White,
                WordWrap = false,
                BorderStyle = BorderStyle.None
            };

            logScrollPanel.Controls.Add(rtbSLAELog);

            // Test buttons panel (reduced)
            Panel testPanel = new Panel
            {
                Location = new Point(8, 210),
                Size = new Size(1150, 35),
                BackColor = Color.FromArgb(236, 240, 241),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblTests = new Label
            {
                Text = "TESTS:",
                Location = new Point(8, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            btnTestWellConditioned = new Button
            {
                Text = "Well cond.",
                Location = new Point(70, 6),
                Size = new Size(100, 22),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTestWellConditioned.FlatAppearance.BorderSize = 0;

            btnTestIllConditioned = new Button
            {
                Text = "Ill cond.",
                Location = new Point(180, 6),
                Size = new Size(100, 22),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTestIllConditioned.FlatAppearance.BorderSize = 0;

            btnTestHilbert = new Button
            {
                Text = "Hilbert",
                Location = new Point(290, 6),
                Size = new Size(90, 22),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTestHilbert.FlatAppearance.BorderSize = 0;

            btnTestDiagonal = new Button
            {
                Text = "Diagonal",
                Location = new Point(390, 6),
                Size = new Size(95, 22),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTestDiagonal.FlatAppearance.BorderSize = 0;

            testPanel.Controls.AddRange(new Control[] {
                lblTests, btnTestWellConditioned, btnTestIllConditioned,
                btnTestHilbert, btnTestDiagonal
            });

            // Status (reduced)
            lblStatus = new Label
            {
                Text = "Status: Ready",
                Location = new Point(8, 250),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            // Save results panel (reduced)
            Panel saveResultPanel = new Panel
            {
                Location = new Point(8, 275),
                Size = new Size(1150, 35),
                BackColor = Color.FromArgb(236, 240, 241),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblSLAEFileName = new Label
            {
                Text = "Result file:",
                Location = new Point(8, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            txtSLAEFileName = new TextBox
            {
                Text = "slae_result.txt",
                Location = new Point(110, 7),
                Width = 250,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle
            };

            btnWriteSLAEResult = new Button
            {
                Text = "Save",
                Location = new Point(370, 5),
                Size = new Size(100, 25),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnWriteSLAEResult.FlatAppearance.BorderSize = 0;
            btnWriteSLAEResult.Click += BtnWriteSLAEResult_Click;

            saveResultPanel.Controls.AddRange(new Control[] { lblSLAEFileName, txtSLAEFileName, btnWriteSLAEResult });

            panelBottom.Controls.AddRange(new Control[] {
                topButtonPanel, lblLog, logScrollPanel, testPanel, lblStatus, saveResultPanel
            });

            mainPanel.Controls.Add(panelA, 0, 0);
            mainPanel.Controls.Add(panelB, 1, 0);
            mainPanel.Controls.Add(panelX, 2, 0);
            mainPanel.Controls.Add(panelBottom, 0, 1);
            mainPanel.SetColumnSpan(panelBottom, 3);

            tabSLAE.Controls.Add(mainPanel);

            // Initialize initial size
            UpdateSLARSize(3);

            // Event handlers
            btnSolveSLAE.Click += BtnSolveSLAE_Click;
            btnLUSLAE.Click += BtnLUSLAE_Click;
            btnInverseSLAE.Click += BtnInverseSLAE_Click;
            btnConditionSLAE.Click += BtnConditionSLAE_Click;
            btnRandomSLAE.Click += BtnRandomSLAE_Click;
            btnReadSLAE.Click += BtnReadSLAE_Click;
            btnSaveSLAE.Click += BtnSaveSLAE_Click;
            btnClearSLAE.Click += BtnClearSLAE_Click;

            // Test buttons event handlers
            btnTestWellConditioned.Click += BtnTestWellConditioned_Click;
            btnTestIllConditioned.Click += BtnTestIllConditioned_Click;
            btnTestHilbert.Click += BtnTestHilbert_Click;
            btnTestDiagonal.Click += BtnTestDiagonal_Click;
        }

        private void InitializeData()
        {
            matrixA = new Matrix(3, 3);
            matrixB = new Matrix(3, 3);
            vectorA = new Vector(3);
            vectorB = new Vector(3);
            stepLog = new StringBuilder();
            UpdateMatrixDisplay();
            UpdateVectorDisplay();
        }

        private void UpdateMatrixDisplay()
        {
            if (matrixA != null) rtbMatrixA.Text = matrixA.ToString();
            if (matrixB != null) rtbMatrixB.Text = matrixB.ToString();
        }

        private void UpdateVectorDisplay()
        {
            if (vectorA != null) rtbVectorA.Text = vectorA.ToString();
            if (vectorB != null) rtbVectorB.Text = vectorB.ToString();
        }

        // --- New handlers for matrix size changes ---
        private void BtnApplySizeA_Click(object sender, EventArgs e)
        {
            try
            {
                int rows = int.Parse(txtRowsA.Text);
                int cols = int.Parse(txtColsA.Text);
                matrixA = new Matrix(rows, cols);
                UpdateMatrixDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnApplySizeB_Click(object sender, EventArgs e)
        {
            try
            {
                int rows = int.Parse(txtRowsB.Text);
                int cols = int.Parse(txtColsB.Text);
                matrixB = new Matrix(rows, cols);
                UpdateMatrixDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- New handlers for vector size changes ---
        private void BtnApplySizeVectorA_Click(object sender, EventArgs e)
        {
            try
            {
                int size = int.Parse(txtSizeA.Text);
                vectorA = new Vector(size);
                UpdateVectorDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnApplySizeVectorB_Click(object sender, EventArgs e)
        {
            try
            {
                int size = int.Parse(txtSizeB.Text);
                vectorB = new Vector(size);
                UpdateVectorDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Test examples for SLAE ---
        private void BtnTestWellConditioned_Click(object sender, EventArgs e)
        {
            // Well-conditioned 3x3 matrix with unit solution
            int n = 3;
            nudSLARSize.Value = n;
            UpdateSLARSize(n);

            // Matrix A: triangular matrix with diagonal dominance
            double[,] A = {
                { 10, 1, 2 },
                { 1, 9, 1 },
                { 2, 1, 8 }
            };

            // Vector B: sum of rows (expected solution - all 1)
            double[] b = { 13, 11, 11 };

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    dgvSLAEA.Rows[i].Cells[j].Value = A[i, j].ToString("F2");
                }
                dgvSLAEB.Rows[i].Cells[0].Value = b[i].ToString("F2");
            }

            rtbSLAELog.Clear();
            LogSLAE("=== TEST: WELL-CONDITIONED MATRIX ===");
            LogSLAE("Matrix with diagonal dominance:");
            LogSLAE("10   1   2");
            LogSLAE(" 1   9   1");
            LogSLAE(" 2   1   8");
            LogSLAE("Expected solution: x = [1, 1, 1]^T");

            // Solve immediately
            BtnSolveSLAE_Click(sender, e);
        }

        private void BtnTestIllConditioned_Click(object sender, EventArgs e)
        {
            // Ill-conditioned matrix (almost singular)
            int n = 3;
            nudSLARSize.Value = n;
            UpdateSLARSize(n);

            // Matrix A: almost singular
            double[,] A = {
                { 1, 2, 3 },
                { 2, 4, 6 },  // linearly dependent row
                { 3, 5, 7 }
            };

            // Vector B
            double[] b = { 6, 12, 15 };

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    dgvSLAEA.Rows[i].Cells[j].Value = A[i, j].ToString("F2");
                }
                dgvSLAEB.Rows[i].Cells[0].Value = b[i].ToString("F2");
            }

            rtbSLAELog.Clear();
            LogSLAE("=== TEST: ILL-CONDITIONED MATRIX ===");
            LogSLAE("Matrix with almost linearly dependent rows:");
            LogSLAE("1   2   3");
            LogSLAE("2   4   6  (linearly dependent on first)");
            LogSLAE("3   5   7");
            LogSLAE("Expected: small determinant, large condition number");

            // Calculate condition number immediately
            BtnConditionSLAE_Click(sender, e);
        }

        private void BtnTestHilbert_Click(object sender, EventArgs e)
        {
            // Hilbert matrix (classic example of ill-conditioned)
            int n = 4;
            nudSLARSize.Value = n;
            UpdateSLARSize(n);

            // Hilbert matrix H[i,j] = 1/(i+j+1)
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < n; j++)
                {
                    double val = 1.0 / (i + j + 1);
                    dgvSLAEA.Rows[i].Cells[j].Value = val.ToString("F6");
                    sum += val;
                }
                // Right side - sum of row (expected solution - all 1)
                dgvSLAEB.Rows[i].Cells[0].Value = sum.ToString("F6");
            }

            rtbSLAELog.Clear();
            LogSLAE($"=== TEST: HILBERT MATRIX {n}x{n} ===");
            LogSLAE("Classic example of ill-conditioned matrix");
            LogSLAE("H[i,j] = 1/(i+j-1)");
            LogSLAE("Expected solution: x = [1, 1, 1, 1]^T");
            LogSLAE("Should have very large condition number");

            // Solve immediately
            BtnSolveSLAE_Click(sender, e);
        }

        private void BtnTestDiagonal_Click(object sender, EventArgs e)
        {
            // Diagonal matrix (perfectly conditioned)
            int n = 4;
            nudSLARSize.Value = n;
            UpdateSLARSize(n);

            // Diagonal matrix
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                        dgvSLAEA.Rows[i].Cells[j].Value = (i + 1).ToString();
                    else
                        dgvSLAEA.Rows[i].Cells[j].Value = "0";
                }
                // Right side
                dgvSLAEB.Rows[i].Cells[0].Value = (i + 1).ToString();
            }

            rtbSLAELog.Clear();
            LogSLAE("=== TEST: DIAGONAL MATRIX ===");
            LogSLAE("Perfectly conditioned matrix:");
            LogSLAE("1 0 0 0");
            LogSLAE("0 2 0 0");
            LogSLAE("0 0 3 0");
            LogSLAE("0 0 0 4");
            LogSLAE("Expected solution: x = [1, 1, 1, 1]^T");
            LogSLAE("Condition number should be small");

            // Solve immediately
            BtnSolveSLAE_Click(sender, e);
        }

        // --- Event handlers for matrices ---
        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                bool isMatrixA = btn == btnGenerateA;
                TextBox txtRows = isMatrixA ? txtRowsA : txtRowsB;
                TextBox txtCols = isMatrixA ? txtColsA : txtColsB;

                int rows = int.Parse(txtRows.Text);
                int cols = int.Parse(txtCols.Text);
                int min = int.Parse(txtMinRandom.Text);
                int max = int.Parse(txtMaxRandom.Text);

                Matrix matrix = new Matrix(rows, cols);
                matrix.RandomInitialize(min, max);

                if (isMatrixA) matrixA = matrix;
                else matrixB = matrix;

                UpdateMatrixDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRead_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                bool isMatrixA = btn == btnReadA;

                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = isMatrixA ? "Select file with matrix A" : "Select file with matrix B";
                    ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        Matrix matrix = Matrix.ReadFromFile(ofd.FileName);
                        if (isMatrixA)
                        {
                            matrixA = matrix;
                            txtRowsA.Text = matrix.Rows.ToString();
                            txtColsA.Text = matrix.Columns.ToString();
                        }
                        else
                        {
                            matrixB = matrix;
                            txtRowsB.Text = matrix.Rows.ToString();
                            txtColsB.Text = matrix.Columns.ToString();
                        }
                        UpdateMatrixDisplay();
                        MessageBox.Show("Matrix successfully loaded!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"File reading error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == btnClearA) rtbMatrixA.Clear();
            else if (btn == btnClearB) rtbMatrixB.Clear();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                matrixResult = matrixA + matrixB;
                rtbResult.Text = "A + B =\n" + matrixResult.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSubtract_Click(object sender, EventArgs e)
        {
            try
            {
                matrixResult = matrixA - matrixB;
                rtbResult.Text = "A - B =\n" + matrixResult.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnMultiply_Click(object sender, EventArgs e)
        {
            try
            {
                matrixResult = matrixA * matrixB;
                rtbResult.Text = "A * B =\n" + matrixResult.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnMultiplyScalar_Click(object sender, EventArgs e)
        {
            try
            {
                double k = double.Parse(txtScalar.Text);
                matrixResult = matrixA * k;
                rtbResult.Text = $"A * {k} =\n" + matrixResult.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTranspose_Click(object sender, EventArgs e)
        {
            try
            {
                matrixResult = matrixA.Transpose();
                rtbResult.Text = "A^T =\n" + matrixResult.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnMatrixInverse_Click(object sender, EventArgs e)
        {
            try
            {
                if (!matrixA.IsSquare())
                {
                    MessageBox.Show("Inverse matrix exists only for square matrices!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                gaussSolver = new GaussianSolver(matrixA, new Vector(matrixA.Rows));
                Matrix inv = gaussSolver.Inverse();
                matrixResult = inv;
                rtbResult.Text = "A⁻¹ =\n" + inv.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnNorm_Click(object sender, EventArgs e)
        {
            try
            {
                double norm;
                string normName;

                if (cmbNormType.SelectedIndex == 0)
                {
                    norm = matrixA.MaxNorm();
                    normName = "Max norm";
                }
                else
                {
                    norm = matrixA.FrobeniusNorm();
                    normName = "Euclidean norm";
                }

                rtbResult.Text = $"Matrix norm A ({cmbNormType.Text}) = {norm:F4}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnWriteResult_Click(object sender, EventArgs e)
        {
            if (matrixResult == null)
            {
                MessageBox.Show("No result to save", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string fileName = txtMatrixFileName.Text.Trim();
                if (string.IsNullOrEmpty(fileName))
                    fileName = "matrix_result.txt";
                if (!fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    fileName += ".txt";

                matrixResult.WriteToFile(fileName);
                MessageBox.Show($"Result successfully saved to file: {fileName}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while saving: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Event handlers for vectors ---
        private void BtnGenerateVector_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                bool isVectorA = btn == btnGenerateVectorA;
                TextBox txtSize = isVectorA ? txtSizeA : txtSizeB;

                int size = int.Parse(txtSize.Text);
                int min = int.Parse(txtMinRandomVector.Text);
                int max = int.Parse(txtMaxRandomVector.Text);

                Vector vector = new Vector(size);
                vector.RandomInitialize(min, max);

                if (isVectorA) vectorA = vector;
                else vectorB = vector;

                UpdateVectorDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReadVector_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                bool isVectorA = btn == btnReadVectorA;

                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = isVectorA ? "Select file with vector A" : "Select file with vector B";
                    ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        Vector vector = Vector.ReadFromFile(ofd.FileName);
                        if (isVectorA)
                        {
                            vectorA = vector;
                            txtSizeA.Text = vector.Size.ToString();
                        }
                        else
                        {
                            vectorB = vector;
                            txtSizeB.Text = vector.Size.ToString();
                        }
                        UpdateVectorDisplay();
                        MessageBox.Show("Vector successfully loaded!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"File reading error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearVector_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == btnClearVectorA)
                rtbVectorA.Clear();
            else if (btn == btnClearVectorB)
                rtbVectorB.Clear();
        }

        private void BtnVectorAdd_Click(object sender, EventArgs e)
        {
            try
            {
                vectorResult = vectorA + vectorB;
                rtbVectorResult.Text = "A + B =\n" + vectorResult.ToString();
                lblVectorNorm.Text = $"Result norm: {vectorResult.Norm():F4}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnVectorSubtract_Click(object sender, EventArgs e)
        {
            try
            {
                vectorResult = vectorA - vectorB;
                rtbVectorResult.Text = "A - B =\n" + vectorResult.ToString();
                lblVectorNorm.Text = $"Result norm: {vectorResult.Norm():F4}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnVectorMultiplyScalar_Click(object sender, EventArgs e)
        {
            try
            {
                double k = double.Parse(txtVectorScalar.Text);
                vectorResult = vectorA * k;
                rtbVectorResult.Text = $"A * {k} =\n" + vectorResult.ToString();
                lblVectorNorm.Text = $"Result norm: {vectorResult.Norm():F4}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnVectorDot_Click(object sender, EventArgs e)
        {
            try
            {
                double dot = vectorA.Dot(vectorB);
                lblVectorDot.Text = $"Dot product: {dot:F4}";
                rtbVectorResult.Text = $"A · B = {dot:F4}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnVectorNorm_Click(object sender, EventArgs e)
        {
            try
            {
                double norm = vectorA.Norm();
                lblVectorNorm.Text = $"Vector A norm: {norm:F4}";
                rtbVectorResult.Text = $"||A|| = {norm:F4}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnVectorWriteResult_Click(object sender, EventArgs e)
        {
            if (vectorResult == null)
            {
                MessageBox.Show("No result to save", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string fileName = txtVectorFileName.Text.Trim();
                if (string.IsNullOrEmpty(fileName))
                    fileName = "vector_result.txt";
                if (!fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    fileName += ".txt";

                vectorResult.WriteToFile(fileName);
                MessageBox.Show($"Result successfully saved to file: {fileName}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while saving: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- SLAE handlers ---
        private void UpdateSLARSize(int n)
        {
            if (dgvSLAEA == null || dgvSLAEB == null || dgvSLAEX == null)
                return;

            int columnWidth = Math.Max(50, 350 / n);

            // Update matrix A
            dgvSLAEA.ColumnCount = n;
            dgvSLAEA.RowCount = n;

            for (int j = 0; j < n; j++)
            {
                dgvSLAEA.Columns[j].Width = columnWidth;
                dgvSLAEA.Columns[j].HeaderText = $"x{j + 1}";
            }

            for (int i = 0; i < n; i++)
            {
                dgvSLAEA.Rows[i].Height = 22;
                dgvSLAEA.Rows[i].HeaderCell.Value = $"r{i + 1}";
                for (int j = 0; j < n; j++)
                {
                    if (dgvSLAEA.Rows[i].Cells[j].Value == null || string.IsNullOrEmpty(dgvSLAEA.Rows[i].Cells[j].Value.ToString()))
                        dgvSLAEA.Rows[i].Cells[j].Value = "0";
                }
            }

            dgvSLAEA.Width = Math.Max(350, n * (columnWidth + 15));
            dgvSLAEA.Height = Math.Max(250, n * 25);

            // Update vector B
            dgvSLAEB.RowCount = n;
            dgvSLAEB.Columns[0].Width = 100;

            for (int i = 0; i < n; i++)
            {
                dgvSLAEB.Rows[i].Height = 22;
                dgvSLAEB.Rows[i].HeaderCell.Value = $"r{i + 1}";
                if (dgvSLAEB.Rows[i].Cells[0].Value == null || string.IsNullOrEmpty(dgvSLAEB.Rows[i].Cells[0].Value.ToString()))
                    dgvSLAEB.Rows[i].Cells[0].Value = "0";
            }

            dgvSLAEB.Height = Math.Max(250, n * 25);
            dgvSLAEB.Width = 250;

            // Update result vector X
            dgvSLAEX.RowCount = n;
            dgvSLAEX.Columns[0].Width = 100;

            for (int i = 0; i < n; i++)
            {
                dgvSLAEX.Rows[i].Height = 22;
                dgvSLAEX.Rows[i].HeaderCell.Value = $"x{i + 1}";
                dgvSLAEX.Rows[i].Cells[0].Value = "";
            }

            dgvSLAEX.Height = Math.Max(250, n * 25);
            dgvSLAEX.Width = 250;
        }

        private void BtnResizeSLAR_Click(object sender, EventArgs e)
        {
            int n = (int)nudSLARSize.Value;
            UpdateSLARSize(n);
            rtbSLAELog.Clear();
        }

        private Matrix GetMatrixFromDGV()
        {
            int n = dgvSLAEA.RowCount;
            Matrix A = new Matrix(n, n);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    double val;
                    if (!double.TryParse(dgvSLAEA.Rows[i].Cells[j].Value?.ToString(), out val))
                        val = 0;
                    A[i, j] = val;
                }
            return A;
        }

        private Vector GetVectorFromDGV()
        {
            int n = dgvSLAEB.RowCount;
            Vector b = new Vector(n);
            for (int i = 0; i < n; i++)
            {
                double val;
                if (!double.TryParse(dgvSLAEB.Rows[i].Cells[0].Value?.ToString(), out val))
                    val = 0;
                b[i] = val;
            }
            return b;
        }

        private void DisplaySolution(Vector x)
        {
            for (int i = 0; i < x.Size; i++)
                dgvSLAEX.Rows[i].Cells[0].Value = x[i].ToString("F6");
        }

        private void LogSLAE(string text)
        {
            rtbSLAELog.AppendText(text + "\n");
            rtbSLAELog.AppendText(new string('-', 40) + "\n");
            rtbSLAELog.SelectionStart = rtbSLAELog.Text.Length;
            rtbSLAELog.ScrollToCaret();
        }

        private void BtnSolveSLAE_Click(object sender, EventArgs e)
        {
            try
            {
                rtbSLAELog.Clear();

                Matrix A = GetMatrixFromDGV();
                Vector b = GetVectorFromDGV();

                if (!A.IsSquare())
                {
                    MessageBox.Show("Matrix A must be square!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                gaussSolver = new GaussianSolver(A, b);
                gaussSolver.Solve();

                // Output log
                rtbSLAELog.Text = gaussSolver.StepLog;

                LogSLAE("=== SLAE SOLUTION RESULT ===");

                switch (gaussSolver.Status)
                {
                    case GaussianSolver.SolutionType.Unique:
                        Vector x = gaussSolver.Solution;
                        DisplaySolution(x);
                        Vector residual = gaussSolver.Residual();
                        double maxResidual = 0;
                        for (int i = 0; i < residual.Size; i++)
                            maxResidual = Math.Max(maxResidual, Math.Abs(residual[i]));

                        LogSLAE($"Status: UNIQUE SOLUTION");
                        LogSLAE($"Max residual: {maxResidual:E6}");
                        LogSLAE("Residual vector:");
                        for (int i = 0; i < residual.Size; i++)
                            LogSLAE($"r[{i + 1}] = {residual[i]:E6}");

                        lblStatus.Text = "Status: Unique solution found";
                        break;

                    case GaussianSolver.SolutionType.None:
                        for (int i = 0; i < b.Size; i++)
                            dgvSLAEX.Rows[i].Cells[0].Value = "---";
                        LogSLAE("Status: SYSTEM HAS NO SOLUTIONS");
                        LogSLAE("System matrix is singular, right side is incompatible.");
                        lblStatus.Text = "Status: No solutions";
                        break;

                    case GaussianSolver.SolutionType.Infinite:
                        for (int i = 0; i < b.Size; i++)
                            dgvSLAEX.Rows[i].Cells[0].Value = "∞ (infinite)";
                        LogSLAE("Status: SYSTEM HAS INFINITE SOLUTIONS");
                        LogSLAE("System matrix is singular, but right side is compatible.");
                        lblStatus.Text = "Status: Infinite solutions";
                        break;
                }

                LogSLAE($"Determinant: {gaussSolver.Determinant():E6}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLUSLAE_Click(object sender, EventArgs e)
        {
            try
            {
                rtbSLAELog.Clear();

                Matrix A = GetMatrixFromDGV();
                Vector b = GetVectorFromDGV();

                gaussSolver = new GaussianSolver(A, b);
                gaussSolver.Decompose();

                // Output log
                rtbSLAELog.Text = gaussSolver.StepLog;

                Matrix lu = gaussSolver.LU;
                int[] perm = gaussSolver.Permutation;

                LogSLAE("=== LU-FACTORIZATION ===");
                LogSLAE("Resulting matrix (L on diagonal are ones, U is upper triangular):");
                LogSLAE(lu.ToString());

                LogSLAE("Permutation vector:");
                string permStr = "";
                for (int i = 0; i < perm.Length; i++)
                    permStr += $"p[{i}] = {perm[i]}  ";
                LogSLAE(permStr);

                LogSLAE($"Matrix is {(gaussSolver.IsSingular ? "SINGULAR" : "NON-SINGULAR")}");

                lblStatus.Text = "Status: LU-factorization completed";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnInverseSLAE_Click(object sender, EventArgs e)
        {
            try
            {
                rtbSLAELog.Clear();

                Matrix A = GetMatrixFromDGV();

                if (!A.IsSquare())
                {
                    MessageBox.Show("Inverse matrix exists only for square matrices!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                gaussSolver = new GaussianSolver(A, new Vector(A.Rows));
                Matrix inv = gaussSolver.Inverse();

                // Output log
                rtbSLAELog.Text = gaussSolver.StepLog;

                LogSLAE("=== INVERSE MATRIX ===");
                LogSLAE(inv.ToString());

                // Verification
                Matrix product = A * inv;
                LogSLAE("Verification: A * A⁻¹ (should be identity):");
                LogSLAE(product.ToString());

                lblStatus.Text = "Status: Inverse matrix calculated";
            }
            catch (Exception ex)
            {
                rtbSLAELog.AppendText($"\nERROR: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnConditionSLAE_Click(object sender, EventArgs e)
        {
            try
            {
                rtbSLAELog.Clear();
                LogSLAE("=== CONDITION NUMBER CALCULATION ===");

                Matrix A = GetMatrixFromDGV();

                LogSLAE("Step 1: Calculate matrix norm A (max-norm)");
                double normA = A.MaxNorm();
                LogSLAE($"||A|| = {normA:E6}");

                LogSLAE("Step 2: Calculate inverse matrix A⁻¹");
                gaussSolver = new GaussianSolver(A, new Vector(A.Rows));
                Matrix inv = gaussSolver.Inverse();

                rtbSLAELog.AppendText(gaussSolver.StepLog);

                LogSLAE("Step 3: Calculate inverse matrix norm");
                double normInv = inv.MaxNorm();
                LogSLAE($"||A⁻¹|| = {normInv:E6}");

                double cond = normA * normInv;

                LogSLAE($"Condition number: {cond:E6}");
                LogSLAE($"Step 4: cond(A) = ||A|| * ||A⁻¹|| = {cond:E6}");

                if (cond > 1e10)
                {
                    LogSLAE("Conclusion: Matrix is ILL-CONDITIONED (cond > 1e10)");
                }
                else if (cond > 1e5)
                {
                    LogSLAE("Conclusion: Matrix is ill-conditioned (cond > 1e5)");
                }
                else if (cond > 1e3)
                {
                    LogSLAE("Conclusion: Matrix is moderately conditioned (cond > 1e3)");
                }
                else
                {
                    LogSLAE("Conclusion: Matrix is WELL-CONDITIONED");
                }

                lblStatus.Text = "Status: Condition number calculated";
            }
            catch (Exception ex)
            {
                rtbSLAELog.AppendText($"\nERROR: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRandomSLAE_Click(object sender, EventArgs e)
        {
            try
            {
                Random rand = new Random();
                int n = (int)nudSLARSize.Value;
                UpdateSLARSize(n);

                for (int i = 0; i < n; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < n; j++)
                    {
                        if (i != j)
                        {
                            double val = rand.Next(-5, 6);
                            dgvSLAEA.Rows[i].Cells[j].Value = val.ToString();
                            sum += Math.Abs(val);
                        }
                    }
                    double diag = sum + rand.Next(1, 5) + rand.NextDouble();
                    dgvSLAEA.Rows[i].Cells[i].Value = diag.ToString("F2");
                    dgvSLAEB.Rows[i].Cells[0].Value = rand.Next(-10, 11).ToString();
                }

                rtbSLAELog.Clear();
                LogSLAE("Random SLAE with diagonal dominance generated.");

                lblStatus.Text = "Status: Random SLAE generated";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReadSLAE_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = "Select file with equation system";
                    ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string[] lines = File.ReadAllLines(ofd.FileName);
                        var nonEmptyLines = new List<string>();
                        foreach (string line in lines)
                            if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                                nonEmptyLines.Add(line);

                        int n = nonEmptyLines.Count / 2;
                        nudSLARSize.Value = n;
                        UpdateSLARSize(n);

                        for (int i = 0; i < n; i++)
                        {
                            string[] values = nonEmptyLines[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int j = 0; j < n; j++)
                                dgvSLAEA.Rows[i].Cells[j].Value = values[j];
                        }
                        for (int i = 0; i < n; i++)
                            dgvSLAEB.Rows[i].Cells[0].Value = nonEmptyLines[n + i];

                        rtbSLAELog.Clear();
                        LogSLAE($"System loaded from file: {ofd.FileName}");

                        MessageBox.Show("System successfully loaded!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSaveSLAE_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Title = "Save equation system";
                    sfd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                    sfd.FileName = "slae_system.txt";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter sw = new StreamWriter(sfd.FileName))
                        {
                            int n = dgvSLAEA.RowCount;

                            for (int i = 0; i < n; i++)
                            {
                                for (int j = 0; j < n; j++)
                                {
                                    sw.Write(dgvSLAEA.Rows[i].Cells[j].Value);
                                    if (j < n - 1)
                                        sw.Write(" ");
                                }
                                sw.WriteLine();
                            }

                            for (int i = 0; i < n; i++)
                            {
                                sw.WriteLine(dgvSLAEB.Rows[i].Cells[0].Value);
                            }
                        }

                        MessageBox.Show("System successfully saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnWriteSLAEResult_Click(object sender, EventArgs e)
        {
            if (gaussSolver == null || gaussSolver.Status == GaussianSolver.SolutionType.Unknown)
            {
                MessageBox.Show("Solve SLAE first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string fileName = txtSLAEFileName.Text.Trim();
                if (string.IsNullOrEmpty(fileName))
                    fileName = "slae_result.txt";
                if (!fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    fileName += ".txt";

                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.WriteLine("# SYSTEM OF LINEAR ALGEBRAIC EQUATIONS SOLUTION RESULT");
                    sw.WriteLine($"# Dimension: {dgvSLAEA.RowCount}");
                    sw.WriteLine();

                    sw.WriteLine("Matrix A:");
                    for (int i = 0; i < dgvSLAEA.RowCount; i++)
                    {
                        for (int j = 0; j < dgvSLAEA.ColumnCount; j++)
                            sw.Write($"{dgvSLAEA.Rows[i].Cells[j].Value} ");
                        sw.WriteLine();
                    }
                    sw.WriteLine();

                    sw.WriteLine("Vector B:");
                    for (int i = 0; i < dgvSLAEB.RowCount; i++)
                        sw.WriteLine(dgvSLAEB.Rows[i].Cells[0].Value);
                    sw.WriteLine();

                    sw.WriteLine("Status: " + gaussSolver.Status.ToString());
                    sw.WriteLine($"Determinant: {gaussSolver.Determinant():E6}");

                    if (gaussSolver.Status == GaussianSolver.SolutionType.Unique)
                    {
                        sw.WriteLine("\nSolution X:");
                        for (int i = 0; i < gaussSolver.Solution.Size; i++)
                            sw.WriteLine($"x{i + 1} = {gaussSolver.Solution[i]:F6}");

                        Vector residual = gaussSolver.Residual();
                        sw.WriteLine("\nResidual:");
                        for (int i = 0; i < residual.Size; i++)
                            sw.WriteLine($"r{i + 1} = {residual[i]:E6}");
                    }
                    else if (gaussSolver.Status == GaussianSolver.SolutionType.None)
                    {
                        sw.WriteLine("\nSystem has no solutions.");
                    }
                    else if (gaussSolver.Status == GaussianSolver.SolutionType.Infinite)
                    {
                        sw.WriteLine("\nSystem has infinite solutions.");
                    }
                }

                MessageBox.Show($"Result successfully saved to file: {fileName}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while saving: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearSLAE_Click(object sender, EventArgs e)
        {
            int n = (int)nudSLARSize.Value;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    dgvSLAEA.Rows[i].Cells[j].Value = "0";
                dgvSLAEB.Rows[i].Cells[0].Value = "0";
                dgvSLAEX.Rows[i].Cells[0].Value = "";
            }
            rtbSLAELog.Clear();
            lblStatus.Text = "Status: Ready";
            gaussSolver = null;
        }
    }
}