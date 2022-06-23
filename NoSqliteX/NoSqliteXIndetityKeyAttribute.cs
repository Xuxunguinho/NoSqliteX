using System;

namespace NoSqliteX
{

    [AttributeUsage(AttributeTargets.Property)]

    public class NoSqliteXIndetityKeyAttribute : Attribute
    {
        public NoSqliteXIndetityKeyAttribute()
        {

        }

    }
}