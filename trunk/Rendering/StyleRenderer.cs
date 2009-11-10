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
    public abstract class StyleRenderer
    {
        public static bool IsSupported
        {
            get { return VisualStyleRenderer.IsSupported; }
        }
        public abstract void DrawBackground(Graphics graphics, Rectangle bounds);
        public abstract void DrawParentBackground(Graphics graphics, Rectangle bounds, Control childControl);
        public abstract void DrawText(Graphics graphics, Rectangle bounds, string text);
        public abstract void DrawImage(Graphics graphics, Rectangle bounds, Image image);
        public abstract bool IsBackgroundPartiallyTransparent();
    }
}
