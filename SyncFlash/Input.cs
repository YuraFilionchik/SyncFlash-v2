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
    public partial class Input : Form
    {
        private string text = "";
        public string TEXT
        {
            get
            {
                return text;
            }
            set { textBox1.Text = value; }
        }
        public Input()
        {
            InitializeComponent();
            textBox1.KeyDown += TextBox1_KeyDown;
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                text = textBox1.Text;
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dr = folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK) TEXT = folderBrowserDialog1.SelectedPath;
            textBox1.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            text = textBox1.Text;
            Close();
        }
    }
}
