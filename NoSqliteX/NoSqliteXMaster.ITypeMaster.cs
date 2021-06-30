//Copyright (c) https://github.com/Xuxunguinho All rights reserved.

using System;
using System.Collections.Generic;

namespace NoSqliteX
{
    internal partial class NoSqliteXMaster
    {
        internal interface ITypeMaster
        { 
            Type Type { get; set; }
            List<string> TypeKeys { get; set; }
            string  TypeDistributedFolder { get; set; }
            string TypeDistributedFileName { get; set; }
            string TypeFileName { get; set; }
        }
    }
}