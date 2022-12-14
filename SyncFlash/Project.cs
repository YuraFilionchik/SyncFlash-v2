using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using filesdates = System.Collections.Generic.Dictionary<string, System.DateTime>;

namespace SyncFlash
{
public    class Project
    {
        private MyTimer timer = new MyTimer(Form1.log);
        public List<Projdir> AllProjectDirs; //список всех папок для синхронизации
        private List<Projdir> onlineDirs = new List<Projdir>();
        public List<string> ExceptionDirs; //папки исключения
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

        public bool AutoSync;
        public string Name;
        public override string ToString()
        {
            return Name;
        }
        public Project(string name)
        {
            this.Name = name;
            AllProjectDirs = new List<Projdir>();
            ExceptionDirs = new List<string>();
            AutoSync = false;
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
        MyTimer tmr;
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
            tmr = new MyTimer(Form1.log);
        }
        public Projdir(string dir, Project project, string pc)
        {
            Dir = dir;
            pc_name = pc;
            FromProject = project;
            tmr = new MyTimer(Form1.log);
        }

        /// <summary>
        /// Поиск файла и даты в списке всех файлов
        /// </summary>
        /// <param name="relateFilePath"></param>
        /// <returns></returns>
        public KeyValuePair<string, DateTime> FindFile(string relateFilePath)
        {
            string ABSpath = Dir + relateFilePath;
            foreach (var file in AllFiles())
            {
               
                // if (Form1.GetRelationPath(file.Key, Dir) == relateFilePath)
                if (file.Key == ABSpath)
                {
                    
                   // tmr.AddLine("Find file " + ABSpath);
                    return file;
                }
               

            }
            return new KeyValuePair<string, DateTime>();
        }
        /// <summary>
        /// Dictionary<files,datetime last write> всех файлов в Projdir
        /// </summary>
        public filesdates AllFiles()
        {
            filesdates res = new filesdates();
            if (!IsOnline)  return res; 
            string[] AllFiles;
            if (_allfiles == null)
            {
                tmr.Start("===GetFilesInDir = " + Dir, 22);
               // CONSTS.AddToTempLine("Получение файлов из " + Dir);
                AllFiles = GetfilesIndir(Dir); //запуск поиска всех файлов директории проекта
                tmr.Stop(22);
            }
            else return _allfiles;
            var n = AllFiles.Count();
            //tmr.Stop(22);
            // tmr.Start("Перевод в структуру filesdates ",23);
            foreach (var file in AllFiles)
            {
                res.Add(file, File.GetLastWriteTime(file));
            }
            // tmr.Stop(23);
            _allfiles = res;
            return res;

        }
        public void ReadFiles()
        {

            filesdates res = new filesdates();

            string[] AllFiles;

            AllFiles = GetfilesIndir(Dir); //запуск поиска всех файлов директории проекта

            foreach (var file in AllFiles)
            {
                res.Add(file, File.GetLastWriteTime(file));
            }

            _allfiles = res;
        }
        private string[] GetfilesIndir(string dir)
        {
            
            string relativeDir = dir.Contains(":\\") ? Form1.GetRelationPath(dir, this.Dir) : dir;//относительный путь
            var result = new string[0];
            if (FromProject.ExceptionDirs.Contains(relativeDir))//filter by ExceptionDirs
            {  return result; }
            if (Directory.GetDirectories(dir).Count() == 0)
            { return Directory.GetFiles(dir); }//file in root dir

            else
            {
                result = result.Concat(Directory.GetFiles(dir)).ToArray();
                foreach (var D in Directory.GetDirectories(dir))
                {
                   
                    result = result.Concat(GetfilesIndir(D)).ToArray();
                    
                }
            }
           
            return result;
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

        /// <summary>
        /// Сравнение с другой папкой проекта пофайлово
        /// </summary>
        /// <param name="Dir2"></param>
        /// <returns>[0]-кол-во файлов новее чем в Dir2, [1]-старше чем в Dir2,
        /// [2]-новых файлов, [3]-отсутсвующих файлов по сравнению с Dir2</returns>
        public int[] CompairDir(Projdir Dir2)
        {
            int[] res = new int[2];
            var files2 = Dir2.AllFiles();
            //key - filepath
            //value - datetime
            var all = AllFiles();
            foreach (var filedate in all)
            {
                var n0 = all.Count(x => files2.Any(c => c.Key == x.Key && c.Value < x.Value));
                var n1 = all.Count(x => files2.Any(c => c.Key == x.Key && c.Value > x.Value));
                var n2 = all.Count(x => !files2.Any(c => c.Key == x.Key));
                var n3 = files2.Count(x => !all.Any(c => c.Key == x.Key));
            }

            return res;
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

    //class Queue
    //{
    //    Dictionary<string, string> Files;
    //    public Queue()
    //    {
    //        Files = new Dictionary<string, string>();
    //    }

    //    public void AddFile(string SourceFilePath, string DestFilePath)
    //    {
    //        Files.Add(SourceFilePath, DestFilePath);
    //    }

    //}
}
