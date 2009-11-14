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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Windows.Forms;
using TaskSharp.Native;

namespace TaskSharp.Wrappers
{
    public sealed class VisibleWindow : IEquatable<VisibleWindow>, INotifyPropertyChanged
    {
        private readonly string _title;
        private readonly string _exeName;
        private readonly string _exePath;
        private readonly IntPtr _hwnd;
        private readonly Icon _icon;
        private readonly Process _process;

        public VisibleWindow(Process process)
        {
            _process = process;
            _title = process.MainWindowTitle;
            _exePath = process.MainModule.FileName;
            _exeName = Path.GetFileName(_exePath);
            _hwnd = process.MainWindowHandle;
            _icon = Icon.ExtractAssociatedIcon(_exePath);
        }

        private void Restore()
        {
            Win32.ShowWindow(Hwnd, Win32.SW.Restore);
            Win32.SetForegroundWindow(Hwnd);
        }
        private void Show()
        {
            Win32.ShowWindow(Hwnd, Win32.SW.Show);
            Win32.SetForegroundWindow(Hwnd);
        }
        private void Minimize()
        {
            Win32.ShowWindow(Hwnd, Win32.SW.Minimize);
        }
        public void ToggleMinimize()
        {
            if (IsMinimized)
                Restore();
            else if (!IsForeground)
                Show();
            else
                Minimize();
        }
        public void ShowButtonOnTaskbar(bool show)
        {
            Win32.ShowTaskbarButton(Hwnd, !show);
        }

        public void ShowSystemMenu(Point pos)
        {
            //0x313 seems to be undocumented, but works
            Win32.SendMessage(Hwnd, 0x313, IntPtr.Zero, new IntPtr(pos.X | (pos.Y << 16)));
        }
        public override bool Equals(object obj)
        {
            if (obj is VisibleWindow)
                return Equals((VisibleWindow)obj);
            return false;
        }
        public bool Equals(VisibleWindow obj)
        {
            if (obj == null)
                return false;
            if (!EqualityComparer<IntPtr>.Default.Equals(_hwnd, obj._hwnd))
                return false;
            return true;
        }
        public override int GetHashCode()
        {
            int hash = 0;
            hash ^= EqualityComparer<IntPtr>.Default.GetHashCode(_hwnd);
            return hash;
        }
        public override string ToString()
        {
            return string.Format("{0} ({1}x{2})", _title, _hwnd, Screen);
        }

        public FormWindowState WindowState
        {
            get { return Win32.GetWindowState(Hwnd); }
        }
        public bool IsForeground
        {
            get { return Win32.GetForegroundWindow() == Hwnd; }
        }
        public bool IsMinimized
        {
            get { return WindowState == FormWindowState.Minimized; }
        }
        public string Title
        {
            get { return _title; }
        }
        public string ExeName
        {
            get { return _exeName; }
        }
        public string ExePath
        {
            get { return _exePath; }
        }
        public IntPtr Hwnd
        {
            get { return _hwnd; }
        }
        public Icon Icon
        {
            get { return _icon; }
        }
        private Screen _screen;
        public Screen Screen
        {
            get { return _screen; }
            internal set
            {
                if (value != _screen)
                {
                    _screen = value;
                    FirePropertyChanged(() => Screen);
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void FirePropertyChanged<T>(Expression<Func<T>> expression)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                var lambda = expression as LambdaExpression;
                var memberExpression = lambda.Body is UnaryExpression ? ((UnaryExpression)lambda.Body).Operand as MemberExpression : lambda.Body as MemberExpression;
                propertyChanged(this, new PropertyChangedEventArgs(memberExpression.Member.Name));
            }
        }

        #endregion
    }
}
