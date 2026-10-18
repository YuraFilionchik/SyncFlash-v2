namespace SyncFlash.Services
{
    public interface IFileSyncService
    {
        Task<List<Queue>> AnalyzeFilesAsync(Project project);
        Task SyncFilesAsync(List<Queue> queue, IProgress<string> progress, IProgress<int> progressbar, CancellationToken cancellationToken);
    }

    public class FileSyncService : IFileSyncService
    {
        private const double TimestampToleranceSeconds = 2;

        public Task<List<Queue>> AnalyzeFilesAsync(Project project)
        {
            return Task.Run(() => AnalyzeFiles(project));
        }

        /// <summary>
        /// Строит очередь детерминированно: сначала для каждого относительного пути
        /// выбирается самый свежий источник, затем создаются записи только для тех
        /// папок, где файл отсутствует или устарел.
        /// </summary>
        private static List<Queue> AnalyzeFiles(Project project)
        {
            var queueManager = new QueueManager();

            project.RefreshAvailability();

            var onlineDirs = project.AllProjectDirs
                .Where(x => (x.PC_Name == Environment.MachineName || x.PC_Name == CONSTS.FlashDrive) && x.IsOnline)
                .GroupBy(x => NormalizeDir(x.Dir), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();

            if (onlineDirs.Count <= 1) return queueManager.GetQueue();

            var filesByRelativePath = new Dictionary<string, List<FileEntry>>(StringComparer.OrdinalIgnoreCase);

            foreach (var dir in onlineDirs)
            {
                foreach (var pair in dir.AllFiles(useCache: false))
                {
                    var relativePath = Form1.GetRelativePath(pair.Key, dir.Dir);
                    if (string.IsNullOrEmpty(relativePath)) continue;

                    long length;
                    try
                    {
                        length = new FileInfo(pair.Key).Length;
                    }
                    catch (IOException)
                    {
                        continue;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }

                    if (!filesByRelativePath.TryGetValue(relativePath, out var entries))
                    {
                        filesByRelativePath[relativePath] = entries = new List<FileEntry>();
                    }

                    entries.Add(new FileEntry(NormalizeDir(dir.Dir), pair.Key, pair.Value, length));
                }
            }

            foreach (var (relativePath, entries) in filesByRelativePath)
            {
                if (entries.Count == 0) continue;

                var newest = entries
                    .OrderByDescending(e => e.LastWriteTimeUtc)
                    .ThenByDescending(e => e.Length)
                    .ThenBy(e => e.FullPath, StringComparer.OrdinalIgnoreCase)
                    .First();

                var byRoot = new Dictionary<string, FileEntry>(StringComparer.OrdinalIgnoreCase);
                foreach (var entry in entries)
                {
                    byRoot[entry.RootDir] = entry;
                }

                foreach (var target in onlineDirs)
                {
                    var targetRoot = NormalizeDir(target.Dir);
                    var targetFile = Path.Combine(targetRoot, relativePath.TrimStart('\\', '/'));

                    if (!byRoot.TryGetValue(targetRoot, out var targetEntry))
                    {
                        queueManager.Add(new Queue(
                            true,
                            newest.FullPath,
                            targetFile,
                            newest.RootDir,
                            targetRoot,
                            newest.LastWriteTime,
                            DateTime.MinValue,
                            true)
                        {
                            RelativePath = relativePath
                        });
                        continue;
                    }

                    if (string.Equals(targetEntry.FullPath, newest.FullPath, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var timeDelta = (newest.LastWriteTimeUtc - targetEntry.LastWriteTimeUtc).TotalSeconds;
                    var differs = Math.Abs(timeDelta) > TimestampToleranceSeconds || newest.Length != targetEntry.Length;

                    if (!differs) continue;

                    // Никогда не затираем более новый файл более старым.
                    if (newest.LastWriteTimeUtc < targetEntry.LastWriteTimeUtc) continue;

                    queueManager.Add(new Queue(
                        true,
                        newest.FullPath,
                        targetFile,
                        newest.RootDir,
                        targetRoot,
                        newest.LastWriteTime,
                        targetEntry.LastWriteTime,
                        false)
                    {
                        RelativePath = relativePath
                    });
                }
            }

            return queueManager.GetQueue();
        }

        public async Task SyncFilesAsync(List<Queue> queue, IProgress<string> progress, IProgress<int> progressbar, CancellationToken cancellationToken)
        {
            var activeFiles = queue?.Where(x => x.Active).ToList() ?? new List<Queue>();
            var errors = new List<string>();
            var copied = 0;
            var total = activeFiles.Count;

            progressbar.Report(0);

            for (var i = 0; i < activeFiles.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var file = activeFiles[i];

                progressbar.Report(total == 0 ? 0 : (int)(i * 100.0 / total));
                progress.Report($"Copying: {file.SourceFile} -> {file.TargetFile}");

                try
                {
                    await CopyFileAsync(file.SourceFile, file.TargetFile, cancellationToken).ConfigureAwait(false);
                    copied++;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    errors.Add($"{file.SourceFile}: {ex.Message}");
                    progress.Report($"Error copying {file.SourceFile}: {ex.Message}");
                }
            }

            progress.Report("End of synchronization");
            progressbar.Report(100);
            progress.Report("------------------------------");
            progress.Report($"Total files: {total}");
            progress.Report($"Copied files: {copied}");
            progress.Report($"Errors: {errors.Count} ==>");

            foreach (var error in errors)
            {
                progress.Report(error);
            }
        }

        private static async Task CopyFileAsync(string sourceFile, string targetFile, CancellationToken cancellationToken)
        {
            var targetDirectory = Path.GetDirectoryName(targetFile);
            if (!string.IsNullOrEmpty(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // Пишем во временный файл и заменяем целевой только после успешной записи,
            // чтобы при сбое не оставить повреждённую копию.
            var tempFile = Path.Combine(
                string.IsNullOrEmpty(targetDirectory) ? "." : targetDirectory,
                $".{Path.GetFileName(targetFile)}.{Guid.NewGuid():N}.tmp");

            try
            {
                const int bufferSize = 128 * 1024;

                await using (var source = new FileStream(
                    sourceFile,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize,
                    FileOptions.Asynchronous | FileOptions.SequentialScan))
                await using (var target = new FileStream(
                    tempFile,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize,
                    FileOptions.Asynchronous | FileOptions.SequentialScan))
                {
                    await source.CopyToAsync(target, bufferSize, cancellationToken).ConfigureAwait(false);
                    await target.FlushAsync(cancellationToken).ConfigureAwait(false);

                    // Для USB важно досылать данные на носитель, иначе при мгновенном извлечении они теряются.
                    await Task.Run(() => target.Flush(flushToDisk: true), cancellationToken).ConfigureAwait(false);
                }

                if (File.Exists(targetFile))
                {
                    var attributes = File.GetAttributes(targetFile);
                    if ((attributes & FileAttributes.ReadOnly) != 0)
                    {
                        File.SetAttributes(targetFile, attributes & ~FileAttributes.ReadOnly);
                    }
                }

                File.Move(tempFile, targetFile, overwrite: true);
                File.SetLastWriteTimeUtc(targetFile, File.GetLastWriteTimeUtc(sourceFile));
            }
            finally
            {
                try
                {
                    if (File.Exists(tempFile)) File.Delete(tempFile);
                }
                catch
                {
                    // Временный файл будет удалён при следующей синхронизации или вручную.
                }
            }
        }

        private static string NormalizeDir(string dir)
        {
            var full = Path.GetFullPath(dir);
            return full.Length <= 3 && full.EndsWith(Path.DirectorySeparatorChar)
                ? full
                : full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        private readonly record struct FileEntry(string RootDir, string FullPath, DateTime LastWriteTime, long Length)
        {
            public DateTime LastWriteTimeUtc => LastWriteTime.ToUniversalTime();
        }
    }

    public class QueueManager
    {
        private readonly List<Queue> _queue = new();
        private readonly HashSet<string> _keys = new(StringComparer.OrdinalIgnoreCase);
        private int _nextNumber = 1;

        /// <summary>
        /// Добавляет запись, если для этой пары «файл → целевая папка» её ещё нет.
        /// </summary>
        public void Add(Queue item)
        {
            var key = item.RelativePath + "|" + item.TargetFileProjectDir;
            if (!_keys.Add(key)) return;

            item.Number = _nextNumber++;
            _queue.Add(item);
        }

        public List<Queue> GetQueue() => _queue;
    }
}
