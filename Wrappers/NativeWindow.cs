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
using System.Text;
using TaskSharp.Native;

namespace TaskSharp.Wrappers
{
    /// <summary>
    /// Object used to control a Windows Form.
    /// </summary>
    public class NativeWindow
    {
        private IntPtr _hWnd;
        private string _title;
        private bool _visible = true;
        private string _process;
        private bool _wasMax = false;
        private Icon _icon;

        public IntPtr hWnd
        {
            get { return _hWnd; }
        }
        public string Title
        {
            get { return _title; }
        }
        public string Process
        {
            get { return _process; }
        }
        public Icon Icon
        {
            get { return _icon; }
        }

        /// <summary>
        /// Sets this Window Object's visibility
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                //show the window
                if (value)
                {
                    if (Win32.ShowWindowAsync(_hWnd, _wasMax ? Win32.SW.ShowMaximized : Win32.SW.ShowNormal))
                        _visible = true;
                }
                //hide the window
                else
                {
                    _wasMax = Win32.IsZoomed(_hWnd);
                    if (Win32.ShowWindowAsync(_hWnd, Win32.SW.Hide))
                        _visible = false;
                }
            }
        }

        /// <summary>
        /// Constructs a NativeWindow Object
        /// </summary>
        /// <param name="title">Title Caption</param>
        /// <param name="hWnd">Handle</param>
        /// <param name="Process">Owning Process</param>
        public NativeWindow(string title, IntPtr hWnd, string process, Icon icon)
        {
            _title = title;
            _hWnd = hWnd;
            _process = process;
            _icon = icon;
        }

        public override string ToString()
        {
            //return the title if it has one, if not return the process name
            return !string.IsNullOrEmpty(_title) ? _title : _process;
        }

        /// <summary>
        /// Sets focus to this Window Object
        /// </summary>
        public void Activate()
        {
            IntPtr foregroundHwnd = Win32.GetForegroundWindow();
            if (_hWnd == foregroundHwnd)
                return;

            IntPtr threadId1 = Win32.GetWindowThreadProcessId(foregroundHwnd, IntPtr.Zero);
            IntPtr threadId2 = Win32.GetWindowThreadProcessId(_hWnd, IntPtr.Zero);

            using (new ThreadAttach(threadId1, threadId2))
                Win32.SetForegroundWindow(_hWnd);

            Win32.ShowWindowAsync(_hWnd, Win32.IsIconic(_hWnd) ? Win32.SW.Restore : Win32.SW.ShowNormal);
        }

        #region Native
        //TODO: hide native code somewhere else, and apply security demands
        [DllImport("user32.dll")]
        private static extern int EnumWindows(EnumWindowsProc ewp, int lParam);
        //delegate used for EnumWindows() callback function
        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        private static List<NativeWindow> _wndList = new List<NativeWindow>();
        private static bool _enumerating;

        /// <summary>
        /// Collection Constructor with additional options
        /// </summary>
        public static IEnumerable<NativeWindow> EnumerateWindows()
        {
            try
            {
                if (!_enumerating)
                {
                    _enumerating = true;
                    _wndList = new List<NativeWindow>();

                    //Declare a callback delegate for EnumWindows() API call
                    EnumWindowsProc ewp = new EnumWindowsProc(EvalWindow);
                    //Enumerate all Windows
                    EnumWindows(ewp, 0);

                }
            }
            finally
            {
                _enumerating = false;
            }
            return _wndList.ToArray();
        }

        //EnumWindows CALLBACK function
        private static bool EvalWindow(IntPtr hWnd, int lParam)
        {
            const bool includeInvisible = false;
            const bool includeUntitled = false;

            if (!includeInvisible && !Win32.IsWindowVisible(hWnd))
                return true;

            StringBuilder title = new StringBuilder(256);
            StringBuilder module = new StringBuilder(256);

            Win32.GetWindowModuleFileName(hWnd, module, title.Capacity);
            Win32.GetWindowText(hWnd, title, module.Capacity);

            if (!includeUntitled && title.Length == 0)
                return true;

            _wndList.Add(new NativeWindow(title.ToString(), hWnd, module.ToString(), Win32.GetApplicationIcon(hWnd)));

            return true;
        }
        #endregion
    }
}
