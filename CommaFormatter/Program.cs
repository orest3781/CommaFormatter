using System;
using System.Windows.Forms;

namespace CommaFormatter
    {
    /// <summary>
    /// Entry point of the application.
    /// </summary>
    public static class Program
        {
        [STAThread]
        public static void Main()
            {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new CommaFormatterForm());
            }
        }
    }