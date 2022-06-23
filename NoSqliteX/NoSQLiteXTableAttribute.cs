using System;

namespace NoSqliteX
{
    [AttributeUsage(AttributeTargets.Class)]
    public  class NoSqLiteXFileTableAttribute: Attribute
    {
        public string TableName {get;}

        public NoSqLiteXFileTableAttribute
            (string tableName)
        {
            TableName = tableName;
        }
       
    }
}

