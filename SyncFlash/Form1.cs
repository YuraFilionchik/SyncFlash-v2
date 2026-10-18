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
using System.Windows.Forms;

namespace SyncFlash
{
    public partial class Form1 : Form
    {
        public string DriveLetter;
        public const string cfg_file = "conf.ini";
        string pc_name = Environment.MachineName;
        readonly BindingList<Project> Projects = new();
        private readonly IFileSyncService _fileSyncService;
        private readonly IConfigService _configService;
        private readonly IProgress<string> _progress;
        private readonly IProgress<int> _progressBar;

        private readonly SemaphoreSlim _syncGate = new(1, 1);
        private CancellationTokenSource? _syncCancellationTokenSource;
        private Task? _copyTask;
        private bool _isRunningSync;
        private bool _suppressProjectSave;

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
            _progress.Report(string.IsNullOrWhiteSpace(DriveLetter) ? "USB-Flash: не найден" : "USB-Flash: " + DriveLetter);
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
            if (selectedProj is null) return;

            var selectedDir = selectedProj.AllProjectDirs
                .FirstOrDefault(x => string.Equals(x.Dir, list_dirs.SelectedItems[0].Text, StringComparison.OrdinalIgnoreCase));

            CONSTS.ClearLog(tblog);
            CONSTS.AddNewLine(tblog, selectedProj.Name);
            if (selectedDir is not null)
            {
                foreach (var str in selectedDir.Info1())
                {
                    CONSTS.AddNewLine(tblog, str);
                }
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            DisplayDirs();
        }

        private void List_Projects_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= List_Projects.Items.Count) return;
            if (List_Projects.Items[e.Index] is not Project project) return;

            var selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            var textColor = selected ? SystemColors.HighlightText : Color.Black;
            var backColor = selected ? SystemColors.Highlight : Color.White;

            if (!selected)
            {
                var onlineCount = project.AllProjectDirs
                    .Where(d => d.PC_Name == pc_name || d.PC_Name == CONSTS.FlashDrive)
                    .Count(d => d.IsOnline);

                if (onlineCount > 1) backColor = Color.LightGreen;
                else if (onlineCount == 0) backColor = Color.LightGray;
            }

            using (var backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            var flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                        TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine;
            TextRenderer.DrawText(e.Graphics, project.ToString(), e.Font ?? List_Projects.Font, e.Bounds, textColor, flags);

            e.DrawFocusRectangle();
        }


        //select project
        private void List_Projects_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayDirs();
            DisplayProjectInfo();
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                DisplayDirs();
                await Task.Yield(); // даём окну отрисоваться до старта автосинхронизации

