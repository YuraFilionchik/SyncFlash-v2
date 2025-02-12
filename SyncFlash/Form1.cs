using SyncFlash.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SyncFlash
{
    public partial class Form1 : Form
    {
        public string DriveLette;
        public const string cfg_file = "conf.ini";
        string pc_name = Environment.MachineName;
        BindingList<Project> Projects = new BindingList<Project>();
        private bool IsRunningSync=false; // if sync is running
        private readonly IFileSyncService _fileSyncService;
        private readonly IConfigService _configService;
        private readonly IProgress<string> _progress;
        private CancellationTokenSource _syncCancellationTokenSource;

        Thread SyncThread; 
        Thread CopyDIRSThread; //процесс принудительного копирования папки
        MyTimer tmr;
        public static LogForm log;
        public Form1()
        { //TODO SYNC SOME PROJECTS 
            InitializeComponent();
            _fileSyncService = new FileSyncService();
            _configService = new ConfigService(cfg_file);
            _progress = new Progress<string>(message => CONSTS.AddNewLine(tblog, message));
            Projects = _configService.GetProjects();
            log = new LogForm();
            tmr = new MyTimer(log);
            List_Projects.DataSource = Projects;
            DriveLette = CONSTS.GetDriveLetter();
            button1.Text = CONSTS.btSyncText1;
            this.FormClosing += Form1_FormClosing;
            List_Projects.SelectedIndexChanged += List_Projects_SelectedIndexChanged;
            list_dirs.SelectedIndexChanged += List_dirs_SelectedIndexChanged;
            list_dirs.DoubleClick += List_dirs_DoubleClick;
            checkBox1.CheckedChanged += CheckBox1_CheckedChanged;
            _progress.Report("USB-Flash: " + DriveLette);
            button1.Click += button1_Click;

        }



        #region Events Handlers
        /// <summary>
        /// открытие папки по двойному клику
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void List_dirs_DoubleClick(object sender, EventArgs e)
        {
            string selectedDir = "";
            try
            {
                if (list_dirs.SelectedItems.Count == 0) return;
                selectedDir = list_dirs.SelectedItems[0].Text;
                if (Directory.Exists(selectedDir))
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();

                    process.StartInfo.UseShellExecute = true;

                    process.StartInfo.FileName = @"explorer";

                    process.StartInfo.Arguments = selectedDir;

                    process.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Opening folder " + selectedDir);
            }
        }
        //selected DIR
        private void List_dirs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (List_Projects.SelectedItem == null)
            { list_dirs.Items.Clear(); listExceptions.Items.Clear(); return; } //clear lists
            if (list_dirs.SelectedItems.Count == 0)
            {
                listExceptions.Items.Clear(); //clear list
                return;
            }
            var selectedProj = GetSelectedProject();
            var selectedDir = selectedProj.AllProjectDirs.FirstOrDefault(x => x.Dir == list_dirs.SelectedItems[0].Text);
            //show Dir info

            if (list_dirs.SelectedItems.Count == 1)
            {
                tblog.Text = selectedProj.Name + Environment.NewLine;
                if (selectedDir != null && selectedDir.Info1() != null)
                    foreach (var str in selectedDir.Info1())
                    {
                        tblog.Text += str + Environment.NewLine;
                    }
            }
            if (list_dirs.SelectedItems.Count == 2)
            {

            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            DisplayDirs();
        }


        //select project
        private void List_Projects_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayDirs();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            DisplayDirs();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            // Если уже идет синхронизация – отменяем её
            if (_syncCancellationTokenSource != null)
            {
                _syncCancellationTokenSource.Cancel();
                _syncCancellationTokenSource = null;
                button1.Text = CONSTS.btSyncText1; // Возвращаем кнопку в исходное состояние
                return;
            }

            var selectedProject = GetSelectedProject();
            if (selectedProject == null) return;

            // Меняем текст кнопки на "Остановить синхронизацию"
            button1.Text = CONSTS.btSyncText2;

            _syncCancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _syncCancellationTokenSource.Token;

            try
            {
                // Этап 1: Анализ файлов
                var selectedFiles = await AnalyzeProjectAsync(selectedProject);
                if (selectedFiles == null)
                {
                    button1.Text = CONSTS.btSyncText1; // Вернуть текст кнопки, если анализ не дал результатов
                    return;
                }

                // Этап 2: Синхронизация файлов
                await SyncSelectedFilesAsync(selectedProject, selectedFiles, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Синхронизация прервана пользователем.");
            }
            finally
            {
                button1.Text = CONSTS.btSyncText1; // Возвращаем кнопку в исходное состояние
                _syncCancellationTokenSource = null;
            }

        }

        private async Task<List<Queue>> AnalyzeProjectAsync(Project project)
        {
            var queue = await _fileSyncService.AnalyzeFilesAsync(project);

            if (queue.Count == 0)
            {
                MessageBox.Show("Все папки одинаковые, нечего синхронизировать.");
                return null;
            }

            // Открываем окно для выбора файлов пользователем
            using (var msgBox = new MsgDialog(project, queue))
            {
                var result = msgBox.ShowDialog();
                SaveAllProjects();
                if (result != DialogResult.OK) return null;
            }

            return queue; // Возвращаем отфильтрованный пользователем список файлов
        }

       
        private async Task SyncSelectedFilesAsync(Project project, List<Queue> selectedFiles, CancellationToken cancellationToken)
        {
            if (selectedFiles == null || selectedFiles.Count == 0)
            {
                MessageBox.Show("Не выбраны файлы для синхронизации.");
                return;
            }

            await _fileSyncService.SyncFilesAsync(selectedFiles, _progress, cancellationToken);

            // Обновляем данные проекта после успешной синхронизации
            project.LastSyncTime = DateTime.Now;
            project.LastSyncSize = GetProjectSize(project);

            SaveAllProjects();
        }



        private long GetProjectSize(Project project)
        {
            long totalSize = 0;
            foreach (var dir in project.AllProjectDirs)
            {
                if (!Directory.Exists(dir.Dir)) continue;
                totalSize += Directory.GetFiles(dir.Dir, "*", SearchOption.AllDirectories)
                                      .Sum(f => new FileInfo(f).Length);
            }
            return totalSize;
        }

        private Project GetSelectedProject()
        {
            if (List_Projects.SelectedItem == null) return null;
            if (List_Projects.SelectedItem is Project selectedProject)
            {
                return selectedProject;
            }
            return null;

        }

        private void btSelectUSB_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.Description = "Выберите любую папку на USB/Select any folder on USB.";
            var dr = fd.ShowDialog();
            if (dr != DialogResult.OK) return;
            if (!Directory.Exists(fd.SelectedPath)) return;
            string newLette = fd.SelectedPath.Split('\\').First();
            DriveLette = newLette;
            CONSTS.AddNewLine(tblog, "New USB letter: " + DriveLette);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (List_Projects.SelectedItems.Count != 1) return;
            var selectedProj = GetSelectedProject();
            if (selectedProj != null) selectedProj.AutoSync = checkBox2.Checked;
            SaveAllProjects();
        }
        #endregion


       
        
        /// <summary>
        /// Выделяет одинаковую часть пусти для файлов одного проекта
        /// </summary>
        /// <param name="fullpath"></param>
        /// <param name="projDir"></param>
        /// <returns></returns>
        public static string GetRelationPath(string fullpath, string projDir)
        {
            string result = fullpath.Remove(0, projDir.Length);
            return result;
        }
        /// <summary>
        /// Write dirs of selected project into list_dirs
        /// </summary>
        public void DisplayDirs()
        {
            list_dirs.Items.Clear();
            listExceptions.Items.Clear();
            if (List_Projects.SelectedItem == null)
            { return; }

            var selectedProj = GetSelectedProject();
            checkBox2.Checked = selectedProj.AutoSync;
            foreach (var item in selectedProj.AllProjectDirs)
            {
                if (!checkBox1.Checked || item.IsOnline && (item.PC_Name == pc_name || item.PC_Name == CONSTS.FlashDrive))
                {
                    list_dirs.Items.Add(item.Dir);
                }

            }
            listExceptions.Items.AddRange(selectedProj.ExceptionDirs.ToArray());
        }
        //Завершение программы
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveAllProjects();
        }

        private void SaveAllProjects()
        {
           if (!_configService.SaveProjects(Projects)) _progress.Report("Error saving list of projects");

        }

        #region contextMenu
        private void добавитьПроектToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new ProjectNameDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string projectName = dialog.ProjectName;

                    if (Projects.Any(x=>x.Name == projectName))
                    {
                        MessageBox.Show("Такой проект уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var project = new Project(projectName);
                    Projects.Add(project);
                    SaveAllProjects();
                }
            }
        }

        private void переименоватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (List_Projects.SelectedItems.Count != 1) return;
            var selectedProj = GetSelectedProject();
            var inputDialog = new ProjectNameDialog(selectedProj.Name);
            var dr = inputDialog.ShowDialog();
            if (dr != DialogResult.OK) return;
            var newName = inputDialog.ProjectName;
            if (Projects.Any(c=>c.Name == newName))
            {
                MessageBox.Show("Такое имя уже есть в списке.");
                return;
            }
            selectedProj.Name = newName;
            SaveAllProjects();

        }
        private void синхронизироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SyncSelectedProjects();
        }
        private void добавитьПапкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Input input = new Input();
            input.ShowDialog();
            var selected = List_Projects.SelectedItem;
            string inputDir = input.TEXT.TrimEnd('\\');
            if (string.IsNullOrWhiteSpace(input.TEXT) ||
                list_dirs.Items.Contains(new ListViewItem(inputDir)) ||
                selected == null) return;

            list_dirs.Items.Add(inputDir);
            var p = GetSelectedProject();
            var lette = input.TEXT.Split('\\')[0];
            if (lette == DriveLette)
            {//removable disk
                string dir = input.TEXT.TrimEnd('\\').Substring(lette.Length);
                p.AllProjectDirs.Add(new Projdir(dir, p, CONSTS.FlashDrive));
            }
            else //HDD disk
            {
                p.AllProjectDirs.Add(new Projdir(inputDir, p));
            }
            p.OnlineDirs.Clear();
           SaveAllProjects();
        }

        private void удалитьПапкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = List_Projects.SelectedItem;
            if (selected == null) return;
            var selectedDirs = list_dirs.SelectedItems;
            if (selectedDirs.Count == 0) return;
            string DirRemove = selectedDirs[0].Text;
            //if (DriveLette == DirRemove.Split('\\')[0])//FlashDrive
            //{
            //    DirRemove = GetRelationPath(DirRemove, DriveLette);
            //}
            var proj = GetSelectedProject();
            if (proj!=null)proj.RemoveDir(DirRemove);
            list_dirs.Items.Remove(selectedDirs[0]);
            proj.OnlineDirs.Clear();
            SaveAllProjects();
        }

        private void удалитьПроектToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = List_Projects.SelectedItem;
            if (selected == null) return;
            Projects.Remove(GetSelectedProject());
            SaveAllProjects();
        }

        //Add Except Dir
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var selected = List_Projects.SelectedItem;//selected project in List
            if (selected == null) return;
            string selectedDir; //Selected directory into project
            if (list_dirs.SelectedItems.Count == 0)
            {
                if (list_dirs.Items.Count != 0)
                    selectedDir = list_dirs.Items[0].Text;
                else
                    selectedDir = Environment.CurrentDirectory;
            }
            else
            {
                selectedDir = list_dirs.SelectedItems[0].Text;
            }
    
            Input input = new Input();
            input.TEXT = selectedDir;
            var dg = input.ShowDialog();
            if (dg != DialogResult.OK) return;
            if (string.IsNullOrWhiteSpace(input.TEXT) ||
                listExceptions.Items.Contains(input.TEXT)) return;
            var pr = GetSelectedProject();

            string relpath = input.TEXT.TrimEnd('\\').Contains(":\\") ?
                GetRelationPath(input.TEXT.TrimEnd('\\'), selectedDir) : input.TEXT.TrimEnd('\\');
            if (pr.ExceptionDirs.Contains(relpath)) return;
            pr.ExceptionDirs.Add(relpath);//добавление относительного пути
            listExceptions.Items.Clear();
            listExceptions.Items.AddRange(pr.ExceptionDirs.ToArray());
            SaveAllProjects();
        }

        //remove Except dir
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var selected = List_Projects.SelectedItem;
            if (selected == null) return;
            var selectedDir = list_dirs.SelectedItems;
            if (selectedDir.Count == 0)
            {
                CONSTS.AddNewLine(tblog, "Для добавления исключений" +
                    " нужно выбрать одну из папок проекта, в котором будете указывать исключения");
                return;
            }
            var selectedExc = listExceptions.SelectedItems;
            if (selectedExc.Count == 0) return;
            var pr = GetSelectedProject();
            string selExc = selectedExc[0].ToString();
            if (!pr.ExceptionDirs.Contains(selExc))
                return;
            pr.ExceptionDirs.Remove(selExc);
            listExceptions.Items.Clear();
            listExceptions.Items.AddRange(pr.ExceptionDirs.ToArray());
         //   SaveAllProjects();
        }
        #endregion


        public void SetSyncStatus(bool status)
        {
            IsRunningSync = status;
        }
        /// <summary>
        /// Запускает процесс синхронизации выделенных проектов
        /// </summary>
        private void SyncSelectedProjects()
        {
            //tmr.Start("Подготовка синхронизации");
            //var selected = List_Projects.SelectedItem;
            //if (selected == null) return;
            //var P = GetSelectedProject();
            //tblog.Rows.Clear();
            //tmr.Stop();
            //StartSync(P, cbSilent.Checked);
        }
        //TODO Menu/edit dirs

        /// <summary>
        /// Run Sync projects with attribute AutoSync=True
        /// </summary>
        public void StartAutoSync()
        {
            //Thread autoSyncThread = new Thread(delegate ()
            //  {
            //      var projAuto = Projects.Where(x => x.AutoSync);//All autosync projects
            //      if (projAuto.Count() == 0)
            //      {
            //          CONSTS.AddNewLine(tblog, "Nothing for autosync");
            //          return;
            //      }
            //      CONSTS.AddNewLine(tblog, "For AutoSync " + projAuto.Count() + " projects");
            //      foreach (var project in projAuto)
            //      {
            //          if (SyncThread != null)
            //              while (IsRunningSync || SyncThread.IsAlive)
            //              {
            //                  Thread.Sleep(1000);
            //              }
            //          StartSync(project, true);

            //      }
            //  });
            //autoSyncThread.Start();

        }

        private void btLog_Click(object sender, EventArgs e)
        {
            if (log.IsDisposed) log = new LogForm();
            if (log.Visible) log.Visible = false;
            else log.Show();
        }

        //add text info to LogForm
        public void Addlog(string text)
        {
            log.AddLine(text);
        }
        public void ClearLog()
        {
            log.ClearLog();
        }

        /// <summary>
        /// Копировать выделенную папку в другие без проверки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void копироватьЭтуПапкуВОстальныеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var selected = List_Projects.SelectedItem;
                if (selected == null) return;
                var selectedDir = list_dirs.SelectedItems;
                if (selectedDir.Count == 0) return;
                string SelectedDirPATH = selectedDir[0].Text;
                if (DriveLette == SelectedDirPATH.Split('\\')[0])//FlashDrive
                {
                    SelectedDirPATH = GetRelationPath(SelectedDirPATH, DriveLette);
                }
                var Project = GetSelectedProject();
                CopyDIRSThread = new Thread(delegate ()
                {
                    CONSTS.invokeProgress(progressBar1, 0);
                    CONSTS.DisableButton(button1); // кнопку старт в активный режим
                    CONSTS.AddNewLine(tblog, "Проект " + Project.Name + ". Принудительное копирование " + SelectedDirPATH);
                    foreach (Projdir onlineDir in Project.OnlineDirs)
                    {
                        if (onlineDir.Dir == SelectedDirPATH) continue;
                        //copy selected Dir into others
                        DirectoryCopy(SelectedDirPATH, onlineDir.Dir, true);
                        CONSTS.AddNewLine(tblog, SelectedDirPATH + " } ---> {" + onlineDir.Dir);
                    }
                    CONSTS.AddNewLine(tblog, $"\t\tГотово. Выбранная папка скопирована в {(Project.OnlineDirs.Count()-1)} другие папки."); ;
                    CONSTS.EnableButton(button1); // кнопку старт в обычный режим
                });
                CopyDIRSThread.Start();
            }
            catch ( Exception ex)
            {
                MessageBox.Show(ex.Message, "ForceCopy");
                CONSTS.AddNewLine(tblog, "\t\tПроцесс копирования прерван.");
            }
               
            

        }
        /// <summary>
        /// Копирование директории
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            { 
                string temppath = Path.Combine(destDirName, file.Name);
                try { file.CopyTo(temppath, true); }
                catch (Exception) { continue; }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private void CheckBox1_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
