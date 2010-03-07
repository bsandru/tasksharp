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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using TaskSharp.Native;
using TaskSharp.Wrappers;
using TaskSharp.Messaging;

namespace TaskSharp
{
    public static class WindowManager
    {
        private static Timer _timer;
        private static List<VisibleWindow> _openWindows = new List<VisibleWindow>();

        public static List<VisibleWindow> OpenWindows
        {
            get { return _openWindows; }
        }

        public static void Start()
        {
            _timer = new Timer(10);
            //_timer = new Timer(1000);
            _timer.AutoReset = true;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }
        public static void Stop()
        {
            if (_timer != null)
                _timer.Stop();
            foreach (var window in _openWindows)
                window.ShowButtonOnTaskbar(true);
            _openWindows.Clear();
        }
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var activeWindow = Win32.GetForegroundWindow();
            var windows = new List<VisibleWindow>();

            var nativeWindows = NativeWindow.EnumerateWindows();
            foreach (NativeWindow wnd in nativeWindows)
            {
                var hWnd = wnd.hWnd;
                if (Win32.HasWindow(hWnd))
                {
                    var screen = Win32.GetScreenFromWindow(hWnd);

                    VisibleWindow window = _openWindows.FirstOrDefault(w => w.Hwnd == hWnd) ?? new VisibleWindow(wnd);
                    windows.Add(window);
                    window.Screen = screen;
                    window.IsForeground = window.Hwnd == activeWindow;

                    if (!_openWindows.Contains(window))
                        Mediator.Send(new WindowAddedMessage { Window = window });
                }
            }
            //TODO: remove this, mediator will notify about added stuff.
            //      removal and screen change...find a better place for it.
            if (windows.Any())
            {
                //TODO: sometimes a window gets lost in the loop above,
                //      and is removed here...next loop, its back?
                var newWindows = windows.Where(w => !_openWindows.Contains(w)).ToArray();
                var goneWindows = _openWindows.Where(w => !windows.Contains(w)).ToArray();
                foreach (var newWindow in newWindows)
                    _openWindows.Add(newWindow);
                foreach (var oldWindow in goneWindows)
                {
                    _openWindows.Remove(oldWindow);
                    Mediator.Send(new WindowRemovedMessage { Window = oldWindow });
                }
            }
        }
    }
}
