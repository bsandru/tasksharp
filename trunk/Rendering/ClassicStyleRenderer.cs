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

using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace TaskSharp.Rendering
{
    public class ClassicStyleRenderer : StyleRenderer
    {
        protected virtual Brush BackgroundColor
        {
            get { return SystemBrushes.ControlLight; }
        }
        public override void DrawBackground(Graphics graphics, Rectangle bounds)
        {
            graphics.FillRectangle(BackgroundColor, bounds);
        }

        public override void DrawParentBackground(Graphics graphics, Rectangle bounds, Control childControl)
        {
        }

        public override void DrawText(Graphics graphics, Rectangle bounds, string text)
        {
            graphics.DrawString(text, SystemFonts.MenuFont, SystemBrushes.ControlText, bounds);
        }

        public override void DrawImage(Graphics graphics, Rectangle bounds, Image image)
        {
            graphics.DrawImage(image, bounds);
        }

        public override bool IsBackgroundPartiallyTransparent()
        {
            return false;
        }

    }
    public class ClassicButtonStyleRenderer : ClassicStyleRenderer
    {
        protected override Brush BackgroundColor
        {
            get { return SystemBrushes.ControlLightLight; }
        }
        public override void DrawBackground(Graphics graphics, Rectangle bounds)
        {
            var rect = Rectangle.Inflate(bounds, -2, -2);
            graphics.FillRectangle(BackgroundColor, rect);
            ControlPaint.DrawBorder3D(graphics, rect, Border3DStyle.Raised, Border3DSide.Bottom | Border3DSide.Right);
            rect.Inflate(-1, -1);
            ControlPaint.DrawBorder3D(graphics, rect, Border3DStyle.RaisedInner, Border3DSide.Top | Border3DSide.Left);
        }
    }
}