                foreach (var project in Projects.ToList())
                {
                    if (!project.AutoSync) continue;

                    try
                    {
                        await StartSync(project, true);
                    }
                    catch (Exception ex)
                    {
                        _progress.Report($"Автосинхронизация «{project.Name}»: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _progress.Report("Ошибка запуска: " + ex.Message);
            }
        }

        private async Task StartSync(Project? project, bool isSilent)
        {
            if (project is null)
            {
                if (!isSilent) MessageBox.Show("Сначала выберите проект.", "SyncFlash", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Не допускаем параллельные синхронизации.
            if (!await _syncGate.WaitAsync(0))
            {
                if (!isSilent) MessageBox.Show("Синхронизация уже выполняется.", "SyncFlash", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _isRunningSync = true;
            _syncCancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _syncCancellationTokenSource.Token;
            UpdateSyncUi();

            try
            {
                // Этап 1: Анализ файлов
                var selectedFiles = await AnalyzeProjectAsync(project, isSilent);
                if (selectedFiles is null || selectedFiles.Count == 0) return;

                // Этап 2: Синхронизация файлов
                await SyncSelectedFilesAsync(project, selectedFiles, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _progress.Report("Синхронизация прервана пользователем.");
                if (!isSilent) MessageBox.Show("Синхронизация прервана пользователем.", "SyncFlash", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _progress.Report("Ошибка синхронизации: " + ex.Message);
                if (!isSilent) MessageBox.Show(ex.Message, "Ошибка синхронизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _syncCancellationTokenSource?.Dispose();
                _syncCancellationTokenSource = null;
                _isRunningSync = false;
                UpdateSyncUi();
                _syncGate.Release();
            }
        }

        private void UpdateSyncUi()
        {
            button1.Text = _isRunningSync ? CONSTS.btSyncText2 : CONSTS.btSyncText1;
            синхронизироватьToolStripMenuItem.Enabled = !_isRunningSync;
            копироватьЭтуПапкуВОстальныеToolStripMenuItem.Enabled = !_isRunningSync;
        }

        //Start syncronization
        private async void button1_Click(object sender, EventArgs e)
        {
            if (_isRunningSync)
            {
                _syncCancellationTokenSource?.Cancel();
                return;
            }

            await SyncSelectedProject();
        }

        private async Task<List<Queue>?> AnalyzeProjectAsync(Project project, bool isSilent)
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
            project.InvalidateCaches();
            project.LastSyncSize = await Task.Run(() => GetProjectSize(project), cancellationToken);
            _progress.Report($"{project.Name} - synchronization completed");
            _progress.Report("==============================================");
            List_Projects.Invalidate();
            SaveAllProjects();
        }

        private static long GetProjectSize(Project project)
        {
            long totalSize = 0;
            var options = new EnumerationOptions
            {
                RecurseSubdirectories = true,
                IgnoreInaccessible = true,
                AttributesToSkip = FileAttributes.System | FileAttributes.ReparsePoint
            };

            foreach (var dir in project.AllProjectDirs)
            {
                try
                {
                    foreach (var file in Directory.EnumerateFiles(dir.Dir, "*", options))
                    {
                        try
                        {
                            totalSize += new FileInfo(file).Length;
                        }
                        catch (IOException) { }
                        catch (UnauthorizedAccessException) { }
                    }
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }

            return totalSize;
        }

        private Project? GetSelectedProject()
        {
            return List_Projects.SelectedItem as Project;
        }

        private void btSelectUSB_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.Description = "Выберите любую папку на USB/Select any folder on USB.";
            var dr = fd.ShowDialog();
            if (dr != DialogResult.OK) return;
            if (!Directory.Exists(fd.SelectedPath)) return;

            var root = Path.GetPathRoot(fd.SelectedPath);
            if (string.IsNullOrWhiteSpace(root)) return;

            DriveLetter = root.TrimEnd('\\');
            CONSTS.SetDriveLetter(DriveLetter);
            foreach (var project in Projects)
            {
                project.InvalidateCaches();
            }

            CONSTS.AddNewLine(tblog, "New USB letter: " + DriveLetter);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressProjectSave) return;
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
            if (string.IsNullOrWhiteSpace(fullpath)) return string.Empty;
            if (string.IsNullOrWhiteSpace(projDir)) return fullpath;

            var full = Path.GetFullPath(fullpath);

            // "D:" — корень диска, его нельзя прогонять через GetFullPath (получится текущий каталог диска).
            var root = projDir.Length == 2 && projDir[1] == ':'
                ? projDir + Path.DirectorySeparatorChar
                : Path.GetFullPath(projDir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (root.Length == 0) return full;

            var rootWithSeparator = root.EndsWith(Path.DirectorySeparatorChar)
                ? root
                : root + Path.DirectorySeparatorChar;

            if (string.Equals(full, root, StringComparison.OrdinalIgnoreCase)) return string.Empty;

            if (full.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase))
            {
                return full.Substring(rootWithSeparator.Length - 1); // сохраняем ведущий "\\"
            }

            return full;
        }
        /// <summary>
        /// Write dirs of selected project into list_dirs
        /// </summary>
        public void DisplayDirs()
        {
            list_dirs.Items.Clear();
            listExceptions.Items.Clear();

            var selectedProj = GetSelectedProject();
            if (selectedProj is null) return;

            _suppressProjectSave = true;
            try
            {
                checkBox2.Checked = selectedProj.AutoSync;
            }
            finally
            {
                _suppressProjectSave = false;
            }

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
            if (_isRunningSync || (_copyTask is not null && !_copyTask.IsCompleted))
            {
                var result = MessageBox.Show(
                    "Операция копирования ещё выполняется. Прервать и закрыть программу?",
                    "SyncFlash",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }

                _syncCancellationTokenSource?.Cancel();
            }

            SaveAllProjects();
        }

        private void SaveAllProjects()
        {
            if (!_configService.SaveProjects(Projects))
            {
                _progress.Report("Не удалось сохранить список проектов.");
            }
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
        private async void синхронизироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await SyncSelectedProject();
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
        private async void копироватьЭтуПапкуВОстальныеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var project = GetSelectedProject();
            if (project is null || list_dirs.SelectedItems.Count == 0) return;

            var sourceDir = list_dirs.SelectedItems[0].Text;
            if (string.IsNullOrWhiteSpace(sourceDir) || !Directory.Exists(sourceDir))
            {
                MessageBox.Show("Выбранная папка недоступна.", "SyncFlash", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_copyTask is not null && !_copyTask.IsCompleted)
            {
                MessageBox.Show("Копирование уже выполняется.", "SyncFlash", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var answer = MessageBox.Show(
                $"Скопировать папку{Environment.NewLine}{sourceDir}{Environment.NewLine}во все остальные папки проекта?{Environment.NewLine}{Environment.NewLine}Файлы будут перезаписаны без проверки версий.",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (answer != DialogResult.Yes) return;

            button1.Enabled = false;
            CONSTS.invokeProgress(progressBar1, 0);
            _copyTask = Task.Run(() => ForceCopyFolder(project, sourceDir));

            try
            {
                await _copyTask;
                CONSTS.invokeProgress(progressBar1, 100);
            }
            catch (Exception ex)
            {
                _progress.Report("Ошибка принудительного копирования: " + ex.Message);
                MessageBox.Show(ex.Message, "ForceCopy", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button1.Enabled = true;
            }
        }

        private void ForceCopyFolder(Project project, string sourceDir)
        {
            project.RefreshAvailability();

            var sourceFull = NormalizeDir(sourceDir);
            var targets = project.AllProjectDirs
                .Where(d => (d.PC_Name == Environment.MachineName || d.PC_Name == CONSTS.FlashDrive) && d.IsOnline)
                .Select(d => NormalizeDir(d.Dir))
                .Where(d => !string.Equals(d, sourceFull, StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (targets.Count == 0)
            {
                _progress.Report("Нет доступных целевых папок для копирования.");
                return;
            }

            _progress.Report($"Проект {project.Name}. Принудительное копирование {sourceFull}");

            var errors = new List<string>();
            var copied = 0;

            foreach (var target in targets)
            {
                try
                {
                    copied += DirectoryCopy(sourceFull, target, true, errors);
                    _progress.Report($"{sourceFull} }} ---> {{{target}");
                }
                catch (Exception ex)
                {
                    errors.Add($"{target}: {ex.Message}");
                }
            }

            _progress.Report($"Готово. Скопировано файлов: {copied}. Ошибок: {errors.Count}.");
            foreach (var error in errors)
            {
                _progress.Report(error);
            }
        }
        #endregion

        /// <summary>
        /// Запускает процесс синхронизации выделенных проектов
        /// </summary>
        private async Task SyncSelectedProject()
        {
            var selectedProject = GetSelectedProject();
            if (selectedProject is null)
            {
                _progress.Report("Сначала выберите проект.");
                return;
            }

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
        private static string NormalizeDir(string path)
        {
            var full = Path.GetFullPath(path);
            return full.Length <= 3 && full.EndsWith(Path.DirectorySeparatorChar)
                ? full
                : full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        private static int DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, List<string>? errors = null)
        {
            var sourceFull = NormalizeDir(sourceDirName);
            var destFull = NormalizeDir(destDirName);

            // Защита от копирования папки в саму себя и в собственную вложенную папку.
            if (string.Equals(sourceFull, destFull, StringComparison.OrdinalIgnoreCase)) return 0;
            if (destFull.StartsWith(sourceFull + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) return 0;

            var dir = new DirectoryInfo(sourceFull);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Исходная папка не найдена: " + sourceFull);
            }

            if (!Directory.Exists(destFull))
            {
                Directory.CreateDirectory(destFull);
            }

            var copied = 0;

            foreach (var file in dir.GetFiles())
            {
                var target = Path.Combine(destFull, file.Name);
                try
                {
                    file.CopyTo(target, true);
                    copied++;
                }
                catch (Exception ex)
                {
                    errors?.Add($"{file.FullName}: {ex.Message}");
                }
            }

            if (copySubDirs)
            {
                foreach (var subdir in dir.GetDirectories())
                {
                    copied += DirectoryCopy(subdir.FullName, Path.Combine(destFull, subdir.Name), true, errors);
                }
            }

            return copied;
        }
               
    }
}
