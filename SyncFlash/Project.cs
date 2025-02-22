using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using filesdates = System.Collections.Generic.Dictionary<string, System.DateTime>;

namespace SyncFlash
{
public    class Project : INotifyPropertyChanged
    {
        public List<Projdir> AllProjectDirs; //список всех папок для синхронизации
        private List<Projdir> onlineDirs = new List<Projdir>();
        public List<string> ExceptionDirs; //папки исключения
        private string _name;
        private DateTime? _lastSyncTime;
        private long _lastSyncSize;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public bool AutoSync { get; set; } 
        
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
                if (onlineDirs.Count != 0) return onlineDirs;
                else
                {
                    foreach (var dir in AllProjectDirs)
                    {
                        if (dir.IsOnline) onlineDirs.Add(dir);
                    }
                    return onlineDirs;
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            string syncTime = LastSyncTime?.ToString("yyyy-MM-dd HH:mm") ?? "Не синхронизирован";
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
           // timer.Start("removeing dir", 1);
            if (AllProjectDirs.Any(x => x.Dir == dir)) AllProjectDirs.Remove(GetProjDirFromString(dir));
           // timer.Stop(1);
        }

        //возвращает тип Project по названию папки
        private Projdir GetProjDirFromString(string dir)
        {
            return AllProjectDirs.First(x => x.Dir == dir);
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
 public   class Projdir
    {
        public string _dir;
        private Project FromProject;
        private string pc_name;
        private bool isOnline;
        private bool checkedOnline = false;
        /// <summary>
        /// Имя компъютера NetBios, на которой находится папка
        /// </summary>
        public string PC_Name
        {
            get
            {
                return pc_name;
            }
            set
            {
                pc_name = value;
            }
        }
        public string Dir
        {
            get
            {
                if (PC_Name == CONSTS.FlashDrive) //если флешка, то добавит букву диска
                    return (CONSTS.GetDriveLetter() + _dir);
                else return _dir;
            }
            set
            {
                if (CONSTS.FlashDrive == PC_Name)//если флешка
                {
                    var seg = value.Split('\\')[0];
                    _dir = value.Substring(seg.Length);//сохраняем без буквы диска
                }
                else _dir = value;
            }
        }
        public bool IsOnline
        {
            get
            {
                //tmr.Start("Checking isOnline: "+Dir, 2);
                if (checkedOnline) return isOnline;
                else
                {
                    isOnline =  Directory.Exists(Dir);
                    checkedOnline = true;
                }
                return isOnline;
               // tmr.Stop(2);
            }
        }
        private filesdates _allfiles;

        private DateTime DefaultDate = new DateTime(2000, 1, 1);

        public Projdir(string dir, Project project)
        {
            Dir = dir;
            pc_name = System.Environment.MachineName;
            FromProject = project;
        }
        public Projdir(string dir, Project project, string pc)
        {
            Dir = dir;
            pc_name = pc;
            FromProject = project;
        }

        /// <summary>
        /// Поиск файла и даты в списке всех файлов,
        /// </summary>
        /// <param name="relativeFilePath"></param>
        /// <returns><key = PathToFile, value = LastModified></returns>
        public KeyValuePair<string, DateTime> FindFile(string relativeFilePath)
        {
            string ABSpath = Dir + relativeFilePath;
            foreach (var file in AllFiles())
            {
                if (file.Key == ABSpath)
                {
                    return file;
                }
            }
            return new KeyValuePair<string, DateTime>();
        }
        /// <summary>
        /// Dictionary<files,datetime last write> всех файлов в Projdir
        /// </summary>
        public filesdates AllFiles(bool useCache=true)
        {
            if (_allfiles != null && useCache) return _allfiles; // Используем кэш, если данные уже загружены

            var res = new filesdates();
            if (!IsOnline) return res; // Если папка оффлайн, возвращаем пустой список

            try
            {
                // Храним исключенные пути как относительные
                var exceptionPaths = new HashSet<string>(
                    FromProject.ExceptionDirs.Where(e => !e.Contains("*")),
                    StringComparer.OrdinalIgnoreCase
                );

                // Отдельно храним шаблоны (*.bak, *.tmp и т. д.)
                var exceptionPatterns = FromProject.ExceptionDirs.Where(e => e.Contains("*")).ToList();

                foreach (var file in Directory.EnumerateFiles(Dir, "*", SearchOption.AllDirectories))
                {
                    // Получаем относительный путь файла внутри папки проекта
                    string relativePath = Form1.GetRelativePath(file, Dir);

                    // Проверяем, не находится ли файл в исключениях (сравниваем относительный путь)
                    if (exceptionPaths.Any(x=>relativePath.Contains(x)))
                        continue;

                    // Проверяем, подпадает ли файл под шаблон (*.bak, *.tmp)
                    if (exceptionPatterns.Any(pattern => MatchesPattern(relativePath, pattern)))
                        continue;

                    res[file] = File.GetLastWriteTime(file);
                }
            }
            catch (Exception) { return res; } // Обрабатываем возможные ошибки доступа к файлам

            _allfiles = res; // Кэшируем результат
            return res;

        }

        private bool MatchesPattern(string filePath, string pattern)
        {
            string fileName = Path.GetFileName(filePath);
            return Regex.IsMatch(fileName, "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$", RegexOptions.IgnoreCase);
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
