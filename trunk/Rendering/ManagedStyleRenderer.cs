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
    public class ManagedStyleRenderer : StyleRenderer
    {
        private VisualStyleRenderer _renderer;

        public ManagedStyleRenderer(VisualStyleElement styleElement)
        {
            _renderer = new VisualStyleRenderer(styleElement);
        }
        public ManagedStyleRenderer(string className, int part, int state)
        {
            _renderer = new VisualStyleRenderer(className, part, state);
        }

        public static bool IsElementDefined(VisualStyleElement element)
        {
            return VisualStyleRenderer.IsElementDefined(element);
        }
        public override void DrawBackground(Graphics graphics, Rectangle bounds)
        {
            _renderer.DrawBackground(graphics, bounds);
        }
        public override void DrawParentBackground(Graphics graphics, Rectangle bounds, Control childControl)
        {
            _renderer.DrawParentBackground(graphics, bounds, childControl);
        }
        public override bool IsBackgroundPartiallyTransparent()
        {
            return _renderer.IsBackgroundPartiallyTransparent();
        }
        public override void DrawText(Graphics graphics, Rectangle bounds, string text)
        {
            _renderer.DrawText(graphics, bounds, text, false, TextFormatFlags.EndEllipsis);
        }
        public override void DrawImage(Graphics graphics, Rectangle bounds, Image image)
        {
            _renderer.DrawImage(graphics, bounds, image);
        }
    }
}
