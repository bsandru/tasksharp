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
using TaskSharp.Native;

namespace TaskSharp.Wrappers
{
    public class ThreadAttach : IDisposable
    {
        private readonly bool _attached;
        private readonly IntPtr _threadId1;
        private readonly IntPtr _threadId2;
        public ThreadAttach(IntPtr threadId1, IntPtr threadId2)
        {
            _threadId1 = threadId1;
            _threadId2 = threadId2;
            if (threadId1 != threadId2)
                _attached = Win32.AttachThreadInput(threadId1, threadId2, true);
        }

        public void Dispose()
        {
            if (_attached)
                Win32.AttachThreadInput(_threadId1, _threadId2, false);
        }
    }
}
