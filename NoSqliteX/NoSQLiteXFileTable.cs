//Copyright (c) https://github.com/Xuxunguinho All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using  Newtonsoft.Json;
using static NoSqliteX.NoSqliteXMaster;

namespace NoSqliteX
{
    public abstract class NoSqliteXFileTable<T> where T : class
    {
        #region Fields
        
        public List<T> Items { get; private set; }

        private readonly TypeMaster _typeMaster;
        private FileStream _stream;
        private readonly bool _useDistributedFolder, _haveTypeMaster, _haveCustonName;
        private List<string> _keys;

        private string _fileName;
        private readonly string _customFileName;
        private readonly string _distributedFolder;

        #endregion

        #region XCrudEvents
        
        
        private readonly EventHandler<NoSqliteXTrigger<T>> _afterInsert;
        private readonly EventHandler<NoSqliteXTrigger<T>> _beforeInsert;
        private readonly EventHandler<NoSqliteXTrigger<T>> _beforeOverride;
        private readonly EventHandler<NoSqliteXTrigger<T>> _afterOverride;
        
        private readonly EventHandler<NoSqliteXTrigger<List<T>>> _afterInserts;
        private readonly EventHandler<NoSqliteXTrigger<List<T>>> _afterDeletes;
        private readonly EventHandler<NoSqliteXTrigger<List<T>>> _afterOverrides;

        private readonly EventHandler<NoSqliteXTrigger<List<T>>> _afterUpdates;

        private readonly EventHandler<NoSqliteXTrigger<List<T>>> _beforeInserts;
        private readonly EventHandler<NoSqliteXTrigger<List<T>>> _beforeOverrides;
        private readonly EventHandler<NoSqliteXTrigger<List<T>>> _beforeDeletes;
        private readonly EventHandler<NoSqliteXTrigger<List<T>>> _beforeUpdates;

