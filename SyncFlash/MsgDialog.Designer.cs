namespace SyncFlash
{
    partial class MsgDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.check = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Source = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateSource = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Arrow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateTarget = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Target = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.добавитьФайлВИсключенияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.отменитьДобавленныеИсключенияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.поменятьМестамиИсточникИНазначениеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.удалитьФайлВоВсехПапкахToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(22, 327);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(93, 33);
            this.btOK.TabIndex = 0;
            this.btOK.Tag = "Копировать";
            this.btOK.Text = "Копировать";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.Location = new System.Drawing.Point(655, 327);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(95, 33);
            this.btCancel.TabIndex = 1;
            this.btCancel.Text = "Отмена";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.check,
            this.Number,
            this.Source,
            this.DateSource,
            this.Arrow,
            this.DateTarget,
            this.Target});
            this.dgv.ContextMenuStrip = this.contextMenuStrip1;
            this.dgv.Location = new System.Drawing.Point(1, -2);
            this.dgv.MultiSelect = false;
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersVisible = false;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(775, 323);
            this.dgv.TabIndex = 2;
            // 
            // check
            // 
            this.check.HeaderText = "V";
            this.check.Name = "check";
            this.check.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.check.Width = 24;
            // 
            // Number
            // 
            this.Number.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Number.HeaderText = "№";
            this.Number.Name = "Number";
            this.Number.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Number.Width = 43;
            // 
            // Source
            // 
            this.Source.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Source.HeaderText = "Источник";
            this.Source.MinimumWidth = 100;
            this.Source.Name = "Source";
            // 
            // DateSource
            // 
            this.DateSource.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.DateSource.HeaderText = "Изменен";
            this.DateSource.Name = "DateSource";
            this.DateSource.Width = 78;
            // 
            // Arrow
            // 
            this.Arrow.HeaderText = "-->";
            this.Arrow.Name = "Arrow";
            this.Arrow.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Arrow.ToolTipText = "Направление";
            this.Arrow.Width = 30;
            // 
            // DateTarget
            // 
            this.DateTarget.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.DateTarget.HeaderText = "Изменен";
            this.DateTarget.Name = "DateTarget";
            this.DateTarget.Width = 78;
            // 
            // Target
            // 
            this.Target.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Target.HeaderText = "Назначение";
            this.Target.Name = "Target";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.добавитьФайлВИсключенияToolStripMenuItem,
            this.отменитьДобавленныеИсключенияToolStripMenuItem,
            this.поменятьМестамиИсточникИНазначениеToolStripMenuItem,
            this.удалитьФайлВоВсехПапкахToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(317, 114);
            // 
            // добавитьФайлВИсключенияToolStripMenuItem
            // 
            this.добавитьФайлВИсключенияToolStripMenuItem.Name = "добавитьФайлВИсключенияToolStripMenuItem";
            this.добавитьФайлВИсключенияToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.добавитьФайлВИсключенияToolStripMenuItem.Text = "Добавить файл в исключения";
            this.добавитьФайлВИсключенияToolStripMenuItem.Click += new System.EventHandler(this.добавитьФайлВИсключенияToolStripMenuItem_Click);
            // 
            // отменитьДобавленныеИсключенияToolStripMenuItem
            // 
            this.отменитьДобавленныеИсключенияToolStripMenuItem.Name = "отменитьДобавленныеИсключенияToolStripMenuItem";
            this.отменитьДобавленныеИсключенияToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.отменитьДобавленныеИсключенияToolStripMenuItem.Text = "Отменить добавленные исключения";
            this.отменитьДобавленныеИсключенияToolStripMenuItem.Click += new System.EventHandler(this.отменитьДобавленныеИсключенияToolStripMenuItem_Click);
            // 
            // поменятьМестамиИсточникИНазначениеToolStripMenuItem
            // 
            this.поменятьМестамиИсточникИНазначениеToolStripMenuItem.Name = "поменятьМестамиИсточникИНазначениеToolStripMenuItem";
            this.поменятьМестамиИсточникИНазначениеToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.поменятьМестамиИсточникИНазначениеToolStripMenuItem.Text = "Поменять местами Источник и Назначение";
            this.поменятьМестамиИсточникИНазначениеToolStripMenuItem.Click += new System.EventHandler(this.поменятьМестамиИсточникИНазначениеToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(122, 337);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "label1";
            // 
            // удалитьФайлВоВсехПапкахToolStripMenuItem
            // 
            this.удалитьФайлВоВсехПапкахToolStripMenuItem.Name = "удалитьФайлВоВсехПапкахToolStripMenuItem";
            this.удалитьФайлВоВсехПапкахToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.удалитьФайлВоВсехПапкахToolStripMenuItem.Text = "Удалить файл источника и назначения";
            this.удалитьФайлВоВсехПапкахToolStripMenuItem.Click += new System.EventHandler(this.удалитьФайлВоВсехПапкахToolStripMenuItem_Click);
            // 
            // MsgDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 362);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximumSize = new System.Drawing.Size(1400, 700);
            this.Name = "MsgDialog";
            this.Text = "Синхронизация каталогов";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.DataGridViewCheckBoxColumn check;
        private System.Windows.Forms.DataGridViewTextBoxColumn Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn Source;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn Arrow;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateTarget;
        private System.Windows.Forms.DataGridViewTextBoxColumn Target;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem добавитьФайлВИсключенияToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem отменитьДобавленныеИсключенияToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem поменятьМестамиИсточникИНазначениеToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem удалитьФайлВоВсехПапкахToolStripMenuItem;
    }
}