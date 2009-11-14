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
using System.Windows.Forms;

namespace TaskSharp.Native
{
    public static class Win32
    {
        public const int GWL_STYLE = -16;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int right, int top, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
            public Rectangle ToRectangle()
            {
                return new Rectangle(Left, Top, Right - Left, Bottom - Top);
            }
            public static RECT FromRectangle(Rectangle rectangle)
            {
                if (rectangle == null)
                    return new RECT();
                return new RECT(rectangle.Left, rectangle.Right, rectangle.Top, rectangle.Bottom);
            }
            public override string ToString()
            {
                return string.Format("{{ X = {0} Y = {1} Width = {2} Height = {3} }}",
                   Left, Top, Right - Left, Bottom - Top);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWINFO
        {
            public uint cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [Flags]
        public enum WS : uint
        {
            Border = 0x800000,
            Caption = 0xc00000,
            Child = 0x40000000,
            ChildWindow = 0x40000000,
            ClipChildren = 0x2000000,
            ClipSiblings = 0x4000000,
            Disabled = 0x8000000,
            DlgFrame = 0x400000,
            Group = 0x20000,
            HScroll = 0x100000,
            Iconic = 0x20000000,
            Maximize = 0x1000000,
            MaximizeBox = 0x10000,
            Minimize = 0x20000000,
            MinimizeBox = 0x20000,
            Overlapped = 0,
            OverlappedWindow = 0xcf0000,
            Popup = 0x80000000,
            PopupWindow = 0x80880000,
            SizeBox = 0x40000,
            SysMenu = 0x80000,
            Tabstop = 0x10000,
            ThickFrame = 0x40000,
            Tiled = 0,
            TiledWindow = 0xcf0000,
            Visible = 0x10000000,
            VScroll = 0x200000,

            ExToolWindow  = 0x00000080,
            ExAppWindow = 0x00040000,
            ExNoActivate = 0x08000000,
        }

        [Flags]
        public enum SW
        {
            ForceMinimize = 11,
            Hide = 0,
            Max = 11,
            Maximize = 3,
            Minimize = 6,
            Normal = 1,
            Restore = 9,
            Show = 5,
            ShowDefault = 10,
            ShowMaximized = 3,
            ShowMinimized = 2,
            ShowMinNoActive = 7,
            ShowNA = 8,
            ShowNoActivate = 4,
            ShowNormal = 1
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern WS GetWindowLong(IntPtr hWnd, [MarshalAs(UnmanagedType.U4)] GWL nIndex);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, [MarshalAs(UnmanagedType.U4)] GWL nIndex, [MarshalAs(UnmanagedType.U4)] WS dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);
        [DllImport("user32")]
        public static extern bool BringWindowToTop(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr hwnd, SW nCmdShow);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, [MarshalAs(UnmanagedType.U4)]GW uCmd);

        public enum GWL : uint
        {
            ExStyle = 0xffffffec, //(-20),
        }
        enum GW : uint
        {
            HwndFirst = 0,
            HwndLast = 1,
            HwndNext = 2,
            HwndPrev = 3,
            Owner = 4,
            Child = 5,
            EnabledPopup = 6
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref POINT lpPoint);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }

        public static void ShowTaskbarButton(IntPtr hwnd, bool remove)
        {
            var tbl = (ITaskbarList)new TaskbarList();

            tbl.HrInit();
            if (remove)
                tbl.DeleteTab(hwnd);
            else
                tbl.AddTab(hwnd);
        }

        public static FormWindowState GetWindowState(IntPtr hWnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hWnd, ref placement);

            return (FormWindowState)(placement.showCmd - 1);
        }

        public static bool HasWindow(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                return false;
            if (!IsWindowVisible(hWnd))
                return false;
            if (GetWindow(hWnd, GW.Owner) != IntPtr.Zero)
                return false;
            if (GetParent(hWnd) != IntPtr.Zero)
                return false;

            return (GetWindowLong(hWnd, GWL.ExStyle) & WS.ExToolWindow) == 0;
        }
        public static Screen GetScreenFromWindow(IntPtr windowHandle)
        {
            WINDOWINFO windowinfo = new WINDOWINFO { cbSize = (uint)Marshal.SizeOf(typeof(WINDOWINFO)) };
            GetWindowInfo(windowHandle, ref windowinfo);
            return Screen.FromRectangle(windowinfo.rcWindow.ToRectangle());
        }
        public static bool ScreenContainsWindow(IntPtr windowHandle, Screen screen)
        {
            return GetScreenFromWindow(windowHandle).Equals(screen);
        }
    }
}