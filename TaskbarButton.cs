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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using TaskSharp.Native;
using TaskSharp.Rendering;
using TaskSharp.Wrappers;

namespace TaskSharp
{
    public class TaskbarButton : Control //Button
    {
        private bool _mouseDown;
        private bool _mouseOver;
        public VisibleWindow Window { get; private set; }
        public Icon Icon { get; set; }
        public Image Image { get; set; }

        public TaskbarButton(VisibleWindow window)
        {
            if (window == null)
                throw new ArgumentNullException("window", "window is null.");
            Window = window;
            Icon = window.Icon;
            Image = Icon.ToBitmap();
            Text = window.Title;
            Margin = Padding.Empty;

            //TODO: find a way to do this managed...
            //      it just doesnt work to call this and use
            //      a managed VisualStyleRenderer instead =/
            UxTheme.SetWindowTheme(base.Handle, "Taskband", null);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _mouseOver = true;
            Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _mouseOver = false;
            Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            _mouseDown = true;
            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            _mouseDown = false;
            Invalidate();
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Right)
                Window.ShowSystemMenu(PointToScreen(e.Location));
            else if (e.Button == MouseButtons.Left)
                Window.ToggleMinimize();
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            var buttonRenderer = GetButtonRenderer();
            if (buttonRenderer != null)
            {
                Rectangle rect = ClientRectangle;
                if (buttonRenderer.IsBackgroundPartiallyTransparent())
                    buttonRenderer.DrawParentBackground(pevent.Graphics, rect, this);
                buttonRenderer.DrawBackground(pevent.Graphics, rect);

                var imageRect = GetImageRectangle();
                buttonRenderer.DrawImage(pevent.Graphics, imageRect, Image);

                var textRect = Rectangle.Inflate(rect, 0, 0);
                int imageOffsetX = imageRect.X + imageRect.Width + 7;
                textRect.Offset(imageOffsetX, 8);
                textRect.Width -= (imageOffsetX + 8);
                buttonRenderer.DrawText(pevent.Graphics, textRect, Text);
            }
            else
                base.OnPaintBackground(pevent);
        }

        private static Rectangle GetImageRectangle()
        {
            return new Rectangle(7, 7, 16, 16);
        }
        private StyleRenderer GetButtonRenderer()
        {
            VisualStyleElement element;
            if (_mouseDown)
                element = VisualStyleElement.ToolBar.Button.Pressed;
            else if (_mouseOver)
            {
                if (Window.IsForeground)
                    element = VisualStyleElement.ToolBar.Button.HotChecked;
                else
                    element = VisualStyleElement.ToolBar.Button.Hot;
            }
            else
            {
                if (Window.IsForeground)
                    element = VisualStyleElement.ToolBar.Button.Checked;
                else
                    element = VisualStyleElement.ToolBar.Button.Normal;
            }

            if (!StyleRenderer.IsSupported)
                return new ClassicButtonStyleRenderer(element);
            return new NativeStyleRenderer(base.Handle, element);
        }
    }
}