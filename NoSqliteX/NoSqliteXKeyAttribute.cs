using System;

namespace NoSqliteX

{
    [AttributeUsage(AttributeTargets.Property)]
    public class NoSqliteXKeyAttribute : Attribute
    {
      
        public NoSqliteXKeyAttribute
            ()
        {
            
        }

    }
}