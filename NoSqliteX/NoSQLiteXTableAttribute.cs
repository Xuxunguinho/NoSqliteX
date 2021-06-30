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
    [AttributeUsage(AttributeTargets.Property)]
    public class NoSqliteXKeyAttribute : Attribute
    {
      
        public NoSqliteXKeyAttribute
            ()
        {
            
        }

    }

}

