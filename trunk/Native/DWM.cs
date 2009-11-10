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
using System.Runtime.InteropServices;

namespace TaskSharp.Native
{
    public static class DWM
    {
        [DllImport("dwmapi.dll", EntryPoint = "DwmIsCompositionEnabled")]
        public static extern void IsCompositionEnabled(ref bool pfEnabled);
        [DllImport("dwmapi.dll", EntryPoint = "DwmExtendFrameIntoClientArea")]
        public static extern void ExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMargins);
        [DllImport("dwmapi.dll", EntryPoint = "DwmGetColorizationColor")]
        public static extern void GetColorizationColor(out uint color, out bool opaque);

        public static Color GlassColor()
        {
            uint num;
            bool flag;
            GetColorizationColor(out num, out flag);
            var color = UintToColor(num);
            Color color2 = Color.FromArgb(color.A, color.R, color.G, color.B);
            return color2;
        }
        internal static Color UintToColor(uint color)
        {
            byte a = (byte)(color >> 24);
            byte r = (byte)(color >> 16);
            byte g = (byte)(color >> 8);
            byte b = (byte)color;
            return Color.FromArgb(a, r, g, b);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }
    }
}
