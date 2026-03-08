using System;
using System.Windows.Forms;

namespace MatrixCalculator
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.Run(new MainForm());
        }
    }
}