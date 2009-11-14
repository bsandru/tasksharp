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

namespace TaskSharp
{
    public static class WindowManager
    {
        private static Timer _timer;
        private static WindowBindingList _openWindows = new WindowBindingList();

        public static WindowBindingList OpenWindows
        {
            get { return _openWindows; }
        }

        public static void Start()
        {
            _timer = new Timer(1000);
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
            var windows = new List<VisibleWindow>();
            foreach (var p in Process.GetProcesses())
            {
                var hWnd = p.MainWindowHandle;
                if (/*!string.IsNullOrEmpty(p.MainWindowTitle) &&*/ Win32.HasWindow(hWnd))
                {
                    var screen = Win32.GetScreenFromWindow(hWnd);
                    VisibleWindow window = new VisibleWindow(p);
                    windows.Add(window);
                    if (_openWindows.Contains(window))
                        window = _openWindows.First(w => w.Equals(window));
                    window.Screen = screen;
                }
            }

            if (windows.Any())
            {
                var newWindows = windows.Where(w => !_openWindows.Contains(w)).ToArray();
                var goneWindows = _openWindows.Where(w => !windows.Contains(w)).ToArray();
                foreach (var newWindow in newWindows)
                    _openWindows.Add(newWindow);
                foreach (var oldWindow in goneWindows)
                    _openWindows.Remove(oldWindow);
            }
        }
    }
}
