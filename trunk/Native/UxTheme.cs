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
using System.Runtime.InteropServices;

namespace TaskSharp.Native
{
    internal class UxTheme
    {
        private static Dictionary<string, IntPtr> _themeMap = new Dictionary<string, IntPtr>();

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
        public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubAppList);

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
    }
}
