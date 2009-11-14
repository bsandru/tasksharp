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
using System.Runtime.InteropServices;

namespace TaskSharp.Native
{
    [ComImport]
    [Guid("56fdf342-fd6d-11d0-958a-006097c9a090")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ITaskbarList
    {
        [PreserveSig]
        int HrInit();
        [PreserveSig]
        int AddTab([In] IntPtr hWnd);
        [PreserveSig]
        int DeleteTab([In] IntPtr hWnd);
        [PreserveSig]
        int ActivateTab([In] IntPtr hWnd);
        [PreserveSig]
        int SetActiveAlt([In] IntPtr hWnd);
    }
    [ComImport]
    [Guid("56fdf344-fd6d-11d0-958a-006097c9a090")]
    class TaskbarList
    { 
    }
}
