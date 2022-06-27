using System;

namespace NoSqliteX
{

    [AttributeUsage(AttributeTargets.Property)]

    public class NoSqliteXCryptoKeyAttribute : Attribute
    {
        public NoSqliteXCryptoKeyAttribute()
        {

        }

    }
}