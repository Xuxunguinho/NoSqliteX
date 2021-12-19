using System;
using System.Collections.Generic;

namespace NoSqliteX
{
    internal partial class NoSqliteXMaster
    {
        [Serializable]
        internal class TypeMaster : ITypeMaster
        {
            [NoSqliteXKey]
          
            public Type Type { get; set; }
            public List<string> TypeKeys { get; set; }
            public string TypeDistributedFolder { get; set; }
            public string TypeDistributedFileName { get; set; }
            public string TypeFileName { get; set; }
        }
    }
}