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

namespace TaskSharp.Wrappers
{
    public class ManagedHdc : IDisposable
    {
        private Graphics _graphics;
        public IntPtr Hdc { get; private set; }

        private ManagedHdc(Graphics g)
        {
            _graphics = g;
            Hdc = g.GetHdc();
        }

        public static ManagedHdc FromGraphics(Graphics graphics)
        {
            return new ManagedHdc(graphics);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_graphics != null && Hdc != IntPtr.Zero)
                _graphics.ReleaseHdc(Hdc);
        }

        #endregion
    }
}
