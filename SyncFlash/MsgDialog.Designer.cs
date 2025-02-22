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
            components = new System.ComponentModel.Container();
            btOK = new System.Windows.Forms.Button();
            btCancel = new System.Windows.Forms.Button();
            dgv = new System.Windows.Forms.DataGridView();
            check = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Source = new System.Windows.Forms.DataGridViewTextBoxColumn();
            DateSource = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Arrow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            DateTarget = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Target = new System.Windows.Forms.DataGridViewTextBoxColumn();
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            добавитьФайлВИсключенияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            отменитьДобавленныеИсключенияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            поменятьМестамиИсточникИНазначениеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            удалитьФайлВоВсехПапкахToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            groupBox1 = new System.Windows.Forms.GroupBox();
            radioButton4 = new System.Windows.Forms.RadioButton();
            radioButton3 = new System.Windows.Forms.RadioButton();
            radioButton2 = new System.Windows.Forms.RadioButton();
            radioButton1 = new System.Windows.Forms.RadioButton();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBox1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // btOK
            // 
            btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            btOK.BackColor = System.Drawing.Color.FromArgb(128, 255, 128);
            btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            btOK.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            btOK.Location = new System.Drawing.Point(4, 457);
            btOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btOK.Name = "btOK";
            btOK.Size = new System.Drawing.Size(150, 46);
            btOK.TabIndex = 0;
            btOK.Tag = "Копировать";
            btOK.Text = "Копировать";
            btOK.UseVisualStyleBackColor = false;
            btOK.Click += btOK_Click;
            // 
            // btCancel
            // 
            btCancel.BackColor = System.Drawing.Color.FromArgb(255, 128, 128);
            btCancel.Dock = System.Windows.Forms.DockStyle.Bottom;
            btCancel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
            btCancel.Location = new System.Drawing.Point(0, 527);
            btCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btCancel.Name = "btCancel";
            btCancel.Size = new System.Drawing.Size(158, 48);
            btCancel.TabIndex = 1;
            btCancel.Text = "Отмена";
            btCancel.UseVisualStyleBackColor = false;
            btCancel.Click += btCancel_Click;
            // 
            // dgv
            // 
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { check, Number, Source, DateSource, Arrow, DateTarget, Target });
            dgv.ContextMenuStrip = contextMenuStrip1;
            dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            dgv.Location = new System.Drawing.Point(0, 0);
            dgv.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dgv.MultiSelect = false;
            dgv.Name = "dgv";
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgv.Size = new System.Drawing.Size(1028, 575);
            dgv.TabIndex = 2;
            // 
            // check
            // 
            check.HeaderText = "V";
            check.Name = "check";
            check.ReadOnly = true;
            check.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            check.Width = 24;
            // 
            // Number
            // 
            Number.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            Number.HeaderText = "№";
            Number.Name = "Number";
            Number.ReadOnly = true;
            Number.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            Number.Width = 45;
            // 
            // Source
            // 
            Source.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Source.HeaderText = "Источник";
            Source.MinimumWidth = 100;
            Source.Name = "Source";
            Source.ReadOnly = true;
            // 
            // DateSource
            // 
            DateSource.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            DateSource.HeaderText = "Изменен";
            DateSource.Name = "DateSource";
            DateSource.ReadOnly = true;
            DateSource.Width = 81;
            // 
            // Arrow
            // 
            Arrow.HeaderText = "-->";
            Arrow.Name = "Arrow";
            Arrow.ReadOnly = true;
            Arrow.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            Arrow.ToolTipText = "Направление";
            Arrow.Width = 30;
            // 
            // DateTarget
            // 
            DateTarget.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            DateTarget.HeaderText = "Изменен";
            DateTarget.Name = "DateTarget";
            DateTarget.ReadOnly = true;
            DateTarget.Width = 81;
            // 
            // Target
            // 
            Target.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Target.HeaderText = "Назначение";
            Target.Name = "Target";
            Target.ReadOnly = true;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { добавитьФайлВИсключенияToolStripMenuItem, отменитьДобавленныеИсключенияToolStripMenuItem, поменятьМестамиИсточникИНазначениеToolStripMenuItem, удалитьФайлВоВсехПапкахToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(317, 92);
            // 
            // добавитьФайлВИсключенияToolStripMenuItem
            // 
            добавитьФайлВИсключенияToolStripMenuItem.Name = "добавитьФайлВИсключенияToolStripMenuItem";
            добавитьФайлВИсключенияToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            добавитьФайлВИсключенияToolStripMenuItem.Text = "Добавить файл в исключения";
            добавитьФайлВИсключенияToolStripMenuItem.Click += добавитьФайлВИсключенияToolStripMenuItem_Click;
            // 
            // отменитьДобавленныеИсключенияToolStripMenuItem
            // 
            отменитьДобавленныеИсключенияToolStripMenuItem.Name = "отменитьДобавленныеИсключенияToolStripMenuItem";
            отменитьДобавленныеИсключенияToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            отменитьДобавленныеИсключенияToolStripMenuItem.Text = "Отменить добавленные исключения";
            отменитьДобавленныеИсключенияToolStripMenuItem.Click += отменитьДобавленныеИсключенияToolStripMenuItem_Click;
            // 
            // поменятьМестамиИсточникИНазначениеToolStripMenuItem
            // 
            поменятьМестамиИсточникИНазначениеToolStripMenuItem.Name = "поменятьМестамиИсточникИНазначениеToolStripMenuItem";
            поменятьМестамиИсточникИНазначениеToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            поменятьМестамиИсточникИНазначениеToolStripMenuItem.Text = "Поменять местами Источник и Назначение";
            поменятьМестамиИсточникИНазначениеToolStripMenuItem.Click += поменятьМестамиИсточникИНазначениеToolStripMenuItem_Click;
            // 
            // удалитьФайлВоВсехПапкахToolStripMenuItem
            // 
            удалитьФайлВоВсехПапкахToolStripMenuItem.Name = "удалитьФайлВоВсехПапкахToolStripMenuItem";
            удалитьФайлВоВсехПапкахToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            удалитьФайлВоВсехПапкахToolStripMenuItem.Text = "Удалить файл источника и назначения";
            удалитьФайлВоВсехПапкахToolStripMenuItem.Click += удалитьФайлВоВсехПапкахToolStripMenuItem_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Top;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
            splitContainer1.Panel1.Controls.Add(groupBox1);
            splitContainer1.Panel1.Controls.Add(btCancel);
            splitContainer1.Panel1.Controls.Add(btOK);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(dgv);
            splitContainer1.Size = new System.Drawing.Size(1190, 575);
            splitContainer1.SplitterDistance = 158;
            splitContainer1.TabIndex = 4;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.AutoSize = true;
            groupBox1.Controls.Add(radioButton4);
            groupBox1.Controls.Add(radioButton3);
            groupBox1.Controls.Add(radioButton2);
            groupBox1.Controls.Add(radioButton1);
            groupBox1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            groupBox1.Location = new System.Drawing.Point(4, 32);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(150, 147);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Filters:";
            // 
            // radioButton4
            // 
            radioButton4.AutoSize = true;
            radioButton4.Location = new System.Drawing.Point(9, 94);
            radioButton4.Name = "radioButton4";
            radioButton4.Size = new System.Drawing.Size(120, 25);
            radioButton4.TabIndex = 3;
            radioButton4.TabStop = true;
            radioButton4.Text = "Not modified";
            radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Location = new System.Drawing.Point(9, 70);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new System.Drawing.Size(90, 25);
            radioButton3.TabIndex = 2;
            radioButton3.TabStop = true;
            radioButton3.Text = "Modified";
            radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new System.Drawing.Point(9, 46);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new System.Drawing.Size(60, 25);
            radioButton2.TabIndex = 1;
            radioButton2.TabStop = true;
            radioButton2.Text = "New";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new System.Drawing.Point(9, 22);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new System.Drawing.Size(46, 25);
            radioButton1.TabIndex = 0;
            radioButton1.TabStop = true;
            radioButton1.Text = "All";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new System.Drawing.Point(0, 578);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(1190, 22);
            statusStrip1.TabIndex = 5;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // MsgDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1190, 600);
            Controls.Add(statusStrip1);
            Controls.Add(splitContainer1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximumSize = new System.Drawing.Size(1631, 802);
            Name = "MsgDialog";
            Text = "Синхронизация каталогов";
            Load += MsgDialog_Load;
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem удалитьФайлВоВсехПапкахToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
    }
}