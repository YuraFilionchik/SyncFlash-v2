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
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();

        }



        public void ClearLog()
        {
            CONSTS.invokeTBClearText(textBox1);// textBox1.Clear();
        }

        public void AddLine(string text)
        {
            CONSTS.invokeTBAppendText(textBox1, text);
            //textBox1.AppendText(text + "\r\n");
        }

    }
}
