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
using System.ComponentModel;
using TaskSharp.Wrappers;

namespace TaskSharp
{
    public class WindowBindingList : BindingList<VisibleWindow>
    {
        public event EventHandler<WindowListChangedEventArgs> WindowListChanged;

        public WindowBindingList()
            : base()
        {
        }
        public WindowBindingList(IList<VisibleWindow> list)
            : base(list)
        {
        }
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
            if (e.ListChangedType == ListChangedType.ItemChanged)
                FireWindowListChanged(new WindowListChangedEventArgs(e.ListChangedType,
                    e.NewIndex, e.PropertyDescriptor,
                    this[e.NewIndex]));
            else if (e.ListChangedType != ListChangedType.ItemDeleted)
                FireWindowListChanged(new WindowListChangedEventArgs(e,
                    e.NewIndex != -1 ? this[e.NewIndex] : null));
        }
        protected override void RemoveItem(int index)
        {
            //BindingList raises ItemDeleted after removing the item
            //http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=148506
            if (index != -1)
                FireWindowListChanged(new WindowListChangedEventArgs(ListChangedType.ItemDeleted,
                    index, null, this[index]));
            base.RemoveItem(index);
        }
        private void FireWindowListChanged(WindowListChangedEventArgs e)
        {
            var windowListChanged = WindowListChanged;
            if (windowListChanged != null)
                windowListChanged(this, e);
        }
    }
    public class WindowListChangedEventArgs : ListChangedEventArgs
    {
        public VisibleWindow Window { get; private set; }

        public WindowListChangedEventArgs(ListChangedType listChangedType, int newIndex, PropertyDescriptor propDesc, VisibleWindow window)
            : base(listChangedType, newIndex, propDesc)
        {
            Window = window;
        }
        public WindowListChangedEventArgs(ListChangedEventArgs e, VisibleWindow window)
            : this(e.ListChangedType, e.NewIndex, e.PropertyDescriptor, window)
        {
        }
    }
}
