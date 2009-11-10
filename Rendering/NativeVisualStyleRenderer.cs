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
using TaskSharp.Wrappers;

namespace TaskSharp.Rendering
{
    public class NativeStyleRenderer : StyleRenderer
    {
        private int _partId = 0;
        private int _stateId = 0;
        private IntPtr _theme = IntPtr.Zero;
        public NativeStyleRenderer(IntPtr handle, VisualStyleElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element", "element is null.");
            _theme = UxTheme.OpenTheme(handle, element.ClassName);
            _partId = element.Part;
            _stateId = element.State;
        }
        public override void DrawBackground(Graphics graphics, Rectangle bounds)
        {
            using (var mh = ManagedHdc.FromGraphics(graphics))
            {
                var rect = Win32.RECT.FromRectangle(bounds);
                UxTheme.DrawThemeBackground(_theme, mh.Hdc, _partId, _stateId, ref rect, IntPtr.Zero);
            }
        }

        public override void DrawParentBackground(Graphics graphics, Rectangle bounds, Control childControl)
        {
            using (var mh = ManagedHdc.FromGraphics(graphics))
            {
                var rect = Win32.RECT.FromRectangle(bounds);
                UxTheme.DrawThemeParentBackground(_theme, mh.Hdc, ref rect);
            }
        }

        public override bool IsBackgroundPartiallyTransparent()
        {
            return UxTheme.IsThemeBackgroundPartiallyTransparent(_theme, _partId, _stateId);
        }

        public override void DrawText(Graphics graphics, Rectangle bounds, string text)
        {
            using (var mh = ManagedHdc.FromGraphics(graphics))
            {
                var rect = Win32.RECT.FromRectangle(bounds);
                UxTheme.DrawThemeText(_theme, mh.Hdc, _partId, _stateId, text, text.Length, 0, 0, ref rect);
            }
        }
        public override void DrawImage(Graphics graphics, Rectangle bounds, Image image)
        {
            graphics.DrawImage(image, bounds);
        }
    }
}
