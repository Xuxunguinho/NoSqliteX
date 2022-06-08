//Copyright (c) https://github.com/Xuxunguinho All rights reserved.

using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace NoSqliteX
{
    public class NoSqLiteXStarterParams
    {
       
        public string FilesEncryptionPassword {internal get;set;}
      
        public string DataEncryptionPassword {internal get; set;}
        
        [Obsolete]
        public FormatterAssemblyStyle AssemblyFormat { internal get; set; }
        [Obsolete]
        public FormatterTypeStyle TypeFormat { internal get; set; }
        [Obsolete]
        public StreamingContextStates StreamingContextState { internal get; set; }
    }
}
