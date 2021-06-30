//Copyright (c) https://github.com/Xuxunguinho All rights reserved.
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace NoSqliteX
{
    public class NoSqLiteXStarterParams
    {
        public string FilesEncryptionPassword {internal get;set;}
        public string DataEncryptionPassword {internal get; set;}
        public FormatterAssemblyStyle AssemblyFormat { internal get; set; }
        public FormatterTypeStyle TypeFormat { internal get; set; }
        public StreamingContextStates StreamingContextState { internal get; set; }
    }
}
