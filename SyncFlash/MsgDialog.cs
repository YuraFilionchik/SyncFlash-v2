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

        public MsgDialog(List<Queue> queues)
        {
            try
            {
                InitializeComponent();
                dgv.SelectionChanged += Dgv_SelectionChanged;
                ReturnedQueue = queues;
                //Fill datagridview
                DisplayQueues(queues);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MsgDialog");
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
                label1.Text = $"Total: {queues.Count()} files. \t\t New {queues.Count(c=>c.isNewFile)} files"; 
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
                //add exceptions
                if (AddExceptions)
                {
                    var cfg =Form1.cfg; //manager of file config
                    var Projects = cfg.ReadAllProjects();
                    var pr = Projects.First(x => x.Alldirs.Any(c => c.Contains(ExceptionsList[0].SourceFileProjectDir))); //selected proj
                    foreach (Queue q in ExceptionsList)
                    {
                        string relpath = Form1.GetRelationPath(q.SourceFile, q.SourceFileProjectDir);
                        if (pr.ExceptionDirs.Contains(relpath)) continue;
                        pr.ExceptionDirs.Add(relpath);//добавление относительного пути
                        _ = ReturnedQueue.Remove(q);
                    }
                    //save changes
                    cfg.SaveProject(pr);
                }
                foreach (DataGridViewRow row in dgv.Rows) //Удаляем из очереди файлы, которые не отмечены
                {
                    if (!(bool)row.Cells["check"].Value)
                    {
                        Queue q = ReturnedQueue.Find(x => x.Number == (int)row.Cells["Number"].Value);
                        if (q != null) ReturnedQueue.Remove(q);
                    }
                }
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
                if (ExceptionsList.Contains(selectedQueue)) return;
                ExceptionsList.Add(selectedQueue);

                AddExceptions = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Add exception");
            }
        }

        private void отменитьДобавленныеИсключенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddExceptions = false;
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
    }
}
