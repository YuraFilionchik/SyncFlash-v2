namespace SyncFlash
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            List_Projects = new System.Windows.Forms.ListBox();
            contextprojects = new System.Windows.Forms.ContextMenuStrip(components);
            добавитьПроектToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            удалитьПроектToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            синхронизироватьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            переименоватьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            list_dirs = new System.Windows.Forms.ListView();
            contextdirs = new System.Windows.Forms.ContextMenuStrip(components);
            добавитьПапкуToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            удалитьПапкуToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            копироватьЭтуПапкуВОстальныеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            checkBox1 = new System.Windows.Forms.CheckBox();
            listExceptions = new System.Windows.Forms.ListBox();
            contextExceptions = new System.Windows.Forms.ContextMenuStrip(components);
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            добавитьФайлыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            удалитьВсеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            label1 = new System.Windows.Forms.Label();
            btSelectUSB = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            tblog = new System.Windows.Forms.DataGridView();
            data = new System.Windows.Forms.DataGridViewTextBoxColumn();
            checkBox2 = new System.Windows.Forms.CheckBox();
            cbSilent = new System.Windows.Forms.CheckBox();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            label2 = new System.Windows.Forms.Label();
            folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            groupBox1 = new System.Windows.Forms.GroupBox();
            contextprojects.SuspendLayout();
            contextdirs.SuspendLayout();
            contextExceptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tblog).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // List_Projects
            // 
            List_Projects.BackColor = System.Drawing.SystemColors.ButtonShadow;
            List_Projects.ContextMenuStrip = contextprojects;
            List_Projects.Dock = System.Windows.Forms.DockStyle.Fill;
            List_Projects.FormattingEnabled = true;
            List_Projects.Location = new System.Drawing.Point(0, 0);
            List_Projects.Margin = new System.Windows.Forms.Padding(0);
            List_Projects.Name = "List_Projects";
            List_Projects.Size = new System.Drawing.Size(358, 247);
            List_Projects.TabIndex = 0;
            // 
            // contextprojects
            // 
            contextprojects.ImageScalingSize = new System.Drawing.Size(20, 20);
            contextprojects.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { добавитьПроектToolStripMenuItem, удалитьПроектToolStripMenuItem, синхронизироватьToolStripMenuItem, переименоватьToolStripMenuItem });
            contextprojects.Name = "contextprojects";
            contextprojects.Size = new System.Drawing.Size(216, 92);
            // 
            // добавитьПроектToolStripMenuItem
            // 
            добавитьПроектToolStripMenuItem.Name = "добавитьПроектToolStripMenuItem";
            добавитьПроектToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            добавитьПроектToolStripMenuItem.Text = "Добавить проект";
            добавитьПроектToolStripMenuItem.Click += добавитьПроектToolStripMenuItem_Click;
            // 
            // удалитьПроектToolStripMenuItem
            // 
            удалитьПроектToolStripMenuItem.Name = "удалитьПроектToolStripMenuItem";
            удалитьПроектToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            удалитьПроектToolStripMenuItem.Text = "Удалить проект из списка";
            удалитьПроектToolStripMenuItem.Click += удалитьПроектToolStripMenuItem_Click;
            // 
            // синхронизироватьToolStripMenuItem
            // 
            синхронизироватьToolStripMenuItem.Name = "синхронизироватьToolStripMenuItem";
            синхронизироватьToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            синхронизироватьToolStripMenuItem.Text = "-=Синхронизировать=-";
            синхронизироватьToolStripMenuItem.Click += синхронизироватьToolStripMenuItem_Click;
            // 
            // переименоватьToolStripMenuItem
            // 
            переименоватьToolStripMenuItem.Name = "переименоватьToolStripMenuItem";
            переименоватьToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            переименоватьToolStripMenuItem.Text = "Переименовать...";
            переименоватьToolStripMenuItem.Click += переименоватьToolStripMenuItem_Click;
            // 
            // list_dirs
            // 
            list_dirs.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            list_dirs.BackColor = System.Drawing.SystemColors.ButtonShadow;
            list_dirs.ContextMenuStrip = contextdirs;
            list_dirs.Location = new System.Drawing.Point(0, 31);
            list_dirs.Margin = new System.Windows.Forms.Padding(0);
            list_dirs.MultiSelect = false;
            list_dirs.Name = "list_dirs";
            list_dirs.Size = new System.Drawing.Size(572, 100);
            list_dirs.TabIndex = 2;
            list_dirs.UseCompatibleStateImageBehavior = false;
            list_dirs.View = System.Windows.Forms.View.List;
            // 
            // contextdirs
            // 
            contextdirs.ImageScalingSize = new System.Drawing.Size(20, 20);
            contextdirs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { добавитьПапкуToolStripMenuItem, удалитьПапкуToolStripMenuItem, копироватьЭтуПапкуВОстальныеToolStripMenuItem });
            contextdirs.Name = "contextdirs";
            contextdirs.Size = new System.Drawing.Size(269, 70);
            // 
            // добавитьПапкуToolStripMenuItem
            // 
            добавитьПапкуToolStripMenuItem.Name = "добавитьПапкуToolStripMenuItem";
            добавитьПапкуToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            добавитьПапкуToolStripMenuItem.Text = "Добавить папку";
            добавитьПапкуToolStripMenuItem.Click += добавитьПапкуToolStripMenuItem_Click;
            // 
            // удалитьПапкуToolStripMenuItem
            // 
            удалитьПапкуToolStripMenuItem.Name = "удалитьПапкуToolStripMenuItem";
            удалитьПапкуToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            удалитьПапкуToolStripMenuItem.Text = "Удалить папку";
            удалитьПапкуToolStripMenuItem.Click += удалитьПапкуToolStripMenuItem_Click;
            // 
            // копироватьЭтуПапкуВОстальныеToolStripMenuItem
            // 
            копироватьЭтуПапкуВОстальныеToolStripMenuItem.Name = "копироватьЭтуПапкуВОстальныеToolStripMenuItem";
            копироватьЭтуПапкуВОстальныеToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            копироватьЭтуПапкуВОстальныеToolStripMenuItem.Text = "Копировать эту папку в остальные!";
            копироватьЭтуПапкуВОстальныеToolStripMenuItem.Click += копироватьЭтуПапкуВОстальныеToolStripMenuItem_Click;
            // 
            // progressBar1
            // 
            progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            progressBar1.Location = new System.Drawing.Point(0, 587);
            progressBar1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(1101, 35);
            progressBar1.Step = 1;
            progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            progressBar1.TabIndex = 4;
            // 
            // checkBox1
            // 
            checkBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            checkBox1.AutoSize = true;
            checkBox1.Checked = true;
            checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox1.Location = new System.Drawing.Point(14, 12);
            checkBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new System.Drawing.Size(86, 19);
            checkBox1.TabIndex = 5;
            checkBox1.Text = "OnlineOnly";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += CheckBox1_CheckedChanged_1;
            // 
            // listExceptions
            // 
            listExceptions.BackColor = System.Drawing.SystemColors.ButtonShadow;
            listExceptions.ContextMenuStrip = contextExceptions;
            listExceptions.Dock = System.Windows.Forms.DockStyle.Bottom;
            listExceptions.FormattingEnabled = true;
            listExceptions.Location = new System.Drawing.Point(0, 168);
            listExceptions.Margin = new System.Windows.Forms.Padding(0);
            listExceptions.Name = "listExceptions";
            listExceptions.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            listExceptions.Size = new System.Drawing.Size(575, 79);
            listExceptions.Sorted = true;
            listExceptions.TabIndex = 7;
            // 
            // contextExceptions
            // 
            contextExceptions.ImageScalingSize = new System.Drawing.Size(20, 20);
            contextExceptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripMenuItem1, добавитьФайлыToolStripMenuItem, toolStripMenuItem2, удалитьВсеToolStripMenuItem });
            contextExceptions.Name = "contextdirs";
            contextExceptions.Size = new System.Drawing.Size(186, 92);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(185, 22);
            toolStripMenuItem1.Text = "Добавить папки...";
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            // 
            // добавитьФайлыToolStripMenuItem
            // 
            добавитьФайлыToolStripMenuItem.Name = "добавитьФайлыToolStripMenuItem";
            добавитьФайлыToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            добавитьФайлыToolStripMenuItem.Text = "Добавить файлы...";
            добавитьФайлыToolStripMenuItem.Click += добавитьФайлыToolStripMenuItem_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(185, 22);
            toolStripMenuItem2.Text = "Удалить выбранные";
            toolStripMenuItem2.Click += toolStripMenuItem2_Click;
            // 
            // удалитьВсеToolStripMenuItem
            // 
            удалитьВсеToolStripMenuItem.Name = "удалитьВсеToolStripMenuItem";
            удалитьВсеToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            удалитьВсеToolStripMenuItem.Text = "Удалить все";
            удалитьВсеToolStripMenuItem.Click += удалитьВсеToolStripMenuItem_Click;
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(4, 145);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(177, 15);
            label1.TabIndex = 8;
            label1.Text = "Исключить из синхронизации:";
            // 
            // btSelectUSB
            // 
            btSelectUSB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btSelectUSB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btSelectUSB.Font = new System.Drawing.Font("Times New Roman", 8.25F);
            btSelectUSB.Location = new System.Drawing.Point(946, 129);
            btSelectUSB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btSelectUSB.Name = "btSelectUSB";
            btSelectUSB.Size = new System.Drawing.Size(146, 35);
            btSelectUSB.TabIndex = 9;
            btSelectUSB.Text = "Select USB drive...";
            btSelectUSB.UseVisualStyleBackColor = true;
            btSelectUSB.Click += btSelectUSB_Click;
            // 
            // button1
            // 
            button1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            button1.BackColor = System.Drawing.Color.Aqua;
            button1.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
            button1.FlatAppearance.BorderSize = 4;
            button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            button1.ForeColor = System.Drawing.SystemColors.ControlText;
            button1.Location = new System.Drawing.Point(946, 169);
            button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(146, 86);
            button1.TabIndex = 10;
            button1.Text = "StartSync";
            button1.UseVisualStyleBackColor = false;
            // 
            // tblog
            // 
            tblog.AllowUserToAddRows = false;
            tblog.AllowUserToDeleteRows = false;
            tblog.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            tblog.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            tblog.BackgroundColor = System.Drawing.SystemColors.ButtonShadow;
            tblog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            tblog.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            tblog.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            tblog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tblog.ColumnHeadersVisible = false;
            tblog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { data });
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            tblog.DefaultCellStyle = dataGridViewCellStyle1;
            tblog.Dock = System.Windows.Forms.DockStyle.Fill;
            tblog.EnableHeadersVisualStyles = false;
            tblog.Location = new System.Drawing.Point(0, 0);
            tblog.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tblog.Name = "tblog";
            tblog.ReadOnly = true;
            tblog.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            tblog.RowHeadersVisible = false;
            tblog.RowHeadersWidth = 51;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Navy;
            tblog.RowsDefaultCellStyle = dataGridViewCellStyle2;
            tblog.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;
            tblog.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            tblog.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Navy;
            tblog.RowTemplate.Height = 10;
            tblog.RowTemplate.ReadOnly = true;
            tblog.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            tblog.Size = new System.Drawing.Size(938, 334);
            tblog.TabIndex = 11;
            // 
            // data
            // 
            data.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            data.HeaderText = "data";
            data.MinimumWidth = 6;
            data.Name = "data";
            data.ReadOnly = true;
            // 
            // checkBox2
            // 
            checkBox2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            checkBox2.AutoSize = true;
            checkBox2.Location = new System.Drawing.Point(14, 66);
            checkBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new System.Drawing.Size(78, 19);
            checkBox2.TabIndex = 12;
            checkBox2.Text = "AutoSync";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // cbSilent
            // 
            cbSilent.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            cbSilent.AutoSize = true;
            cbSilent.Location = new System.Drawing.Point(14, 39);
            cbSilent.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            cbSilent.Name = "cbSilent";
            cbSilent.Size = new System.Drawing.Size(101, 19);
            cbSilent.TabIndex = 13;
            cbSilent.Text = "Тихий режим";
            cbSilent.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tblog);
            splitContainer1.Size = new System.Drawing.Size(938, 587);
            splitContainer1.SplitterDistance = 247;
            splitContainer1.SplitterWidth = 6;
            splitContainer1.TabIndex = 15;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.Location = new System.Drawing.Point(0, 0);
            splitContainer2.Margin = new System.Windows.Forms.Padding(0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(List_Projects);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(label2);
            splitContainer2.Panel2.Controls.Add(label1);
            splitContainer2.Panel2.Controls.Add(list_dirs);
            splitContainer2.Panel2.Controls.Add(listExceptions);
            splitContainer2.Size = new System.Drawing.Size(938, 247);
            splitContainer2.SplitterDistance = 358;
            splitContainer2.SplitterWidth = 5;
            splitContainer2.TabIndex = 9;
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(3, 6);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(170, 15);
            label2.TabIndex = 8;
            label2.Text = "Объекты для синхронизации:";
            // 
            // folderBrowserDialog1
            // 
            folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            //folderBrowserDialog1.ShowHiddenFiles = true;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Multiselect = true;
            //openFileDialog1.ShowHiddenFiles = true;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.AutoSize = true;
            groupBox1.BackColor = System.Drawing.SystemColors.ControlDark;
            groupBox1.Controls.Add(cbSilent);
            groupBox1.Controls.Add(checkBox1);
            groupBox1.Controls.Add(checkBox2);
            groupBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
            groupBox1.Location = new System.Drawing.Point(945, 12);
            groupBox1.Margin = new System.Windows.Forms.Padding(0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(152, 112);
            groupBox1.TabIndex = 16;
            groupBox1.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1101, 622);
            Controls.Add(groupBox1);
            Controls.Add(splitContainer1);
            Controls.Add(button1);
            Controls.Add(btSelectUSB);
            Controls.Add(progressBar1);
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            MinimumSize = new System.Drawing.Size(756, 549);
            Name = "Form1";
            Text = "SyncFlash";
            Load += Form1_Load;
            contextprojects.ResumeLayout(false);
            contextdirs.ResumeLayout(false);
            contextExceptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tblog).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListBox List_Projects;
        private System.Windows.Forms.ListView list_dirs;
        private System.Windows.Forms.ContextMenuStrip contextprojects;
        private System.Windows.Forms.ToolStripMenuItem добавитьПроектToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem удалитьПроектToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextdirs;
        private System.Windows.Forms.ToolStripMenuItem добавитьПапкуToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem удалитьПапкуToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ListBox listExceptions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextExceptions;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        public System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ToolStripMenuItem синхронизироватьToolStripMenuItem;
        private System.Windows.Forms.Button btSelectUSB;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView tblog;
        private System.Windows.Forms.DataGridViewTextBoxColumn data;
        private System.Windows.Forms.ToolStripMenuItem переименоватьToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox cbSilent;
        private System.Windows.Forms.ToolStripMenuItem копироватьЭтуПапкуВОстальныеToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem добавитьФайлыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem удалитьВсеToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

