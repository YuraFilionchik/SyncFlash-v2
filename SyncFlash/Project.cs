using System.ComponentModel;
using System.IO.Enumeration;
using System.Xml.Linq;
using FilesDates = System.Collections.Generic.Dictionary<string, System.DateTime>;

namespace SyncFlash
{
public    class Project : INotifyPropertyChanged
    {
        public List<Projdir> AllProjectDirs; //список всех папок для синхронизации
        private readonly List<Projdir> onlineDirs = new();
        public List<string> ExceptionDirs; //папки исключения
        private string _name = string.Empty;
        private DateTime? _lastSyncTime;
        private long _lastSyncSize;
        private bool _autoSync;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public bool AutoSync
        {
            get => _autoSync;
            set
            {
                if (_autoSync == value) return;
                _autoSync = value;
                OnPropertyChanged(nameof(AutoSync));
            }
        }

        // Последняя успешная синхронизация
        public DateTime? LastSyncTime { 
            get => _lastSyncTime;
            set 
            {
                _lastSyncTime = value;
                OnPropertyChanged(nameof(LastSyncTime));
            } }
        public long LastSyncSize { 
            get => _lastSyncSize;
            set
            {
                _lastSyncSize = value;
                OnPropertyChanged(nameof(LastSyncSize));
            } } // Размер проекта в байтах при последней синхронизации
        public List<Projdir> OnlineDirs//доступные сейчас
        {
            get
            {
                if (onlineDirs.Count == 0)
                {
                    foreach (var dir in AllProjectDirs)
                    {
                        if (dir.IsOnline && !onlineDirs.Contains(dir))
                        {
                            onlineDirs.Add(dir);
                        }
                    }
                }

                return onlineDirs;
            }
        }

        /// <summary>
        /// Сбрасывает кеш доступности папок и списка файлов.
        /// </summary>
        public void InvalidateCaches()
        {
            onlineDirs.Clear();
            foreach (var dir in AllProjectDirs)
            {
                dir.InvalidateCaches();
            }
        }

        /// <summary>
        /// Перепроверяет, какие папки проекта сейчас доступны.
        /// </summary>
        public void RefreshAvailability() => InvalidateCaches();

