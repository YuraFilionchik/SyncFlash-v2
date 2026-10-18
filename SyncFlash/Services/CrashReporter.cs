using System.Text;
using System.Windows.Forms;

namespace SyncFlash.Services
{
    /// <summary>
    /// Пишет необработанные исключения в файл и показывает пользователю понятное сообщение.
    /// </summary>
    internal static class CrashReporter
    {
        private static readonly object SyncRoot = new();

        public static void Report(Exception? exception, string source)
        {
            if (exception is null) return;

            var logPath = WriteLog(exception, source);

            try
            {
                MessageBox.Show(
                    $"Произошла непредвиденная ошибка ({source}).{Environment.NewLine}{Environment.NewLine}" +
                    $"{exception.Message}{Environment.NewLine}{Environment.NewLine}" +
                    $"Подробности записаны в файл:{Environment.NewLine}{logPath}",
                    "SyncFlash",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch
            {
                // Показ сообщения не должен приводить к повторному падению.
            }
        }

        private static string WriteLog(Exception exception, string source)
        {
            try
            {
                var directory = ResolveLogDirectory();
                Directory.CreateDirectory(directory);

                var path = Path.Combine(directory, $"crash-{DateTime.Now:yyyyMMdd}.log");
                var text = new StringBuilder()
                    .AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {source}")
                    .AppendLine(exception.ToString())
                    .AppendLine(new string('-', 80))
                    .ToString();

                lock (SyncRoot)
                {
                    File.AppendAllText(path, text, Encoding.UTF8);
                }

                return path;
            }
            catch
            {
                return "(не удалось записать лог)";
            }
        }

        private static string ResolveLogDirectory()
        {
            var local = Path.Combine(AppContext.BaseDirectory, "logs");
            try
            {
                Directory.CreateDirectory(local);
                var probe = Path.Combine(local, ".write-test");
                File.WriteAllText(probe, string.Empty);
                File.Delete(probe);
                return local;
            }
            catch
            {
                var fallback = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "SyncFlash",
                    "logs");

                Directory.CreateDirectory(fallback);
                return fallback;
            }
        }
    }
}
