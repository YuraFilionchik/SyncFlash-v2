using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SyncFlash
{
    public partial class MsgDialog : Form
    {
        public List<Queue> ReturnedQueue;
        public List<Queue> ExceptionsList = new List<Queue>();
        private bool AddExceptions = false;
        private Project currentProject;

        public MsgDialog(Project project, List<Queue> queues)
        {
            try
            {
                InitializeComponent();
                currentProject = project;
                dgv.SelectionChanged += Dgv_SelectionChanged;
                //dgv checkbox checked
                dgv.CellContentClick += Dgv_CellContentClick;
                ReturnedQueue = queues;
                radioButton1.Checked = true;
                //Fill datagridview
                DisplayQueues(queues);
                radioButton1.CheckedChanged += RadioButton_CheckedChanged;
                radioButton2.CheckedChanged += RadioButton_CheckedChanged;
                radioButton3.CheckedChanged += RadioButton_CheckedChanged;
                radioButton4.CheckedChanged += RadioButton_CheckedChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MsgDialog");
            }


        }

        private void Dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0) return;
            DataGridViewRow selectedrow = dgv.Rows[e.RowIndex];
            if (selectedrow == null) return;
            bool cellCheckedState = (bool)selectedrow.Cells[0].EditedFormattedValue;
            int Number = (int)selectedrow.Cells["Number"].Value;//number of selected queue
            Queue selectedQueue = ReturnedQueue.Find(x => x.Number == Number);
            selectedQueue.Active = cellCheckedState;
        }

        /// <summary>
        /// Filtering view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)//All files
            {
                DisplayQueues(ReturnedQueue);
            }
            else if (radioButton2.Checked) //New files
            {
                DisplayQueues(ReturnedQueue.Where(x => x.isNewFile).ToList());
            }
            else if (radioButton3.Checked) //Modified files
            {
                DisplayQueues(ReturnedQueue.Where(x => x.Active && !x.isNewFile).ToList());
            }
            else if (radioButton4.Checked) //Not modified files
            {
                DisplayQueues(ReturnedQueue.Where(x => !x.Active).ToList());
            }
        }

     
        /// <summary>
        /// Вывод очереди в таблицу datagridview
        /// </summary>
        /// <param name="queues"></param>
        private void DisplayQueues(List<Queue> queues)
        {
            try
            {
                dgv.Rows.Clear();
                foreach (var q in queues)
                {
                    int i = dgv.Rows.Add();

                    dgv.Rows[i].Cells["Number"].Value = q.Number;
                    dgv.Rows[i].Cells["Source"].Value = q.SourceFile;
                    dgv.Rows[i].Cells["Target"].Value = q.TargetFile;
                    dgv.Rows[i].Cells["DateSource"].Value = q.DateSource;
                    dgv.Rows[i].Cells["DateTarget"].Value = q.DateTarget;
                    dgv.Rows[i].Cells["Arrow"].Value = "-->";
                    var defstyle = dgv.Rows[i].DefaultCellStyle;
                    if (q.isNewFile) //меняем цвето строки если файл новый
                    {

                        defstyle.BackColor = Color.LightGreen;

                    }
                    else if ((q.DateSource - q.DateTarget).TotalSeconds < 3)//разница времени файлов меньше 3 секунд
                    {
                        defstyle.BackColor = Color.LightSkyBlue;
                        q.Active = false;                           //если разница во времени <3c, то не отмечаем на копирование, скорее всего погрешность времени 
                    }
                    dgv.Rows[i].DefaultCellStyle = defstyle;
                    dgv.Rows[i].Cells["check"].Value = q.Active;

                }
                toolStripStatusLabel1.Text = $"Total: {queues.Count()} files. \t\t New {queues.Count(c => c.isNewFile)} files";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Display queues");
            }
        }
        /// <summary>
        /// Select row in datagridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Button OK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOK_Click(object sender, EventArgs e)
        {
            try
            {
                ////add exceptions
                //if (AddExceptions)
                //{
                //    foreach (Queue q in ExceptionsList)
                //    {
                //        _ = ReturnedQueue.Remove(q);
                //    }
                //}
                //foreach (DataGridViewRow row in dgv.Rows) //Удаляем из очереди файлы, которые не отмечены
                //{
                //    if (!(bool)row.Cells["check"].Value)
                //    {
                //        Queue q = ReturnedQueue.Find(x => x.Number == (int)row.Cells["Number"].Value);
                //        if (q != null) ReturnedQueue.Remove(q);
                //    }
                //}
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Add selected source file to ExceptionsList but not confirm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void добавитьФайлВИсключенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv.SelectedRows.Count == 0) return;
                DataGridViewRow selectedrow = dgv.SelectedRows[0];
                int Number = (int)selectedrow.Cells["Number"].Value;//number of selected queue
                Queue selectedQueue = ReturnedQueue.First(x => x.Number == Number);
                selectedQueue.Active = false;
                if (ExceptionsList.Contains(selectedQueue)) return;
                ExceptionsList.Add(selectedQueue);
                currentProject.ExceptionDirs.Add(Form1.GetRelativePath(selectedQueue.SourceFile, selectedQueue.SourceFileProjectDir));
                AddExceptions = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Add exception");
            }
        }

        private void отменитьДобавленныеИсключенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv.SelectedRows.Count == 0) return;
                DataGridViewRow selectedrow = dgv.SelectedRows[0];
                int Number = (int)selectedrow.Cells["Number"].Value;//number of selected queue
                Queue selectedQueue = ReturnedQueue.First(x => x.Number == Number);
                if (!ExceptionsList.Contains(selectedQueue)) return;
                ExceptionsList.Remove(selectedQueue);
                currentProject.ExceptionDirs.Remove(Form1.GetRelativePath(selectedQueue.SourceFile, selectedQueue.SourceFileProjectDir));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "remove exception");
            }
        }

        private void поменятьМестамиИсточникИНазначениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv.SelectedRows.Count == 0) return;
                DataGridViewRow selectedrow = dgv.SelectedRows[0];
                int Number = (int)selectedrow.Cells["Number"].Value;//number of selected queue
                Queue selectedQueue = ReturnedQueue.Find(x => x.Number == Number);
                if (selectedQueue.isNewFile) return;
                string oldsourceFile = selectedQueue.SourceFile;
                string oldsourceDir = selectedQueue.SourceFileProjectDir;
                DateTime oldsourceDate = selectedQueue.DateSource;
                selectedQueue.SourceFile = selectedQueue.TargetFile;
                selectedQueue.SourceFileProjectDir = selectedQueue.TargetFileProjectDir;
                selectedQueue.DateSource = selectedQueue.DateTarget;
                selectedQueue.TargetFile = oldsourceFile;
                selectedQueue.TargetFileProjectDir = oldsourceDir;
                selectedQueue.DateTarget = oldsourceDate;
                DisplayQueues(ReturnedQueue);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Swap Target and Source");
            }
        }

        private void удалитьФайлВоВсехПапкахToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv.SelectedRows.Count == 0) return;
                DataGridViewRow selectedrow = dgv.SelectedRows[0];
                int Number = (int)selectedrow.Cells["Number"].Value;//number of selected queue
                Queue selectedQueue = ReturnedQueue.Find(x => x.Number == Number);
                System.IO.File.Delete(selectedQueue.SourceFile);
                if (!selectedQueue.isNewFile)
                {
                    System.IO.File.Delete(selectedQueue.TargetFile);
                }
                ReturnedQueue.Remove(selectedQueue);
                DisplayQueues(ReturnedQueue);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Удаление выбранного файла");
            }
        }

        private void MsgDialog_Load(object sender, EventArgs e)
        {

        }

    }
}