        public void AddDir(Projdir dir)
        {
            AllProjectDirs.Add(dir);
            InvalidateCaches();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            string syncTime = LastSyncTime?.ToString("yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture)
                ?? "Не синхронизирован";
            string size = FormatSize(LastSyncSize);
            return $"{Name} | {syncTime} | {size}";
        }

        private string FormatSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F2} kB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024):F2} MB";
            return $"{bytes / (1024.0 * 1024 * 1024):F2} GB";
        }

        public List<string> Alldirs
        {
            get
            {
                var list = new List<string>();
                foreach (var pd in AllProjectDirs)
                {
                    list.Add(pd.Dir);
                }
                return list;
            }
        }

       
        public Project(string name)
        {
            this.Name = name;
            AllProjectDirs = new List<Projdir>();
            ExceptionDirs = new List<string>();
            AutoSync = false;
        }

        public List<string> Info()
        {
            var res = new List<string>();
            res.Add("Project: " + Name);
            res.Add("AutoSync: " + AutoSync);
            res.Add("Last sync time: " + LastSyncTime);
            res.Add("LastSyncSize: " + FormatSize(LastSyncSize));
            return res;
        }
        public void RemoveDir(string dir)
        {
            if (AllProjectDirs.RemoveAll(x => string.Equals(x.Dir, dir, StringComparison.OrdinalIgnoreCase)) > 0)
            {
                InvalidateCaches();
            }
        }
        /// <summary>
        /// <Project name = Name>
        ///     <directory> dirpath</directory>
        ///     ...
        ///     <directory> dirpath</directory>
        /// </Project>
        /// </summary>
        /// <returns></returns>
        public XElement ToXElement()
        {
            var xel = new XElement(CONSTS.ProjXML);
            xel.SetAttributeValue("name", Name);
            xel.SetAttributeValue(CONSTS.AutoSync, this.AutoSync.ToString());
            foreach (var dir in AllProjectDirs)
            {
                XElement xdir = new XElement(CONSTS.DirXML);
                xdir.SetAttributeValue(CONSTS.PC_XML, dir.PC_Name);//имя компа в атрибутах
                xdir.SetAttributeValue(CONSTS.AttDirName, dir._dir);//путь к папке в атрибутах
                xel.Add(xdir);
            }
            //папки исключения
            foreach (var exc in ExceptionDirs)
            {
                XElement xdir = new XElement(CONSTS.ExceptXML);
                xdir.SetAttributeValue(CONSTS.AttDirName, exc);//Папка исключения в атрибутах
                xel.Add(xdir);
            }

            return xel;
        }
    }
    /// <summary>
    /// Класс представляющий одну папку для данного проекта
    /// </summary>
    public class Projdir
    {
        public string _dir = string.Empty;
        private readonly Project? FromProject;
        private string pc_name;
        private bool isOnline;
        private bool checkedOnline;
        private DateTime onlineCheckedAtUtc;
        private FilesDates? _allfiles;

        private static readonly TimeSpan OnlineCacheTtl = TimeSpan.FromSeconds(2);
        private static readonly DateTime DefaultDate = new(2000, 1, 1);
        private static readonly string[] IgnoredFileNames = { "Thumbs.db", "desktop.ini" };

        /// <summary>
        /// Имя компъютера NetBios, на которой находится папка
        /// </summary>
        public string PC_Name
        {
            get => pc_name;
            set => pc_name = value;
        }

        public string Dir
        {
            get => PC_Name == CONSTS.FlashDrive
                ? CONSTS.CombineWithDriveLetter(_dir)
                : _dir;
            set
            {
                if (CONSTS.FlashDrive == PC_Name)//если флешка
                {
                    var root = Path.GetPathRoot(value) ?? string.Empty;
                    _dir = value.Length > root.Length ? value.Substring(root.Length) : string.Empty;

                    if (!string.IsNullOrEmpty(_dir) &&
                        _dir[0] != Path.DirectorySeparatorChar &&
                        _dir[0] != Path.AltDirectorySeparatorChar)
                    {
                        _dir = Path.DirectorySeparatorChar + _dir;
                    }
                }
                else
                {
                    _dir = value;
                }
            }
        }

        public bool IsOnline
        {
            get
            {
                var now = DateTime.UtcNow;
                if (checkedOnline && (now - onlineCheckedAtUtc) < OnlineCacheTtl)
                {
                    return isOnline;
                }

                if (PC_Name == CONSTS.FlashDrive && string.IsNullOrWhiteSpace(CONSTS.GetDriveLetter()))
                {
                    isOnline = false;
                    checkedOnline = true;
                    onlineCheckedAtUtc = now;
                    return false;
                }

                isOnline = Directory.Exists(Dir);
                checkedOnline = true;
                onlineCheckedAtUtc = now;
                return isOnline;
            }
        }

        public Projdir(string dir, Project project)
            : this(dir, project, Environment.MachineName)
        {
        }

        public Projdir(string dir, Project project, string pc)
        {
            pc_name = pc;
            FromProject = project;
            Dir = dir;
        }

        /// <summary>
        /// Сбрасывает кеш доступности и список файлов папки.
        /// </summary>
        public void InvalidateCaches()
        {
            checkedOnline = false;
            isOnline = false;
            _allfiles = null;
        }

        /// <summary>
        /// Поиск файла и даты в списке всех файлов.
        /// </summary>
        public KeyValuePair<string, DateTime> FindFile(string relativeFilePath)
        {
            var baseDir = Dir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var absolutePath = relativeFilePath.StartsWith('\\') || relativeFilePath.StartsWith('/')
                ? baseDir + relativeFilePath
                : baseDir + Path.DirectorySeparatorChar + relativeFilePath;

            return AllFiles().TryGetValue(absolutePath, out var lastWrite)
                ? new KeyValuePair<string, DateTime>(absolutePath, lastWrite)
                : default;
        }

        /// <summary>
        /// Dictionary&lt;file, last write time&gt; всех файлов в Projdir.
        /// </summary>
        public FilesDates AllFiles(bool useCache = true)
        {
            if (_allfiles is not null && useCache) return _allfiles;

            var result = new FilesDates(StringComparer.OrdinalIgnoreCase);
            if (!IsOnline) return result;

            var exceptionPaths = new List<string>();
            var exceptionPatterns = new List<string>();

            foreach (var exception in FromProject?.ExceptionDirs ?? new List<string>())
            {
                if (string.IsNullOrWhiteSpace(exception)) continue;

                if (exception.Contains('*') || exception.Contains('?'))
                {
                    exceptionPatterns.Add(exception.Trim());
                }
                else
                {
                    exceptionPaths.Add(NormalizeRelative(exception));
                }
            }

            var options = new EnumerationOptions
            {
                RecurseSubdirectories = true,
                IgnoreInaccessible = true,
                AttributesToSkip = FileAttributes.System | FileAttributes.ReparsePoint,
                MatchType = MatchType.Simple
            };

            try
            {
                foreach (var file in Directory.EnumerateFiles(Dir, "*", options))
                {
                    var relativePath = Form1.GetRelativePath(file, Dir);
                    if (IsExcluded(relativePath, exceptionPaths, exceptionPatterns)) continue;

                    try
                    {
                        result[file] = File.GetLastWriteTime(file);
                    }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }

            _allfiles = result;
            return result;
        }

        private static string NormalizeRelative(string path) =>
            path.Replace('/', '\\').Trim('\\');

        private static bool IsExcluded(string relativePath, List<string> exceptionPaths, List<string> exceptionPatterns)
        {
            var normalized = NormalizeRelative(relativePath);
            var fileName = Path.GetFileName(normalized);

            if (IgnoredFileNames.Any(x => fileName.Equals(x, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            foreach (var path in exceptionPaths)
            {
                if (normalized.Equals(path, StringComparison.OrdinalIgnoreCase) ||
                    normalized.StartsWith(path + "\\", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            foreach (var pattern in exceptionPatterns)
            {
                if (FileSystemName.MatchesSimpleExpression(pattern, fileName, ignoreCase: true) ||
                    FileSystemName.MatchesSimpleExpression(pattern, normalized, ignoreCase: true))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Показывает время подификации самого нового файла в папке Dir и одной подпапке внутрь
        /// </summary>
        public DateTime LastMod
        {
            get
            {
                DateTime lastmod = DefaultDate; ;
                if (!IsOnline) return lastmod;
                var files1 = AllFiles();
                if (files1.Count == 0) return lastmod;//no one file in Dir and subdirs
                return files1.Max(x => x.Value);
            }
        }


        public override string ToString()
        {
            return Dir;
        }

        /// <summary>
        /// Предоставляет информацию о папке
        /// </summary>
        /// <returns></returns>
        public List<string> Info2()
        {
            var res = new List<string>();

            if (IsOnline) res.Add("ONLINE == " + Dir); else res.Add("OFFLINE == " + Dir);
            if (LastMod == DefaultDate) res.Add("Dir Last Modif.: недоступно");
            else res.Add("Самый свежий файл: " + LastMod.ToString("dd.MM.yyyy HH:mm:ss"));
            res.Add("Файлов \t:" + AllFiles().Count);
            return res;
        }

        public List<string> Info1()
        {
            var res = new List<string>();

            if (IsOnline) res.Add("ONLINE == " + Dir); else res.Add("OFFLINE == " + Dir);
            // if (LastMod == DefaultDate) res.Add("Dir Last Modif.: недоступно");
            //else res.Add("Dir Last Modif.: " + LastMod.ToString("dd.MM.yyyy HH:mm:ss"));
            //res.Add("Файлов \t:" + AllFiles.Count);
            return res;
        }
    }
}
