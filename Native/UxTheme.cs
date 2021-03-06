﻿//This file is part of TaskSharp.
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
using System.Runtime.InteropServices;
using System.Drawing;
using TaskSharp.Wrappers;
using System.Text;

namespace TaskSharp.Native
{
    internal class UxTheme
    {
        private static Dictionary<string, IntPtr> _themeMap = new Dictionary<string, IntPtr>();
        private static Dictionary<IntPtr, Font> _themeFontMap = new Dictionary<IntPtr, Font>();

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern IntPtr OpenThemeData(IntPtr hWnd, [MarshalAs(UnmanagedType.LPTStr)] string pszClassList);
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int CloseThemeData(IntPtr hTheme);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int DrawThemeBackground(IntPtr hTheme, IntPtr hDc, int iPartId, int iStateId, ref Win32.RECT pRect, IntPtr pClipRect);
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsThemeBackgroundPartiallyTransparent(IntPtr hTheme, int iPartId, int iStateId);
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int DrawThemeParentBackground(IntPtr hWnd, IntPtr hDc, ref Win32.RECT pRc);
        [DllImport("uxtheme", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public extern static int DrawThemeText(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int textLength, uint textFlags, uint textFlags2, ref Win32.RECT pRect);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int GetThemeFont(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, [MarshalAs(UnmanagedType.I4)] PropertyIdentifier iPropId, out LOGFONT pFont);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubAppList);

        private enum PropertyIdentifier
        {
            Font = 210, //TMT_FONT
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct LOGFONT
        {
            public int Height;
            public int Width;
            public int Escapement;
            public int Orientation;
            public int Weight;
            public byte Italic;
            public byte Underline;
            public byte StrikeOut;
            public byte Charset;
            public byte OutPrecision;
            public byte ClipPrecision;
            public byte Quality;
            public byte PitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string FaceName;
        }

        public static IntPtr OpenTheme(IntPtr handle, string className)
        {
            if (string.IsNullOrEmpty(className) || handle == IntPtr.Zero)
                return IntPtr.Zero;
            className = className.ToUpper();
            if (!_themeMap.ContainsKey(className))
                _themeMap.Add(className, OpenThemeData(handle, className));
            return _themeMap[className];
        }
        public static void CloseThemes()
        {
            foreach (var item in _themeMap)
            {
                try
                {
                    CloseThemeData(item.Value);
                }
                catch { }
            }
        }
        public static Font GetThemeFont(IntPtr theme, Graphics graphics)
        {
            if (!_themeFontMap.ContainsKey(theme))
            {
                using (var m = ManagedHdc.FromGraphics(graphics))
                {
                    LOGFONT font = new LOGFONT();
                    int ret = GetThemeFont(theme, m.Hdc, 1, 1, PropertyIdentifier.Font, out font);
                    if (ret != 0)
                        return SystemFonts.CaptionFont;
                    Font themeFont = Font.FromLogFont(font);
                    _themeFontMap.Add(theme, themeFont);
                    return themeFont;
                }
            }
            return _themeFontMap[theme];
        }
    }
}
