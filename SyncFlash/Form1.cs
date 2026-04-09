using SyncFlash.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace SyncFlash
{
    public partial class Form1 : Form
    {
        public string DriveLetter;
        public const string cfg_file = "conf.ini";
        string pc_name = Environment.MachineName;
        BindingList<Project> Projects = new BindingList<Project>();
        private bool IsRunningSync = false; // if sync is running
        private readonly IFileSyncService _fileSyncService;
        private readonly IConfigService _configService;
        private readonly IProgress<string> _progress;
        private readonly IProgress<int> _progressBar;

        private CancellationTokenSource _syncCancellationTokenSource;

        Thread SyncThread;
        Thread CopyDIRSThread; //процесс принудительного копирования папки

        public Form1()
        { 
            InitializeComponent();
            _fileSyncService = new FileSyncService();
            _configService = new ConfigService(cfg_file);

            _progress = new Progress<string>(message =>
            {
                CONSTS.AddNewLine(tblog, message);
            });

            _progressBar = new Progress<int>(message =>
            {
                CONSTS.invokeProgress(progressBar1, message);
            });

            Projects = _configService.GetProjects();
            List_Projects.DataSource = Projects;
            DriveLetter = CONSTS.GetDriveLetter();
            button1.Text = CONSTS.btSyncText1;
            this.FormClosing += Form1_FormClosing;
            List_Projects.SelectedIndexChanged += List_Projects_SelectedIndexChanged;
            list_dirs.SelectedIndexChanged += List_dirs_SelectedIndexChanged;
            list_dirs.DoubleClick += List_dirs_DoubleClick;
            checkBox1.CheckedChanged += CheckBox1_CheckedChanged;
            _progress.Report("USB-Flash: " + DriveLetter);
            button1.Click += button1_Click;
            List_Projects.DrawMode = DrawMode.OwnerDrawFixed;
            List_Projects.DrawItem += List_Projects_DrawItem;

        }

        



        #region Events Handlers

        /// <summary>
        /// открытие папки по двойному клику
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void List_dirs_DoubleClick(object sender, EventArgs e)
        {
            string selectedDir = string.Empty;
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

        private void List_Projects_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground(); // Отрисовка фона элемента

            if (e.Index < 0)
                return; // Проверка на корректный индекс

            string text = List_Projects.Items[e.Index].ToString();
            Color textColor = Color.Black; // Цвет текста по умолчанию
            Color backColor = Color.White; // Цвет фона по умолчанию

            Project project = (Project)List_Projects.Items[e.Index];

            // Логика для выбора цвета
            if (project.OnlineDirs.Count > 1)
            {
                textColor = Color.Black;
                backColor = Color.LightGreen;
            }
            else if (project.OnlineDirs.Count == 0)
            {
                textColor = Color.Black;
                backColor = Color.LightGray;
            }

            // Отрисовка фона с выбранным цветом
            using (SolidBrush backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            // Отрисовка текста с выбранным цветом
            using (SolidBrush textBrush = new SolidBrush(textColor))
            {
                e.Graphics.DrawString(text, e.Font, textBrush, e.Bounds, StringFormat.GenericDefault);
            }

            e.DrawFocusRectangle(); // Отрисовка рамки фокуса
        }


        //select project
        private void List_Projects_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayDirs();
            DisplayProjectInfo();
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            DisplayDirs();
            foreach (var project in Projects)
            {
                if (project.AutoSync)
                {
                    await StartSync(project, true);
                }
            }   
        }

        private async Task StartSync(Project project, bool isSilent)
        {
            // Меняем текст кнопки на "Остановить синхронизацию"
            button1.Text = CONSTS.btSyncText2;

            _syncCancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _syncCancellationTokenSource.Token;

            try
            {
                // Этап 1: Анализ файлов
                var selectedFiles = await AnalyzeProjectAsync(project, isSilent);
                if (selectedFiles == null)
                {
                    button1.Text = CONSTS.btSyncText1; // Вернуть текст кнопки, если анализ не дал результатов
                    return;
                }

                // Этап 2: Синхронизация файлов
                await SyncSelectedFilesAsync(project, selectedFiles, cancellationToken);
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


        //Start syncronization
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

            await SyncSelectedProject();

        }

        private async Task<List<Queue>> AnalyzeProjectAsync(Project project, bool isSilent)
        {
            var queue = await _fileSyncService.AnalyzeFilesAsync(project);

            if (queue.Count == 0)
            {
                if (!isSilent)
                    MessageBox.Show("Все папки одинаковые, нечего синхронизировать.");
                return null;
            }
            if (cbSilent.Checked || isSilent) return queue; // Если включен тихий режим, то не показываем окно выбора файлов

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
            _progress.Report("+++++++++++++++++++++++++++++++++++++++++++++++");
            _progress.Report($"{project.Name} - synchronization started");
            await _fileSyncService.SyncFilesAsync(selectedFiles, _progress, _progressBar, cancellationToken);

            // Обновляем данные проекта после успешной синхронизации
            project.LastSyncTime = DateTime.Now;
            project.LastSyncSize = GetProjectSize(project);
            _progress.Report($"{project.Name} - synchronization completed");
            _progress.Report("==============================================");
            SaveAllProjects();
        }

        private static long GetProjectSize(Project project)
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
            DriveLetter = newLette;
            CONSTS.AddNewLine(tblog, "New USB letter: " + DriveLetter);
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
        public static string GetRelativePath(string fullpath, string projDir)
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

                    if (Projects.Any(x => x.Name == projectName))
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
            if (Projects.Any(c => c.Name == newName))
            {
                MessageBox.Show("Такое имя уже есть в списке.");
                return;
            }
            selectedProj.Name = newName;
            SaveAllProjects();

        }
        private void синхронизироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SyncSelectedProject();
        }

        private void добавитьПапкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = List_Projects.SelectedItem;
            var dr = folderBrowserDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;
            string inputDir = folderBrowserDialog1.SelectedPath.TrimEnd('\\');
            if (string.IsNullOrWhiteSpace(inputDir) ||
                list_dirs.Items.Contains(new System.Windows.Forms.ListViewItem(inputDir)) ||
                selected == null) return;

            list_dirs.Items.Add(inputDir);
            var p = GetSelectedProject();
            var lette = inputDir.Split('\\')[0];
            if (lette == DriveLetter)
            {//removable disk
                string flashDriveDir = inputDir.TrimEnd('\\').Substring(lette.Length);
                p.AllProjectDirs.Add(new Projdir(flashDriveDir, p, CONSTS.FlashDrive));
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
            if (proj != null) proj.RemoveDir(DirRemove);
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
            var selectedProject = GetSelectedProject();
            if (selectedProject == null) return;
            string dir; //Selected directory into project

            if (list_dirs.Items.Count != 0)
                dir = list_dirs.Items[0].Text;
            else
            {
                MessageBox.Show("Добавьте папки в проект");
                return;
            }

            folderBrowserDialog1.InitialDirectory = dir;
            //folderBrowserDialog1.Multiselect = true;

            var dr = folderBrowserDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;
            var inputDirs = folderBrowserDialog1.SelectedPaths;
            if (inputDirs.Length == 0) return;
            foreach (string path in inputDirs)
            {
                if (string.IsNullOrWhiteSpace(path) ||
                    listExceptions.Items.Contains(new System.Windows.Forms.ListViewItem(path))) continue;

                string relativePath = GetRelativePath(path.TrimEnd('\\'), dir);
                if (selectedProject.ExceptionDirs.Contains(relativePath)) continue;
                selectedProject.ExceptionDirs.Add(relativePath);//добавление относительного пути

            }
            listExceptions.Items.Clear();
            listExceptions.Items.AddRange(selectedProject.ExceptionDirs.ToArray());
            SaveAllProjects();
        }

        //remove Except dir
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (list_dirs.Items.Count == 0) return; // Ensure there are items in the list

            if (listExceptions.Items.Count == 0) return;
            if (listExceptions.SelectedItems.Count == 0) return;
            var selectedExceptions = listExceptions.SelectedItems;
            var selectedProject = GetSelectedProject();
            if (selectedProject == null) return;

            //foreach (ListViewItem item in selectedExceptions)
            //{
            //    string selectedException = item.Text;
            //    if (!selectedProject.ExceptionDirs.Contains(selectedException))
            //        continue;
            //    selectedProject.ExceptionDirs.Remove(selectedException);
            //}
            selectedProject.ExceptionDirs.RemoveAll(x => selectedExceptions.Contains(x));

            listExceptions.Items.Clear();
            listExceptions.Items.AddRange(selectedProject.ExceptionDirs.ToArray());
            SaveAllProjects();
        }

        private void добавитьФайлыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedProject = GetSelectedProject();
            if (selectedProject == null) return;
            string dir; //Selected directory into project

            if (list_dirs.Items.Count != 0)
                dir = list_dirs.Items[0].Text;
            else
            {
                MessageBox.Show("Добавьте папки в проект");
                return;
            }

            openFileDialog1.InitialDirectory = dir;
            openFileDialog1.Multiselect = true;

            var dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;
            var inputDirs = openFileDialog1.FileNames;
            if (inputDirs.Length == 0) return;
            foreach (string path in inputDirs)
            {
                if (string.IsNullOrWhiteSpace(path) ||
                    listExceptions.Items.Contains(new System.Windows.Forms.ListViewItem(path))) continue;

                string relativePath = GetRelativePath(path, dir);
                if (selectedProject.ExceptionDirs.Contains(relativePath)) continue;
                selectedProject.ExceptionDirs.Add(relativePath);//добавление относительного пути

            }
            listExceptions.Items.Clear();
            listExceptions.Items.AddRange(selectedProject.ExceptionDirs.ToArray());
            SaveAllProjects();
        }

        private void удалитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedProject = GetSelectedProject();
            if (selectedProject == null) return;
            selectedProject.ExceptionDirs.Clear();
            listExceptions.Items.Clear();
            SaveAllProjects();
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
                if (DriveLetter == SelectedDirPATH.Split('\\')[0])//FlashDrive
                {
                    SelectedDirPATH = GetRelativePath(SelectedDirPATH, DriveLetter);
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
                    CONSTS.AddNewLine(tblog, $"\t\tГотово. Выбранная папка скопирована в {(Project.OnlineDirs.Count() - 1)} другие папки."); ;
                    CONSTS.EnableButton(button1); // кнопку старт в обычный режим
                });
                CopyDIRSThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ForceCopy");
                CONSTS.AddNewLine(tblog, "\t\tПроцесс копирования прерван.");
            }



        }
        #endregion

        /// <summary>
        /// Запускает процесс синхронизации выделенных проектов
        /// </summary>
        private async Task SyncSelectedProject()
        {
            var selectedProject = GetSelectedProject();
            await StartSync(selectedProject, false);
        }

        private void DisplayProjectInfo()
        {
            var selectedProj = GetSelectedProject();
            if (selectedProj == null) return;
            CONSTS.ClearLog(tblog);
            foreach(string line in selectedProj.Info())
            {
                CONSTS.AddNewLine(tblog, line);
            }
        }
        /// <summary>
        /// Run Sync projects with attribute AutoSync=True
        /// </summary>

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
               
    }
}
