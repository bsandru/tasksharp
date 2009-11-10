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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using TaskSharp.Native;
using TaskSharp.Rendering;
using TaskSharp.Wrappers;

namespace TaskSharp
{
    public partial class Taskbar : Form
    {
        //testing
        //private Color _backColor;
        //private bool _isGlassSupported = false;
        private StyleRenderer _taskBarRenderer;
        private Screen _screen;
        private Dictionary<IntPtr, TaskbarButton> _buttonMap = new Dictionary<IntPtr, TaskbarButton>();

        protected override bool ShowWithoutActivation
        {
            get { return true; } //do not steal focus
        }
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int MA_NOACTIVATE = 0x0003;
        /// <summary>
        /// Change ExStyle to set NoActivate on click (keep focus of active window instead)
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams moo = base.CreateParams;
                moo.ExStyle = WS_EX_NOACTIVATE;
                return moo;
            }
        }
        protected override void WndProc(ref Message m)
        {
            //same as WS_EX_NOACTIVATE, just when clicked
            if (m.Msg == WM_MOUSEACTIVATE)
                m.Result = (IntPtr)MA_NOACTIVATE;
            else
                base.WndProc(ref m);
        }

        public Taskbar()
        {
            InitializeComponent();
            //testing
            //if (Environment.OSVersion.Version.Major >= 6)
            //{
            //    DWM.IsCompositionEnabled(ref _isGlassSupported);
            //    _backColor = DWM.GlassColor();
            //    if (_isGlassSupported)
            //    {
            //        var marg = new DWM.MARGINS();
            //        marg.Top = 0;
            //        marg.Left = 0;
            //        marg.Right = ClientSize.Width;
            //        marg.Bottom = ClientSize.Height;
            //        DWM.ExtendFrameIntoClientArea(this.Handle, ref marg);
            //    }
            //}
        }

        private void AddButton(VisibleWindow window)
        {
            if (!_buttonMap.ContainsKey(window.Hwnd))
            {
                var btn = new TaskbarButton(window)
                {
                    Height = GetButtonHeight(),
                    Width = GetButtonWidth(),
                };
                _flp.Controls.Add(btn);
                _buttonMap.Add(window.Hwnd, btn);
            }
        }
        private void RemoveButton(VisibleWindow window)
        {
            if (_buttonMap.ContainsKey(window.Hwnd))// && !window.IsMinimized)
            {
                _flp.Controls.Remove(_buttonMap[window.Hwnd]);
                _buttonMap[window.Hwnd].Dispose();
                _buttonMap.Remove(window.Hwnd);
            }
        }

        private int GetButtonWidth()
        {
            return 160;
        }
        private int GetButtonHeight()
        {
            return 30;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //find taskbar
            Rectangle rect;
            AppBarEdge edge;
            AppBar.GetTaskBarInfo(out edge, out rect);

            //swap the sides for vertical ones
            if (edge == AppBarEdge.Left)
                edge = AppBarEdge.Right;
            else if (edge == AppBarEdge.Right)
                edge = AppBarEdge.Left;

            Screen taskBarScreen = Screen.FromRectangle(rect);
            if (taskBarScreen == null)
                throw new InvalidOperationException("Cant find screen for taskbar");
            var otherScreens = Screen.AllScreens.Where(s => !s.Equals(taskBarScreen));

            var fs = otherScreens.FirstOrDefault();
            if (fs == null)
            {
                MessageBox.Show("Uh, single monitor? makes no sense, bye");
                Close();
                return;
            }

            _screen = fs;
            _taskBarRenderer = GetRenderer(edge);

            bool isHorizontal = (edge & (AppBarEdge.Bottom | AppBarEdge.Top)) > 0;
            if (isHorizontal)
                Height = rect.Height;
            else
                Width = rect.Width;

            AppBar.Register(this, edge, fs);
            WindowManager.OpenWindows.WindowListChanged += OpenWindows_WindowListChanged;
        }

        private void InvokeOrNot(MethodInvoker method)
        {
            if (InvokeRequired)
                Invoke(method);
            else
                method();
        }
        void OpenWindows_WindowListChanged(object sender, WindowListChangedEventArgs e)
        {
            VisibleWindow changedItem = e.Window;
            if (e.ListChangedType != ListChangedType.ItemChanged &&
                (changedItem == null ||
                 changedItem.Screen == null ||
                 !changedItem.Screen.Equals(_screen)))
                return;

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    InvokeOrNot(() => AddButton(changedItem));
                    break;
                case ListChangedType.ItemChanged:
                    InvokeOrNot(() =>
                    {
                        if (changedItem.Screen.Equals(_screen))
                            AddButton(changedItem);
                        else
                            RemoveButton(changedItem);
                    });
                    break;
                case ListChangedType.ItemDeleted:
                    InvokeOrNot(() => RemoveButton(changedItem));
                    break;
                case ListChangedType.ItemMoved:
                    break;
                case ListChangedType.PropertyDescriptorAdded:
                    break;
                case ListChangedType.PropertyDescriptorChanged:
                    break;
                case ListChangedType.PropertyDescriptorDeleted:
                    break;
                case ListChangedType.Reset:
                    break;
                default:
                    break;
            }
        }

        private static StyleRenderer GetRenderer(AppBarEdge edge)
        {
            if (!StyleRenderer.IsSupported)
                return new ClassicStyleRenderer();
            var renderMap = new Dictionary<AppBarEdge, VisualStyleElement>
            {
                { AppBarEdge.Bottom, VisualStyleElement.Taskbar.BackgroundBottom.Normal },
                { AppBarEdge.Top, VisualStyleElement.Taskbar.BackgroundTop.Normal },
                { AppBarEdge.Left, VisualStyleElement.Taskbar.BackgroundLeft.Normal },
                { AppBarEdge.Right, VisualStyleElement.Taskbar.BackgroundRight.Normal },
            };
            return new ManagedStyleRenderer(renderMap[edge]);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (!DesignMode && _taskBarRenderer != null)
            {
                try
                {
                    _taskBarRenderer.DrawBackground(e.Graphics, e.ClipRectangle);
                    //if (_isGlassSupported)
                    //using (SolidBrush blackBrush = new SolidBrush(_backColor))
                    //    e.Graphics.FillRectangle(blackBrush, 0, 0, ClientSize.Width, ClientSize.Height);
                }
                catch { }
            }
            else
                base.OnPaintBackground(e);
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
