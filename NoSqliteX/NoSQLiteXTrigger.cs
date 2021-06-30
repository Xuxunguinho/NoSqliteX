using System;

namespace NoSqliteX
{
    public class NoSqliteXTrigger<T> : EventArgs
    {
        public  T Inserted { get; }
        public T Deleted { get; }
        public NoSqliteXTrigger(T inserted,T deleted = default(T))
        {
            this.Inserted = inserted;
            this.Deleted = deleted;
        }

    }
   
}
