//Copyright (c) https://github.com/Xuxunguinho All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using NoSqliteX.Extensions;

namespace NoSqliteX
{
    [NoSqLiteXFileTable("Master")]
    internal partial class NoSqliteXMaster
    {
        public List<TypeMaster> Items { get; private set; }
        private FileStream _stream;
        private string FileName { get; set; }
        private readonly BinaryFormatter _formatter;

        public NoSqliteXMaster()
        {
            _formatter = Shareds.Formatter;
            Create();
        }


        private async void Create()
        {
            try
            {
                var t = typeof(TypeMaster);
                var str = GetFileName();

                var filename = string.IsNullOrEmpty(str) ? t.Name : str;

                FileName = Shareds.DataRootPath +
                           $@"\{filename}\/ Collection.{Shareds.FileExtension}";

                var path = FileName?.Split('/')[0];
                if (!string.IsNullOrEmpty(path))
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                if (File.Exists(FileName))
                {
                    GetData();
                    return;
                }
                var task = Save();
                task.Start();
                await task;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Gera o Nome do FileTable Abstraindo o Nome do FileTable do NoSQLiteXFileTableAttribute
        /// </summary>
        private string GetFileName()
        {
            try
            {
                var props = this.GetType().GetCustomAttributes(false);
                //foreach (var p in props)
                //{               
                // for every property loop through all attributes
                foreach (Attribute a in props)
                {
                    if (a.GetType() != typeof(NoSqLiteXFileTableAttribute)) continue;
                    var name = (NoSqLiteXFileTableAttribute) a;
                    if (string.IsNullOrEmpty(name?.TableName))
                        throw new Exception("Error: cant not find a DataTableName ");
                    return name.TableName.ToUpper();
                    ;
                }

                return string.Empty;
                //}
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// obter registos no arquivo de armazenamento
        /// </summary>
        private void GetData()
        {
            _stream?.Close();
            if (FileName.IsNullOrEmpty()) return;
            if (!File.Exists(FileName)) return;
            _stream = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            if (_stream?.Length < 1)
            {
                Items = new List<TypeMaster>();
                return;
            }
            var deserialized = _formatter.Deserialize(_stream) as string;
            if (string.IsNullOrEmpty(deserialized)) return;
            var baseConverted = Convert.FromBase64String(deserialized);
            var ms = new MemoryStream(baseConverted);
            Items = _formatter.Deserialize(ms) as List<TypeMaster>;
            _stream?.Close();
            ms?.Dispose();
        }

        /// <summary>
        ///  Usado para salvar os registos
        /// </summary>
        /// <returns></returns>
        private Task<bool> Save()
        {
            var task = new Task<bool>(() =>
            {
                try
                {
                    _stream?.Close();
                    _stream = File.Open(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    var coArray = Items.IsNullOrEmpty() ? new List<TypeMaster>().ToByteArray() : Items?.ToByteArray();
                    var base64 = Convert.ToBase64String(coArray ?? throw new InvalidOperationException());
                    _formatter.Serialize(_stream, base64);
                    _stream?.Close();
                    GetData();
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            });

            return task;
        }

        /// <summary>
        /// Insere novo item ao FileTable
        /// </summary>
        /// <param name="item"> Item a ser Inserido</param>
        /// <param name="overrideFileMode">Indica se poder Sobscrever o FileTable Inteiro e salvar apenas este item!</param>
        public async void Insert(TypeMaster item, bool overrideFileMode = false)
        {
            try
            {
                if (overrideFileMode)
                {
                    Items = new List<TypeMaster> {item};
                    var task = Save();
                    task.Start();
                    await task;
                }
                else
                {
                    if (Items.IsNullOrEmpty()) Items = new List<TypeMaster>();
                    if (Items.Any(x =>
                        x.Type == item.Type && x.TypeDistributedFolder == item.TypeDistributedFolder &&
                        x.TypeDistributedFileName == item.TypeDistributedFileName)) return;
                    Items.Add(item);
                    var task = Save();
                    task.Start();
                    await task;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}