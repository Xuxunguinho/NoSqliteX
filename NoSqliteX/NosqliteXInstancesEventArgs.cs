using System;

namespace NoSqliteX
{
    public class NosqliteXInstancesEventArgs : EventArgs
    {
        public WeakReference Item;

        public NosqliteXInstancesEventArgs(WeakReference item)
        {
            this.Item = item;
        }
    }
}
