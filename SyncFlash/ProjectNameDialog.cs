using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace SyncFlash
{
    public partial class ProjectNameDialog : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ProjectName { get; private set; }

        public ProjectNameDialog()
        {
            InitializeComponent();
        }

        public ProjectNameDialog(String oldName)
        {
            InitializeComponent();
            txtProjectName.Text = oldName;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProjectName.Text))
            {
                MessageBox.Show("Название проекта не может быть пустым!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ProjectName = txtProjectName.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
