using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace SyncFlash.Services
{
    public interface IFileSyncService
    {
        Task<List<Queue>> AnalyzeFilesAsync(Project project);
        Task SyncFilesAsync(List<Queue> queue, IProgress<string> progress,IProgress<int> progressbar, CancellationToken cancellationToken);
    }

    public class FileSyncService : IFileSyncService
    {
     
        public async Task<List<Queue>> AnalyzeFilesAsync(Project project)
        {
            return await Task.Run(() =>
            {
                var queueManager = new QueueManager();
                var onlineDirs = project.OnlineDirs.Where(x => x.PC_Name == Environment.MachineName || x.PC_Name == CONSTS.FlashDrive).ToList();

                if (onlineDirs.Count <= 1) return queueManager.GetQueue(); // Если доступна только 1 папка — синхронизация не нужна

                // Читаем файлы во всех папках проекта
                foreach (var dir in onlineDirs) dir.AllFiles(false);

                // Перебираем все файлы во всех папках
                foreach (var dir in onlineDirs)
                {
                    foreach (var file in dir.AllFiles())
                    {
                        string relativePath = Form1.GetRelativePath(file.Key, dir.Dir);
                        var newestFile = file;
                        string sourceDir = dir.Dir;

                        // Ищем этот же файл в других папках проекта
                        foreach (var otherDir in onlineDirs.Where(d => d != dir))
                        {
                            var foundFile = otherDir.FindFile(relativePath);

                            if (string.IsNullOrEmpty(foundFile.Key)) // Если файла нет в другой папке → создаем новую запись в очереди
                            {
                                queueManager.AddToQueue(newestFile.Key,
                                    otherDir.Dir + relativePath,
                                    sourceDir,
                                    otherDir.Dir,
                                    newestFile.Value,
                                    DateTime.MinValue, // В целевой папке файла нет
                                    true);
                            }
                            else if (newestFile.Value > foundFile.Value) // Если наш файл новее файла в другой папке
                            {
                                queueManager.AddToQueue(newestFile.Key,
                                    foundFile.Key,
                                    sourceDir,
                                    otherDir.Dir,
                                    newestFile.Value,
                                    foundFile.Value,
                                    false);
                            }
                            else if (newestFile.Value < foundFile.Value) // Если в другой папке файл новее
                            {
                                newestFile = foundFile;
                                sourceDir = otherDir.Dir;
                            }
                        }
                    }
                }

                return queueManager.GetQueue();
            });
        }


        public async Task SyncFilesAsync(List<Queue> queue, IProgress<string> progress, IProgress<int> progressbar, CancellationToken cancellationToken)
        {
            var errors = new List<string>();

            foreach (var file in queue)
            {
                cancellationToken.ThrowIfCancellationRequested(); // Прерывание при отмене
                progressbar.Report(queue.IndexOf(file) + 1);
                progress.Report($"Copying: {file.SourceFile} -> {file.TargetFile}");

                try
                {
                    await Task.Run(() => File.Copy(file.SourceFile, file.TargetFile, true), cancellationToken);
                }
                catch (System.IO.DirectoryNotFoundException ex)
                {
                    Directory.CreateDirectory(Directory.GetParent(file.TargetFile).ToString());
                    await Task.Run(() => File.Copy(file.SourceFile, file.TargetFile, true), cancellationToken);

                    progress.Report($"Created directory {Directory.GetParent(file.TargetFile)}");
                }
                catch (Exception ex)
                {
                    progress.Report($"Error copying {file.SourceFile}: {ex.Message}");
                    errors.Add(file.SourceFile);
                }
            }

            progress.Report($"End of synchronization");
            progressbar.Report(100);
            progress.Report("------------------------------");
            progress.Report($"Total files: {queue.Count}");
            progress.Report($"Errors: {errors.Count} ==>");
            foreach (var error in errors)
            {
                progress.Report(error);
            }
        }

    }

    public class QueueManager
    {
        private readonly List<Queue> _queue = new List<Queue>();
        private readonly Dictionary<string, KeyValuePair<string, DateTime>> _latestFiles = new Dictionary<string, KeyValuePair<string, DateTime>>();

        /// <summary>
        /// Добавляет файл в очередь, автоматически выбирая самый свежий SourceFile.
        /// </summary>
        public void AddToQueue(string sourceFile, string targetFile, string sourceDir, string targetDir, DateTime sourceDate, DateTime targetDate, bool isNewFile)
        {
            string relativePath = Form1.GetRelativePath(sourceFile, sourceDir);

            // Проверяем, есть ли уже этот файл в очереди и у кого самая свежая дата
            if (_latestFiles.TryGetValue(relativePath, out var existingFile))
            {
                if (sourceDate > existingFile.Value) // Нашли более свежую версию
                {
                    _latestFiles[relativePath] = new KeyValuePair<string, DateTime>(sourceFile, sourceDate);
                }
                else
                {
                    sourceFile = existingFile.Key; // Оставляем старый sourceFile
                }
            }
            else
            {
                _latestFiles[relativePath] = new KeyValuePair<string, DateTime>(sourceFile, sourceDate);
            }

            // Проверяем, нет ли уже такой пары SourceFile → TargetFile
            if (!_queue.Any(q => q.SourceFile == sourceFile && q.TargetFile == targetFile))
            {
                _queue.Add(new Queue(true, sourceFile, targetFile, sourceDir, targetDir, sourceDate, targetDate, isNewFile));
            }
        }

        /// <summary>
        /// Возвращает готовую очередь файлов.
        /// </summary>
        public List<Queue> GetQueue() => _queue;
    }

}
