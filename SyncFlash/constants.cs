using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SyncFlash
{
    public static class CONSTS
    {
        public const string RootXMLProject = "Projects";
        public const string ProjXML = "Project";
        public const string DirXML = "directory";
        public const string AutoSync = "Autosync";
        public const string AttDirName = "path";
        public const string PC_XML = "NetBios";
        public const string ExceptXML = "ExceptionDir";
        public const string FlashDrive = "FLASHDRIVE";
        public const string btSyncText1 = "StartSync";
        public const string btSyncText2 = "StopSync";
        private static DriveInfo[] allDrives;
        private static string DriveLette = String.Empty;
        public static void AddNewLine(DataGridView control, string text)
        {
            if (text == null)
            {

            }
            else
            {
                int last = control.Rows.Count - 1;
                var value = last >= 0 ? control.Rows[last].Cells["data"].Value : null;
                if (last >= 0 && value != null && value.ToString().Contains("Skipped"))
                {//уже есть временная строка
                    control.Invoke(new MethodInvoker(delegate ()
                    {

                    }));
                    if (control.InvokeRequired)
                        control.Invoke(new MethodInvoker(delegate ()
                            { control.Rows[last].Cells["data"].Value = text; }));
                    else control.Rows[last].Cells["data"].Value = text;
                }
                else//создаем новую строку
                {
                    if (control.Columns.Count == 0) return;
                    int lastrow = 0;
                    if (control.InvokeRequired)
                        control.Invoke(new MethodInvoker(delegate () { lastrow = control.Rows.Add(); }));
                    else lastrow = control.Rows.Add();
                    if (control.InvokeRequired)
                        control.Invoke(new MethodInvoker(delegate () { control.Rows[lastrow].Cells["data"].Value = text; }));
                    else control.Rows[lastrow].Cells["data"].Value = text;
                }

                int offset = 10;
                if (control.RowCount > offset)
                {
                    if (control.InvokeRequired)
                        control.Invoke(new MethodInvoker(delegate ()
                            { control.FirstDisplayedScrollingRowIndex = control.RowCount - offset; }));
                    else
                        control.FirstDisplayedScrollingRowIndex = control.RowCount - offset;
                }

            }

        }

        public static void ClearLog(DataGridView control)
        {
           if (control.InvokeRequired)
              control.Invoke(new MethodInvoker(delegate ()
                        { control.Rows.Clear(); }));
           else control.Rows.Clear();
        }

        public static void EnableButton(Button bt)
        {

            if (bt.InvokeRequired) bt.Invoke(new Action<string>(s => bt.Text = (s)), CONSTS.btSyncText1);
            else bt.Text = CONSTS.btSyncText1;

        }

        public static void DisableButton(Button bt)
        {

            if (bt.InvokeRequired) bt.Invoke(new Action<string>(s => bt.Text = (s)), CONSTS.btSyncText2);
            else bt.Text = CONSTS.btSyncText2;

        }

        public static void invokeProgress(ProgressBar bar, int value)
        {
            if (value > 100) return;
            if (bar.InvokeRequired) bar.Invoke(new Action<int>(s => bar.Value = s), value);
            else bar.Value = value;
        }

        /// <summary>
        /// Get NAme of Removable drive on computer
        /// </summary>
        /// <returns>"D:"</returns>
        public static string GetDriveLetter()
        {
            if (!String.IsNullOrWhiteSpace(DriveLette)) return DriveLette;
            if (allDrives == null || allDrives.Length == 0)
            {
                allDrives = DriveInfo.GetDrives();
            }

            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady && d.DriveType == DriveType.Removable && d.TotalSize > 1600000)
                {
                    DriveLette = d.Name.TrimEnd('\\');
                }

            }
            return DriveLette;
        }
    }
}
