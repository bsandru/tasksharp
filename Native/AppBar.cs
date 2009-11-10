//This file is part of TaskSharp.
//
//TaskSharp is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//TaskSharp is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with TaskSharp.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TaskSharp.Native
{
    public static class AppBar
    {
        private static List<Form> _appBars = new List<Form>();

        public static void Register(Form form, AppBarEdge edge, Screen screen)
        {
            Rectangle screenArea = screen.WorkingArea;
            if (edge == AppBarEdge.Top || edge == AppBarEdge.Bottom)
            {
                form.Left = screenArea.Left;
                form.Width = screenArea.Width;
                form.Top = edge == AppBarEdge.Top ? screenArea.Top : screenArea.Bottom - form.Height;
            }
            else
            {
                form.Left = edge == AppBarEdge.Left ? screenArea.Left : screenArea.Right - form.Width;
                form.Height = screenArea.Height;
                form.Top = screenArea.Top;
            }
            form.FormBorderStyle = FormBorderStyle.None;
            uint callbackId = RegisterWindowMessage(form.GetHashCode().ToString("x"));

            APPBARDATA appData = new APPBARDATA();
            appData.cbSize = (uint)Marshal.SizeOf(appData);
            appData.uCallbackMessage = callbackId;
            appData.hWnd = form.Handle;

            uint ret = SHAppBarMessage(AppBarMessage.New, ref appData);
            if (ret != 0)
            {
                _appBars.Add(form);
                form.FormClosing += (s, e) => Unregister(form);
                AppBarPosition(form, edge, AppBarMessage.QueryPos);
                AppBarPosition(form, edge, AppBarMessage.SetPos);
            }
        }
        public static void UnregisterAll()
        {
            foreach (var form in _appBars.ToArray())
            {
                Unregister(form);
            }
        }

        public static void Unregister(Form form)
        {
            _appBars.Remove(form);
            if (form.IsDisposed)
                return;

            APPBARDATA appData = new APPBARDATA();
            appData.cbSize = (uint)Marshal.SizeOf(appData);
            appData.hWnd = form.Handle;
            SHAppBarMessage(AppBarMessage.Remove, ref appData);
        }
        private static void AppBarPosition(Form form, AppBarEdge edge, AppBarMessage appBarMessage)
        {
            APPBARDATA appData = new APPBARDATA();
            appData.cbSize = (uint)Marshal.SizeOf(appData);
            appData.uEdge = edge;
            appData.rc = Win32.RECT.FromRectangle(form.Bounds);
            appData.hWnd = form.Handle;

            SHAppBarMessage(appBarMessage, ref appData);
            //let the application handle some events first
            //if we dont do this (especially for SetPos)
            //the position will be off and the bar jumps around
            Application.DoEvents();
            form.Bounds = appData.rc.ToRectangle();
        }

        public static void GetTaskBarInfo(out AppBarEdge taskBarEdge, out Rectangle region)
        {
            APPBARDATA appData = new APPBARDATA();

            uint ret = SHAppBarMessage(AppBarMessage.GetTaskBarPos, ref appData);

            taskBarEdge = appData.uEdge;
            region = appData.rc.ToRectangle();
        }

        #region Native
        [DllImport("shell32.dll")]
        private static extern uint SHAppBarMessage(AppBarMessage dwMessage, ref APPBARDATA pData);
        [DllImport("user32.dll")]
        private static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPTStr)] string lpString);
        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            [MarshalAs(UnmanagedType.U4)]
            public AppBarEdge uEdge;
            public Win32.RECT rc;
            public int lParam;
        }
        private enum AppBarMessage
        {
            New = 0,
            Remove = 1,
            QueryPos = 2,
            SetPos = 3,
            GetState = 4,
            GetTaskBarPos = 5,
            Activate = 6,
            GetAutoHideBar = 7,
            SetAutoHideBar = 8,
            WindowPosChanged = 9,
            SetState = 10,
        }
        #endregion
    }
    public enum AppBarEdge
    {
        Left = 0,
        Top = 1,
        Right = 2,
        Bottom = 3
    }
}
