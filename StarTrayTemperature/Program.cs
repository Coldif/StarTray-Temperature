using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StarTrayTemperature
{
    internal static class Program
    {
        private static string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log");

        [STAThread]

        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new IconTray());
            }
            catch (Exception ex)
            {
                File.WriteAllText(logPath, ex.ToString());
                MessageBox.Show(ex.ToString(), "StarTray Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            File.WriteAllText(logPath, e.Exception.ToString());
            MessageBox.Show(e.Exception.ToString(), "StarTray Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(1);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                File.WriteAllText(logPath, ex.ToString());
                MessageBox.Show(ex.ToString(), "StarTray Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Environment.Exit(1);
        }
    }
}
