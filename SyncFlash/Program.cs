using SyncFlash.Services;

namespace SyncFlash
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (_, e) => CrashReporter.Report(e.Exception, "UI-поток");
            AppDomain.CurrentDomain.UnhandledException += (_, e) => CrashReporter.Report(e.ExceptionObject as Exception, "AppDomain");
            TaskScheduler.UnobservedTaskException += (_, e) =>
            {
                CrashReporter.Report(e.Exception, "Задача без обработки исключения");
                e.SetObserved();
            };

            Application.Run(new Form1());
        }
    }
}
