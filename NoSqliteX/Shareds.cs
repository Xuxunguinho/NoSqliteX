using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NoSqliteX
{
    internal static class Shareds
    {
     
        public static BinaryFormatter Formatter { get; set; }
       
        public static FileStream Stream { get; set; }
      
        public static string DataRootPath { get; set; }
        public static bool CanInit = false;
        public static string FileExtension => "json";

    }
}
