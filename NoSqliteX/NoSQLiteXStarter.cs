//Copyright (c) https://github.com/Xuxunguinho All rights reserved.
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NoSqliteX
{
    public static  class NoSqLiteXStarter
    {
        
        public static void Start(NoSqLiteXStarterParams parameters,string rootSourcePath)
        {

            if (parameters is null)
                throw new Exception("Os parâmetros de Inicialização não podem ser nulos");

            if (string.IsNullOrEmpty(rootSourcePath))
                throw new Exception("O parâmetro SourcePath não  pode ser nulo ou vazio");

            Shareds.Formatter = new BinaryFormatter
            {
                AssemblyFormat = parameters.AssemblyFormat,
                TypeFormat = parameters.TypeFormat,
                Context = new System.Runtime.Serialization.StreamingContext
                (parameters.StreamingContextState)
               
            };

            Shareds.DataRootPath = rootSourcePath.ToUpper();
            Create();
        }

        private static  void Create() {

            try
            {
                if (!Directory.Exists(Shareds.DataRootPath))
                {
                    Directory.CreateDirectory(Shareds.DataRootPath);

                }
                Shareds.CanInit = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
