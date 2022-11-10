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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.List_Projects = new System.Windows.Forms.ListBox();
            this.contextprojects = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.добавитьПроектToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.удалитьПроектToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.синхронизироватьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.переименоватьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.обновитьСписокПроектовToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.list_dirs = new System.Windows.Forms.ListView();
            this.contextdirs = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.добавитьПапкуToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.удалитьПапкуToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.копироватьЭтуПапкуВОстальныеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.listExceptions = new System.Windows.Forms.ListBox();
            this.contextExceptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.btSelectUSB = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tblog = new System.Windows.Forms.DataGridView();
            this.data = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.cbSilent = new System.Windows.Forms.CheckBox();
            this.btLog = new System.Windows.Forms.Button();
            this.contextprojects.SuspendLayout();
            this.contextdirs.SuspendLayout();
            this.contextExceptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tblog)).BeginInit();
            this.SuspendLayout();
            // 
            // List_Projects
            // 
            this.List_Projects.ContextMenuStrip = this.contextprojects;
            this.List_Projects.FormattingEnabled = true;
            this.List_Projects.Location = new System.Drawing.Point(3, 2);
            this.List_Projects.Name = "List_Projects";
            this.List_Projects.Size = new System.Drawing.Size(148, 147);
            this.List_Projects.TabIndex = 0;
            // 
            // contextprojects
            // 
            this.contextprojects.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.добавитьПроектToolStripMenuItem,
            this.удалитьПроектToolStripMenuItem,
            this.синхронизироватьToolStripMenuItem,
            this.переименоватьToolStripMenuItem,
            this.обновитьСписокПроектовToolStripMenuItem});
            this.contextprojects.Name = "contextprojects";
            this.contextprojects.Size = new System.Drawing.Size(225, 114);
            // 
            // добавитьПроектToolStripMenuItem
            // 
            this.добавитьПроектToolStripMenuItem.Name = "добавитьПроектToolStripMenuItem";
            this.добавитьПроектToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.добавитьПроектToolStripMenuItem.Text = "Добавить проект";
            this.добавитьПроектToolStripMenuItem.Click += new System.EventHandler(this.добавитьПроектToolStripMenuItem_Click);
            // 
            // удалитьПроектToolStripMenuItem
            // 
            this.удалитьПроектToolStripMenuItem.Name = "удалитьПроектToolStripMenuItem";
            this.удалитьПроектToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.удалитьПроектToolStripMenuItem.Text = "Удалить проект из списка";
            this.удалитьПроектToolStripMenuItem.Click += new System.EventHandler(this.удалитьПроектToolStripMenuItem_Click);
            // 
            // синхронизироватьToolStripMenuItem
            // 
            this.синхронизироватьToolStripMenuItem.Name = "синхронизироватьToolStripMenuItem";
            this.синхронизироватьToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.синхронизироватьToolStripMenuItem.Text = "-=Синхронизировать=-";
            this.синхронизироватьToolStripMenuItem.Click += new System.EventHandler(this.синхронизироватьToolStripMenuItem_Click);
            // 
            // переименоватьToolStripMenuItem
            // 
            this.переименоватьToolStripMenuItem.Name = "переименоватьToolStripMenuItem";
            this.переименоватьToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.переименоватьToolStripMenuItem.Text = "Переименовать...";
            this.переименоватьToolStripMenuItem.Click += new System.EventHandler(this.переименоватьToolStripMenuItem_Click);
            // 
            // обновитьСписокПроектовToolStripMenuItem
            // 
            this.обновитьСписокПроектовToolStripMenuItem.Name = "обновитьСписокПроектовToolStripMenuItem";
            this.обновитьСписокПроектовToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.обновитьСписокПроектовToolStripMenuItem.Text = "Обновить список проектов";
            this.обновитьСписокПроектовToolStripMenuItem.Click += new System.EventHandler(this.обновитьСписокПроектовToolStripMenuItem_Click);
            // 
            // list_dirs
            // 
            this.list_dirs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.list_dirs.ContextMenuStrip = this.contextdirs;
            this.list_dirs.HideSelection = false;
            this.list_dirs.Location = new System.Drawing.Point(157, 2);
            this.list_dirs.MultiSelect = false;
            this.list_dirs.Name = "list_dirs";
            this.list_dirs.Size = new System.Drawing.Size(368, 68);
            this.list_dirs.TabIndex = 2;
            this.list_dirs.UseCompatibleStateImageBehavior = false;
            this.list_dirs.View = System.Windows.Forms.View.List;
            // 
            // contextdirs
            // 
            this.contextdirs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.добавитьПапкуToolStripMenuItem,
            this.удалитьПапкуToolStripMenuItem,
            this.копироватьЭтуПапкуВОстальныеToolStripMenuItem});
            this.contextdirs.Name = "contextdirs";
            this.contextdirs.Size = new System.Drawing.Size(269, 70);
            // 
            // добавитьПапкуToolStripMenuItem
            // 
            this.добавитьПапкуToolStripMenuItem.Name = "добавитьПапкуToolStripMenuItem";
            this.добавитьПапкуToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.добавитьПапкуToolStripMenuItem.Text = "Добавить папку";
            this.добавитьПапкуToolStripMenuItem.Click += new System.EventHandler(this.добавитьПапкуToolStripMenuItem_Click);
            // 
            // удалитьПапкуToolStripMenuItem
            // 
            this.удалитьПапкуToolStripMenuItem.Name = "удалитьПапкуToolStripMenuItem";
            this.удалитьПапкуToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.удалитьПапкуToolStripMenuItem.Text = "Удалить папку";
            this.удалитьПапкуToolStripMenuItem.Click += new System.EventHandler(this.удалитьПапкуToolStripMenuItem_Click);
            // 
            // копироватьЭтуПапкуВОстальныеToolStripMenuItem
            // 
            this.копироватьЭтуПапкуВОстальныеToolStripMenuItem.Name = "копироватьЭтуПапкуВОстальныеToolStripMenuItem";
            this.копироватьЭтуПапкуВОстальныеToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.копироватьЭтуПапкуВОстальныеToolStripMenuItem.Text = "Копировать эту папку в остальные!";
            this.копироватьЭтуПапкуВОстальныеToolStripMenuItem.Click += new System.EventHandler(this.копироватьЭтуПапкуВОстальныеToolStripMenuItem_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 447);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(634, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 4;
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(531, 2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(77, 17);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.Text = "OnlineOnly";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged_1);
            // 
            // listExceptions
            // 
            this.listExceptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listExceptions.ContextMenuStrip = this.contextExceptions;
            this.listExceptions.FormattingEnabled = true;
            this.listExceptions.Location = new System.Drawing.Point(157, 88);
            this.listExceptions.Name = "listExceptions";
            this.listExceptions.Size = new System.Drawing.Size(367, 56);
            this.listExceptions.TabIndex = 7;
            // 
            // contextExceptions
            // 
            this.contextExceptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.contextExceptions.Name = "contextdirs";
            this.contextExceptions.Size = new System.Drawing.Size(162, 48);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItem1.Text = "Добавить папку";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItem2.Text = "Удалить папку";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(157, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Исключения:";
            // 
            // btSelectUSB
            // 
            this.btSelectUSB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSelectUSB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSelectUSB.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btSelectUSB.Location = new System.Drawing.Point(532, 59);
            this.btSelectUSB.Name = "btSelectUSB";
            this.btSelectUSB.Size = new System.Drawing.Size(102, 23);
            this.btSelectUSB.TabIndex = 9;
            this.btSelectUSB.Text = "Select USB drive...";
            this.btSelectUSB.UseVisualStyleBackColor = true;
            this.btSelectUSB.Click += new System.EventHandler(this.btSelectUSB_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.Aqua;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
            this.button1.FlatAppearance.BorderSize = 4;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button1.Location = new System.Drawing.Point(532, 87);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 56);
            this.button1.TabIndex = 10;
            this.button1.Text = "StartSync";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // tblog
            // 
            this.tblog.AllowUserToAddRows = false;
            this.tblog.AllowUserToDeleteRows = false;
            this.tblog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tblog.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.tblog.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.tblog.BackgroundColor = System.Drawing.SystemColors.Control;
            this.tblog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tblog.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.tblog.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.tblog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tblog.ColumnHeadersVisible = false;
            this.tblog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.data});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.tblog.DefaultCellStyle = dataGridViewCellStyle1;
            this.tblog.EnableHeadersVisualStyles = false;
            this.tblog.Location = new System.Drawing.Point(3, 156);
            this.tblog.Name = "tblog";
            this.tblog.ReadOnly = true;
            this.tblog.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.tblog.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Navy;
            this.tblog.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.tblog.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;
            this.tblog.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tblog.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Navy;
            this.tblog.RowTemplate.Height = 10;
            this.tblog.RowTemplate.ReadOnly = true;
            this.tblog.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.tblog.Size = new System.Drawing.Size(626, 285);
            this.tblog.TabIndex = 11;
            // 
            // data
            // 
            this.data.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.data.HeaderText = "data";
            this.data.Name = "data";
            this.data.ReadOnly = true;
            // 
            // checkBox2
            // 
            this.checkBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(531, 20);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(72, 17);
            this.checkBox2.TabIndex = 12;
            this.checkBox2.Text = "AutoSync";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // cbSilent
            // 
            this.cbSilent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSilent.AutoSize = true;
            this.cbSilent.Location = new System.Drawing.Point(531, 38);
            this.cbSilent.Name = "cbSilent";
            this.cbSilent.Size = new System.Drawing.Size(93, 17);
            this.cbSilent.TabIndex = 13;
            this.cbSilent.Text = "Тихий режим";
            this.cbSilent.UseVisualStyleBackColor = true;
            // 
            // btLog
            // 
            this.btLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btLog.Location = new System.Drawing.Point(609, 2);
            this.btLog.Name = "btLog";
            this.btLog.Size = new System.Drawing.Size(24, 23);
            this.btLog.TabIndex = 14;
            this.btLog.Text = "->";
            this.btLog.UseVisualStyleBackColor = true;
            this.btLog.Click += new System.EventHandler(this.btLog_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 470);
            this.Controls.Add(this.btLog);
            this.Controls.Add(this.cbSilent);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.tblog);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btSelectUSB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listExceptions);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.list_dirs);
            this.Controls.Add(this.List_Projects);
            this.Name = "Form1";
            this.Text = "SyncFlash";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextprojects.ResumeLayout(false);
            this.contextdirs.ResumeLayout(false);
            this.contextExceptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tblog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.Button btLog;
        private System.Windows.Forms.ToolStripMenuItem копироватьЭтуПапкуВОстальныеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem обновитьСписокПроектовToolStripMenuItem;
    }
}