        #endregion
        protected NoSqliteXFileTable(string distributedFolderName = null, string customFileName = null)
        {
            try
            {
                if (!Shareds.CanInit) return;
                _afterInsert += OnAfterInsert;
                _beforeInsert += OnBeforeInsert;
                _afterInserts += OnAfterInsert;
                _afterOverride += OnAfterOverride;
                _afterOverrides += OnAfterOverride;

                _beforeOverride += OnBeforeOverride;
                _beforeOverrides += OnBeforeOverride;

                _afterDeletes += OnAfterDelete;
                _afterUpdates += OnAfterUpdate;
                _beforeInserts += OnBeforeInsert;
                _beforeDeletes += OnBeforeDelete;
                _beforeUpdates += OnBeforeUpdate;
                var xMaster = new NoSqliteXMaster();
                _typeMaster = new TypeMaster {Type = typeof(T)};

                _stream = Shareds.Stream;
               

                _haveCustonName = !string.IsNullOrEmpty(customFileName);

                if (!string.IsNullOrEmpty(distributedFolderName))
                {
                    _useDistributedFolder = true;
                    this._distributedFolder = distributedFolderName;
                    this._customFileName = customFileName;

                    var st = _customFileName?.Replace("/", "_")?.Replace(' ', '_');
                    var dist = _distributedFolder?.Replace("/", "_")?.Replace(' ', '_');

                    if (_haveCustonName)
                        _haveTypeMaster = xMaster.Items?.Exists(x =>
                                              x.Type == typeof(T) && x.TypeDistributedFolder == dist &&
                                              x.TypeDistributedFileName == st) ?? false;
                    else
                        _haveTypeMaster = xMaster.Items?.Exists(x =>
                                              x.Type == typeof(T) && x.TypeDistributedFolder == dist) ?? false;
                }
                else
                {
                    _haveTypeMaster = xMaster.Items?.Exists(x => x.Type == typeof(T)) ?? false;
                }

                if (_haveTypeMaster)
                {
                    var st = _customFileName?.Replace("/", "_")?.Replace(' ', '_');
                    var dist = _distributedFolder?.Replace("/", "_")?.Replace(' ', '_');

                    if (_useDistributedFolder && _haveCustonName)
                        _typeMaster = xMaster.Items?.FirstOrDefault(x =>
                            x.Type == typeof(T) && x.TypeDistributedFolder == dist &&
                            x.TypeDistributedFileName == st);
                    else if (!_useDistributedFolder && _haveCustonName)
                        _typeMaster = xMaster.Items?.FirstOrDefault(x =>
                            x.Type == typeof(T) &&
                            x.TypeDistributedFileName == st);
                    else
                        _typeMaster = xMaster.Items?.FirstOrDefault(x =>
                            x.Type == typeof(T));
                }

                GetKeys();
                Create();
                if (_haveTypeMaster) return;

                xMaster.Insert(_typeMaster);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        protected NoSqliteXFileTable( string fileName = null)
        {
            try
            {
                if (!Shareds.CanInit) return;
                _afterInsert += OnAfterInsert;
                _beforeInsert += OnBeforeInsert;
                _afterInserts += OnAfterInsert;
                _afterOverride += OnAfterOverride;
                _afterOverrides += OnAfterOverride;

                _beforeOverride += OnBeforeOverride;
                _beforeOverrides += OnBeforeOverride;

                _afterDeletes += OnAfterDelete;
                _afterUpdates += OnAfterUpdate;
                _beforeInserts += OnBeforeInsert;
                _beforeDeletes += OnBeforeDelete;
                _beforeUpdates += OnBeforeUpdate;
                var xMaster = new NoSqliteXMaster();
                _typeMaster = new TypeMaster {Type = typeof(T)};

                _stream = Shareds.Stream;
               

                _haveCustonName = !string.IsNullOrEmpty(fileName);
          
                if (!string.IsNullOrEmpty(fileName))
                {
                    _customFileName = "_"+ fileName;
                    var st = _customFileName?.Replace("/", "_")?.Replace(' ', '_');
                   
                    if (_haveCustonName)
                        _haveTypeMaster = xMaster.Items?.Exists(x =>
                                              x.Type == typeof(T) &&
                                              x.TypeDistributedFileName == st) ?? false;
                    else
                        _haveTypeMaster = xMaster.Items?.Exists(x =>
                                              x.Type == typeof(T)) ?? false;
                }
               
                if (_haveTypeMaster)
                {
                    var st = _customFileName?.Replace("/", "_")?.Replace(' ', '_');
                    if (_haveCustonName)
                        _typeMaster = xMaster.Items?.FirstOrDefault(x =>
                            x.Type == typeof(T) &&
                            x.TypeDistributedFileName == st);
                    else
                        _typeMaster = xMaster.Items?.FirstOrDefault(x =>
                            x.Type == typeof(T));
                }
                else
                {
                    _typeMaster.TypeDistributedFileName= _customFileName;
                }
                GetKeys();
                Create();
                if (_haveTypeMaster) return;

                xMaster.Insert(_typeMaster);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public T this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }
        private void Create()
        {
            try
            {
                var t = this.GetType();
                var str = GetFilesFolderName();

                var filename = string.IsNullOrEmpty(str) ? t.Name.ToUpper() : str;

                if (!_haveTypeMaster)
                {
                    if (!_useDistributedFolder && !_haveCustonName)
                    {
                        _fileName = Shareds.DataRootPath +
                                    $@"\{filename}\/ Collection.{Shareds.FileExtension}";
                        _typeMaster.TypeFileName = _fileName;
                        _typeMaster.TypeDistributedFileName = _haveCustonName ? _customFileName : string.Empty;
                        _typeMaster.TypeDistributedFolder = string.Empty;
                    }
                    else if (!_useDistributedFolder && _haveCustonName)
                    {
                        _fileName = Shareds.DataRootPath +
                                    $@"\{filename}\/ {_customFileName}.{Shareds.FileExtension}";
                        _typeMaster.TypeFileName = _fileName;
                        _typeMaster.TypeDistributedFileName = _haveCustonName ? _customFileName : string.Empty;
                        _typeMaster.TypeDistributedFolder = string.Empty;
                    }
                    else
                    {
                        var st = _customFileName?.Replace("/", "_")?.Replace(' ', '_');
                        var custfilename = _haveCustonName ? st : "Collection";

                        var dist = _distributedFolder?.Replace("/", "_")?.Replace(' ', '_');

                        _fileName = Shareds.DataRootPath +
                                    $@"\{filename}\{dist}\/ {custfilename}.{Shareds.FileExtension}";

                        _typeMaster.TypeFileName = _fileName;
                        _typeMaster.TypeDistributedFileName = _haveCustonName ? custfilename : string.Empty;
                        ;
                        _typeMaster.TypeDistributedFolder = dist;
                    }
                }
                else
                {
                    _fileName = _typeMaster?.TypeFileName;
                }

                var path = _fileName?.Split('/')[0];
                if (!string.IsNullOrEmpty(path))
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                if (File.Exists(_fileName))
                {
                    GetData();
                    return;
                }

                Save();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private string GetFilesFolderName()
        {
            try
            {
                var props = this.GetType().GetCustomAttributes(false);
                foreach (Attribute a in props)
                {
                    if (a.GetType() != typeof(NoSqLiteXFileTableAttribute)) continue;
                    var name = (NoSqLiteXFileTableAttribute) a;
                    if (string.IsNullOrEmpty(name?.TableName))
                        throw new Exception("Error: cant not find a FileTableName ");
                    return name.TableName.ToUpper();
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private static bool HasAtrribute( PropertyDescriptor descriptor)
        {
            return Enumerable.Cast<object>(descriptor.Attributes).Any(x => x.GetType() == typeof(NoSqliteXKeyAttribute));
        }
        private void GetKeys()
        {
            try
            {
                var props = TypeDescriptor.GetProperties(typeof(T));
                _keys = new List<string>();
                props.ForEach<PropertyDescriptor>(n =>
                {
                    if(HasAtrribute(n))
                        _keys.Add(n.Name);
                });
                _typeMaster.TypeKeys = _keys;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private  bool Save()
        {
            try
            {
                 _stream?.Close();
                  if (_fileName.IsNullOrEmpty()) return false;
                  var  stream = File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                  var json =  JsonConvert.SerializeObject(Items);
                  var writer = new StreamWriter(stream);
                  writer.Write(json);
                  writer.Dispose();
                  writer.Close();
                  GetData();

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private List<T> GetData()
        {
            try
            {
#if DEBUG
                var stopwatch = new Stopwatch();
                stopwatch.Start();
#endif
                if (_fileName.IsNullOrEmpty()) return new List<T>();
                if (!File.Exists(_fileName)) return new List<T>();
                
                var ouText = File.ReadAllText(_fileName);
                var deserialized = JsonConvert.DeserializeObject<List<T>>(ouText);
                Items = deserialized;

                if(Items.IsNullOrEmpty())
                    Items = new List<T>();
#if DEBUG
                stopwatch.Stop();
                Console.WriteLine($"Tempo Estimado para pegar os {Items.Count} item(s): " +
                                  TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds));
#endif
                return Items;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        #region EventsInvoke

        private void OnAfterInsert(T item)
        {
            _afterInsert?.Invoke(this, new NoSqliteXTrigger<T>(item));
        }

        private void OnBeforeInsert(T item)
        {
            _beforeInsert?.Invoke(this, new NoSqliteXTrigger<T>(item));
        }

        private void OnBeforeOverride(T item)
        {
            _beforeOverride?.Invoke(this, new NoSqliteXTrigger<T>(item));
        }

        private void OnAfterOverride(T item)
        {
            _afterOverride?.Invoke(this, new NoSqliteXTrigger<T>(item));
        }

        private void OnAfterInsert(List<T> item)
        {
            _afterInserts?.Invoke(this, new NoSqliteXTrigger<List<T>>(item));
        }

        private void OnAfterUpdate(List<T> inserted, List<T> deleted)
        {
            _afterUpdates?.Invoke(this, new NoSqliteXTrigger<List<T>>(inserted, deleted));
        }

        private void OnAfterDelete(List<T> item)
        {
            _afterDeletes?.Invoke(this, new NoSqliteXTrigger<List<T>>(item));
        }

        private void OnBeforeInsert(List<T> item)
        {
            _beforeInserts?.Invoke(this, new NoSqliteXTrigger<List<T>>(item));
        }

        private void OnBeforeUpdate(List<T> item)
        {
            _beforeUpdates?.Invoke(this, new NoSqliteXTrigger<List<T>>(item));
        }

        private void OnBeforeDelete(List<T> item)
        {
            _beforeDeletes?.Invoke(this, new NoSqliteXTrigger<List<T>>(item));
        }


        private void OnBeforeOverride(List<T> item)
        {
            _beforeOverrides?.Invoke(this, new NoSqliteXTrigger<List<T>>(item));
        }

        private void OnAfterOverride(List<T> item)
        {
            _afterOverrides?.Invoke(this, new NoSqliteXTrigger<List<T>>(item));
        }

        #endregion

        #region Handling Triggers

        /// <summary>
        /// The name of the method says it all
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAfterInsert(object sender, NoSqliteXTrigger<T> e)
        {
            // execute aqui o codigo para antes de Inserir o Item [e.Inserted] 
        }

        /// <summary>
        /// The name of the method says it all
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeforeInsert(object sender, NoSqliteXTrigger<T> e)
        {
            // execute aqui o codigo para antes de inserir o Item [e.Deleted] dos registos
        }

        /// <summary>
        /// The name of the method says it all
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAfterOverride(object sender, NoSqliteXTrigger<T> e)
        {
            // execute aqui o codigo para antes de Inserir o Item [e.Inserted] 
        }


        /// <summary>
        /// The name of the method says it all
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeforeOverride(object sender, NoSqliteXTrigger<T> e)
        {
            // execute aqui o codigo para antes de inserir o Item [e.Deleted] dos registos
        }

        protected virtual void OnBeforeOverride(object sender, NoSqliteXTrigger<List<T>> e)
        {
            // execute aqui o codigo para antes de inserir o Item [e.Deleted] dos registos
        }

        /// <summary>
        /// The name of the method says it all
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAfterInsert(object sender, NoSqliteXTrigger<List<T>> e)
        {
            // 
        }

        /// <summary>
        /// The name of the method says it all
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAfterOverride(object sender, NoSqliteXTrigger<List<T>> e)
        {
            //
        }

        /// <summary>
        /// The name of the method says it all
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAfterUpdate(object sender, NoSqliteXTrigger<List<T>> e)
        {
            // execute aqui o codigo para ser executado depois de Atualizado o Item [e.Deleted,e.Inserted] 
        }

        /// <summary>
        /// The name of the method says it all
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAfterDelete(object sender, NoSqliteXTrigger<List<T>> e)
        {
            // execute aqui o codigo para ser executado depois de Atualizado o Item [e.Deleted,e.Inserted] 
        }

        /// <summary>
        /// The name of the method says it all
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeforeInsert(object sender, NoSqliteXTrigger<List<T>> e)
        {
            // execute aqui o codigo  
        }

        /// <summary>
        /// The name of the method says it all
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeforeUpdate(object sender, NoSqliteXTrigger<List<T>> e)
        {
            // execute aqui o codigo  
        }

        /// <summary>
        /// The name of the method says it all
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeforeDelete(object sender, NoSqliteXTrigger<List<T>> e)
        {
            // execute aqui o codigo  
        }

        #endregion

        #region CrudMethods

        /// <summary>
        /// Sobscrever o FileTable Inteiro e salvar apenas este item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Override(T item)
        {
            try
            {
                if (item is null) return false;
                OnBeforeOverride(item);
                Items = new List<T> {item};
                if (!Save()) return false;
                OnAfterOverride(item);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Sobscrever o FileTable Inteiro e salvar apenas estes items
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool Override(List<T> items)
        {
            try
            {
                if (items.IsNullOrEmpty()) return false;
                OnBeforeOverride(items);
                Items = items;
                if (!Save()) return false;
                OnAfterOverride(items);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Insere novo item ao FileTable
        /// </summary>
        /// <param name="item"> Item a ser Inserido</param>
        public bool Insert(T item)
        {
            try
            {
                if (item is null) return false;
                OnBeforeInsert(item);

                if (Items.IsNullOrEmpty()) Items = new List<T>();
                if (_keys.IsNullOrEmpty())
                {
                    if (Items.Contains(item)) return false;
                    Items.Add(item);
                    if (!Save()) return false;
                    OnAfterInsert(item);
                    return true;
                }
                if (Items.ContainsKey(item, _keys)) return false;
                Items.Add(item);
                if (!Save()) return false;
                OnAfterInsert(item);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Insere ou Atualiza um item na Tabela
        /// </summary>
        /// <param name="item"> Item a ser Inserido </param>
        /// <param name="setter"></param>
        /// <param name="equater"></param>
        public bool Insert(T item, Action<T, T> setter, Func<T, T, bool> equater)
        {
            try
            {
                if (item is null) return false;
                if (setter is null || equater is null)
                    throw new NullReferenceException();
                OnBeforeInsert(item);
                {
                    if (Items.IsNullOrEmpty()) Items = new List<T>();
                    if (_keys.IsNullOrEmpty())
                    {
                        if (Items.Contains(item)) return false;
                        Items.Add(item);
                        if (!Save()) return false;
                        OnAfterInsert(item);
                        return true;
                    }

                    if (Items.ContainsKey(item, _keys))
                    {
                        return Update(s => setter(s, item), s => equater(s, item));
                    }
                    Items.Add(item);
                    if (!Save()) return false;
                    OnAfterInsert(item);
                    return true;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Insere ou Atualiza um item na Tabela
        /// </summary>
        /// <param name="item"> Item a ser Inserido </param>
        /// <param name="setter"></param>
        /// <param name="equater"></param>
        public bool Insert(T item, Action<T, T> setter, Func<T, bool> equater)
        {
            try
            {
                if (item is null) return false;
                if (setter is null || equater is null)
                    throw new NullReferenceException();
                OnBeforeInsert(item);
                {
                    if (Items.IsNullOrEmpty()) Items = new List<T>();
                    if (_keys.IsNullOrEmpty())
                    {
                        if (Items.Contains(item)) return false;
                        Items.Add(item);
                        if (!Save()) return false;
                        OnAfterInsert(item);
                        return true;
                    }
                    
                    if (Items.ContainsKey(item, _keys))
                    {
                        return Update(s => setter(s, item), s => equater(s));
                    }
                    Items.Add(item);
                    if (!Save()) return false;
                    OnAfterInsert(item);
                    return true;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Insere uma Lista de  novos items ao FileTable
        /// </summary>
        /// <param name="items">Items a serem Inseridos</param>
        public bool Insert(List<T> items)
        {
            try
            {
#if DEBUG
                var stow = new Stopwatch();
                stow.Start();
#endif
                var alterFlag = new List<bool>();
                if (items.IsNullOrEmpty()) return false;
                OnBeforeInsert(items);
                {
                    if (Items.IsNullOrEmpty()) Items = new List<T>();
                    foreach (var x in items)
                    {
                        if (_keys.IsNullOrEmpty())
                        {
                            if (Items.Contains(x)) continue;
                            Items.Add(x);
                            alterFlag.Add(true);
                        }
                        else
                        {
                            if (Items.ContainsKey(x, _keys)) continue;
                            Items.Add(x);
                            alterFlag.Add(true);
                        }
                    }

                    if (!alterFlag.Contains(true)) return false;
                    if (!Save()) return false;
                    OnAfterInsert(items);
#if DEBUG
                    stow.Stop();
                    Console.WriteLine(TimeSpan.FromMilliseconds(stow.ElapsedMilliseconds).Seconds);
#endif
                    return true;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Insere  ou atualiza uma Lista os dados na tablela
        /// </summary>
        /// <param name="items">Items a serem Inseridos</param>
        /// <param name="setter"> 'TIn,TOut' metodo para setar os dados pretendidos</param>
        /// <param name="equater">'TIn,TOut' Função condicional => representa a opcao Where</param>
        /// <returns></returns>
        public bool Insert(List<T> items, Action<T, T> setter, Func<T, T, bool> equater)
        {
            try
            {
#if DEBUG
                var stow = new Stopwatch();
                stow.Start();
#endif
                var alterFlag = new List<bool>();
                if (items.IsNullOrEmpty()) return false;
                if (setter is null || equater is null)
                    throw new NullReferenceException();

                OnBeforeInsert(items);

                if (Items.IsNullOrEmpty()) Items = new List<T>();
                foreach (var x in items)
                {
                    if (_keys.IsNullOrEmpty())
                    {
                        if (Items.Contains(x)) continue;
                        Items.Add(x);
                        alterFlag.Add(true);
                    }
                    else
                    {
                        if (Items.ContainsKey(x, _keys))
                        {
                            alterFlag.Add(Update(s => setter(s, x), s => equater(s, x)));
                            continue;
                        }
                        Items.Add(x);
                        alterFlag.Add(true);
                        Console.WriteLine("Adicionando...");
                    }
                }

                if (!alterFlag.Contains(true)) return false;
                if (!Save()) return false;
                OnAfterInsert(items);

#if DEBUG
                stow.Stop();
                Console.WriteLine(TimeSpan.FromMilliseconds(stow.ElapsedMilliseconds).Seconds);
#endif
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///  Atualiza um ou mais  items especificos no FileTable
        /// </summary>
        /// <param name="setter"> metodo para setar os dados pretendidos</param>
        /// <param name="equater">Função condicional => representa a opcao Where</param>
        public bool Update(Action<T> setter, Func<T, bool> equater)
        {
            try
            {
                var data = Items?.Where(equater)?.ToList();
                //disparando a Trigger Before Update
                OnBeforeUpdate(data);
                //captura os items que serão atualizados para retornar estes na trigger Before & After Update
                var deleted = data;
                /* quado não encontra registos a ser atualizado então retorna true
                   decidi fazer isso pela experiencia que adiquiri ao longo dos anos enquanto trabalher com o MS SQLServer
                 */
                if (data.IsNullOrEmpty()) return true;
                //Setando os dados nos registos 
                Parallel.ForEach(Items.Where(equater), setter);
                //Salvando alterações nos registos
                Save();
                // disparando a Trigger AfterUpdate
                OnAfterUpdate(data, deleted);
                return !deleted.IsNullOrEmpty();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        ///  Atualiza um ou mais  items especificos no FileTable apartir de uma lista referente
        /// </summary>
        /// <param name="source"></param>
        /// <param name="setter"> metodo para setar os dados pretendidos</param>
        /// <param name="equater">Função condicional => representa a opcao Where</param>
        public bool Update(List<T> source, Action<T, T> setter, Func<T, T, bool> equater)
        {
            try
            {
                if (source.IsNullOrEmpty()) return false;
                OnBeforeUpdate(source);
                var deleteds = new List<T>();
                foreach (var s in source)
                {
                    var data = Items?.Where(x => equater(x, s));
                    //disparando a Trigger Before Update
                    if (data == null) continue;
                    {
                        var enumerable = data as T[] ?? data.ToArray();
                        if (enumerable.IsNullOrEmpty()) continue;
                        //captura os items que serão atualizados para retornar estes na trigger Before & After Update
                        deleteds.AddRange(enumerable);
                        /* quado não encontra registos a ser atualizado então retorna true
                           decidi fazer isso pela experiencia que adiquiri ao longo dos anos enquanto trabalher com o MS SQLServer
                         */
                        //Setando os dados nos registos 
                        Parallel.ForEach(Items.Where(x => equater(x, s)), x => setter(x, s));
                    }
                }
                //Salvando alterações nos registos
                Save();
                // disparando a Trigger AfterUpdate
                OnAfterUpdate(source, deleteds);
                return !deleteds.IsNullOrEmpty();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Elimina do FileTable todos items representados pela função condicional
        /// </summary>
        /// <param name="equater">Função condicional</param>
        public bool Delete(Func<T, bool> equater)
        {
            try
            {
                //captura os items que serão atualizados para retornar estes na trigger Before & After delete 
                var data = Items.Where(equater)?.ToList();
                if (data.IsNullOrEmpty()) return true;
                //disparando a Trigger Before Update
                OnBeforeDelete(data);
                // removendo os items dos registos
                Items.Remove(data);
                //Salavando alterações nos registos
                Save();
                //disparando a Trigger AfterDelete
                OnAfterDelete(data);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        #endregion
    }
}