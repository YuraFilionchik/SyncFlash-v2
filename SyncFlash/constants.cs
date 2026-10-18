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
        private static DriveInfo[]? allDrives;
        private static string DriveLette = string.Empty;
        public static void AddNewLine(DataGridView control, string text)
        {
            if (control is null || control.IsDisposed || string.IsNullOrEmpty(text)) return;
            if (control.InvokeRequired)
            {
                if (control.IsHandleCreated)
                {
                    control.BeginInvoke(new Action<DataGridView, string>(AddNewLine), control, text);
                }
                return;
            }
            if (control.Columns.Count == 0) return;
            {
                const int maxLogRows = 2000;
                int last = control.Rows.Count - 1;
                var value = last >= 0 ? control.Rows[last].Cells["data"].Value : null;
                if (last >= 0 && value?.ToString()?.Contains("Skipped", StringComparison.Ordinal) == true)
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

                if (control.Rows.Count > maxLogRows)
                {
                    control.Rows.RemoveAt(0);
                }

                int offset = 10;
                if (control.RowCount > offset)
                {
                    try
                    {
                        control.FirstDisplayedScrollingRowIndex = Math.Max(0, control.RowCount - offset);
                    }
                    catch (InvalidOperationException) { }
                    catch (ArgumentOutOfRangeException) { }
                }

            }

        }

        public static void ClearLog(DataGridView control)
        {
            if (control is null || control.IsDisposed) return;
            if (control.InvokeRequired)
            {
                if (control.IsHandleCreated) control.BeginInvoke(new Action(() => control.Rows.Clear()));
                return;
            }
            control.Rows.Clear();
        }

        public static void EnableButton(Button bt)
        {
            SetButtonText(bt, btSyncText1);
        }

        public static void DisableButton(Button bt)
        {
            SetButtonText(bt, btSyncText2);
        }

        private static void SetButtonText(Button? bt, string text)
        {
            if (bt is null || bt.IsDisposed) return;
            if (bt.InvokeRequired)
            {
                if (bt.IsHandleCreated) bt.BeginInvoke(new Action(() => bt.Text = text));
                return;
            }
            bt.Text = text;
        }

        public static void invokeProgress(ProgressBar bar, int value)
        {
            if (bar is null || bar.IsDisposed) return;
            value = Math.Clamp(value, 0, 100);
            if (bar.InvokeRequired)
            {
                if (bar.IsHandleCreated) bar.BeginInvoke(new Action<int>(v => bar.Value = v), value);
                return;
            }
            bar.Value = value;
        }

        public static void invokeTBAppendText(TextBox tb, string text)
        {
            if (tb is null || tb.IsDisposed || string.IsNullOrEmpty(text)) return;
            if (tb.InvokeRequired)
            {
                if (tb.IsHandleCreated) tb.BeginInvoke(new Action(() => tb.AppendText(text + Environment.NewLine)));
                return;
            }
            tb.AppendText(text + Environment.NewLine);
        }

        public static void invokeTBClearText(TextBox tb)
        {
            if (tb is null || tb.IsDisposed) return;
            if (tb.InvokeRequired)
            {
                if (tb.IsHandleCreated) tb.BeginInvoke(new Action(() => tb.Clear()));
                return;
            }
            tb.Clear();
        }

        public static void invokeEnableControl(Control control, bool enabled)
        {
            if (control is null || control.IsDisposed) return;
            if (control.InvokeRequired)
            {
                if (control.IsHandleCreated) control.BeginInvoke(new Action<bool>(v => control.Enabled = v), enabled);
                return;
            }
            control.Enabled = enabled;
        }

        /// <summary>
        /// Возвращает букву первого доступного съёмного диска (например, "D:").
        /// </summary>
        public static string GetDriveLetter()
        {
            if (!string.IsNullOrWhiteSpace(DriveLette)) return DriveLette;
            RefreshDrives();
            return DriveLette;
        }

        /// <summary>
        /// Сбрасывает кеш дисков и заново ищет съёмный диск.
        /// </summary>
        public static void RefreshDrives()
        {
            DriveLette = string.Empty;
            allDrives = null;

            try
            {
                allDrives = DriveInfo.GetDrives();
                foreach (var d in allDrives)
                {
                    try
                    {
                        if (d.IsReady && d.DriveType == DriveType.Removable && d.TotalSize > 1_600_000)
                        {
                            DriveLette = d.Name.TrimEnd('\\');
                            break;
                        }
                    }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                }
            }
            catch (IOException) { }
        }

        public static void SetDriveLetter(string? letter)
        {
            DriveLette = (letter ?? string.Empty).TrimEnd('\\');
        }

        /// <summary>
        /// Собирает полный путь к папке на флешке из относительного пути.
        /// </summary>
        public static string CombineWithDriveLetter(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return string.Empty;
            if (Path.IsPathRooted(relativePath) && relativePath.Length > 1 && relativePath[1] == ':')
            {
                return relativePath;
            }

            var drive = GetDriveLetter();
            if (string.IsNullOrWhiteSpace(drive)) return relativePath;

            return Path.Combine(drive + Path.DirectorySeparatorChar, relativePath.TrimStart('\\', '/'));
        }
    }
}
