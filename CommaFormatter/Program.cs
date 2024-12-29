using System;
using System.Windows.Forms;

namespace CommaFormatter
    {
    /// <summary>
    /// The main entry point of the application.
    /// </summary>
    public static class Program
        {
        [STAThread]
        public static void Main()
            {
            // Standard WinForms setup
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new CommaFormatterForm());
            }
        }
    }
