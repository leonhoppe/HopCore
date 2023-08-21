using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using HopCore.Server.Database;
using HopCore.Shared;
using HopCore.Shared.DependencyInjection;
using Newtonsoft.Json;

namespace HopCore.Server.Core.Database {
    public sealed class DbTable<T> : IDbTable<T> {
        private readonly IDbConnection _connection;
        private readonly DatabaseContext _context;
        private readonly ILogger _logger;

        private string _addOrder;
        
        public string TableName { get; }
        public PropertyInfo PrimaryKey { get; set; }
        private string[] ForeignKeys { get; set; }

        public DbTable(in IDbConnection connection, in string tableName, in DatabaseContext context) {
            _connection = connection;
            TableName = tableName;
            _context = context;
            _logger = Dependency.Inject<ILogger>();

            if (LoadMetadata()) {
                _logger.Database($"Initialized {tableName} handler.");
            }
            else {
                _logger.Fatal($"Could not initialize {tableName} handler!");
            }
        }

        private bool LoadMetadata() {
            var props = typeof(T).GetProperties();
            _addOrder = $"({string.Join(", ", props.Select(prop => prop.Name))})";

            try {
                PrimaryKey = props
                    .SingleOrDefault(prop => prop.GetCustomAttributes()
                        .Any(attr => attr is PrimaryKeyAttribute));
            }
            catch (Exception) {
                _logger.Error($"Only one primary key is supported but {typeof(T).Name} has more!");
                return false;
            }
            
            ForeignKeys = props
                .Where(prop => prop.GetCustomAttributes()
                    .Any(attr => attr is ForeignKeyAttribute))
                .Select(prop => prop.Name).ToArray();

            if (PrimaryKey is null) {
                _logger.Error($"Database model {typeof(T).Name} has no primary key!");
                return false;
            }
            
            _logger.Database($"Metadata for {TableName} loaded.");

            return true;
        }
        
        public IEnumerator<T> GetEnumerator() {
            var reader = _connection.ExecuteReader($"SELECT * FROM {TableName}");
            var data = ReadData(reader);
            reader.Dispose();

            foreach (var row in data) {
                yield return (T)LoadData(row);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public T Get(in object key) {
            var reader = _connection.ExecuteReader($"SELECT * FROM {TableName} WHERE {PrimaryKey.Name} = @Key", new {Key = PrepareValue(key)});
            var data = ReadData(reader);
            reader.Dispose();
            
            _logger.Database($"Read entry {key} from {TableName} table.");
            return data.Count == 1 ? (T)LoadData(data[0]) : default;
        }

        public async void Add(T item) {
            var props = item.GetType().GetProperties();
            var dataString = $"({string.Join(", ", props.Select(prop => "@" + prop.Name))})";
            var parameters = new DynamicParameters();
            
            foreach (var prop in props) {
                parameters.Add("@" + prop.Name, PrepareValue(prop.GetValue(item)));
            }
            
            await _connection.ExecuteAsync($"INSERT INTO {TableName} {_addOrder} VALUES {dataString}", parameters);
            _logger.Database($"Added new entry to {TableName} table.");
        }

        public async void Update(T item) {
            var props = item.GetType().GetProperties();
            var statements = new List<string>();
            
            foreach (var info in props) {
                if (info.Name == PrimaryKey.Name) continue;
                statements.Add($"{info.Name} = {PrepareValue(info.GetValue(item))}");
            }

            await _connection.ExecuteAsync($"UPDATE {TableName} SET {string.Join(", ", statements)} WHERE {PrimaryKey.Name} = @Key", new {Key = PrepareValue(PrimaryKey.GetValue(item))});
            _logger.Database($"Updated entry in {TableName} table.");
        }

        public async void Clear() {
            await _connection.ExecuteAsync($"DELETE FROM {TableName}");
            _logger.Database($"Cleared {TableName} table.");
        }

        public bool Contains(in T item) {
            using var result = _connection.ExecuteReader($"SELECT {PrimaryKey.Name} FROM {TableName} WHERE {PrimaryKey.Name} = @Key", new {Key = PrepareValue(PrimaryKey.GetValue(item))});
            return result.Read();
        }
        
        public async Task<bool> ContainsAsync(T item) {
            using var result = await _connection.ExecuteReaderAsync($"SELECT {PrimaryKey.Name} FROM {TableName} WHERE {PrimaryKey.Name} = @Key", new {Key = PrepareValue(PrimaryKey.GetValue(item))});
            return result.Read();
        }

        public bool Remove(in T item) {
            var rows = _connection.Execute($"DELETE FROM {TableName} WHERE {PrimaryKey.Name} = @Key", new {Key = PrepareValue(PrimaryKey.GetValue(item))});
            _logger.Database($"Removed entry from {TableName} table.");
            return rows != 0;
        }
        
        public async Task<bool> RemoveAsync(T item) {
            var rows = await _connection.ExecuteAsync($"DELETE FROM {TableName} WHERE {PrimaryKey.Name} = @Key", new {Key = PrepareValue(PrimaryKey.GetValue(item))});
            _logger.Database($"Removed entry from {TableName} table.");
            return rows != 0;
        }

        public int Count {
            get {
                using var result = _connection.ExecuteReader($"SELECT {PrimaryKey.Name} FROM {TableName}");
                
                int counter = 0;
                while (result.Read()) {
                    counter++;
                }
                
                return counter;
            }
        }

        public bool IsReadOnly { get; } = false;

        public void Insert(in int index, in T item) {
            Add(item);
        }
        
        public T this[object key] {
            get => Get(key);
            set {
                _connection.Execute($"DELETE FROM {TableName} WHERE {PrimaryKey.Name} = @Key", new {Key = PrepareValue(PrimaryKey.GetValue(key))});
                Add(value);
            }
        }

        private object PrepareValue(object input) {
            if (input == null) return null;
            
            if (HopCore.Converters.ContainsKey(input.GetType())) {
                var convertor = HopCore.Converters[input.GetType()];
                input = convertor.ConvertForDatabaseInternal(input);
            }
            
            return input;
        }

        private object LoadData(in IDictionary<string, object> data, Type inType = null, string[] foreinKeys = null) {
            inType ??= typeof(T);
            foreinKeys ??= ForeignKeys;
            
            var obj = Activator.CreateInstance(inType);
            var props = obj.GetType().GetProperties();
            
            foreach (var info in props) {
                var type = info.PropertyType;
                var value = data[info.Name];
                if (value == null) continue;
                
                if (HopCore.Converters.ContainsKey(type)) {
                    var convertor = HopCore.Converters[type];
                    value = convertor.ConvertFromDatabaseInternal((string)value);
                }
                
                if (foreinKeys.Contains(info.Name)) {
                    var handlerInfo = _context.GetType().GetProperties()
                        .SingleOrDefault(prop => prop.PropertyType.GetGenericArguments()[0] == type);

                    if (handlerInfo == null) {
                        _logger.Error($"Could not find db handler for {type.Name}!");
                        continue;
                    }

                    var handler = handlerInfo.GetValue(_context);
                    var handlerProps = typeof(DbTable<>).MakeGenericType(type).GetProperties();
                    var table = handlerProps.Single(prop => prop.Name == nameof(TableName)).GetValue(handler);
                    var prim = handlerProps.Single(prop => prop.Name == nameof(PrimaryKey)).GetValue(handler) as PropertyInfo;
                    var keys = handlerProps.Single(prop => prop.Name == nameof(ForeignKeys)).GetValue(handler) as string[];
                    
                    var reader = _connection.ExecuteReader($"SELECT * FROM {table} WHERE {prim!.Name} = @Key", new {Key = PrepareValue(value)});
                    var data2 = ReadData(reader);
                    reader.Dispose();

                    if (data2.Count != 1) {
                        _logger.Error($"Database entry of foreign key {info.Name} in table {TableName} does not exist (row {value})!");
                        continue;
                    }
                    
                    var result = LoadData(data2[0], type, keys);
                    info.SetValue(obj, result);
                    
                    continue;
                }

                if (value.GetType() != type) {
                    value = Convert.ChangeType(value, type);
                }
                
                info.SetValue(obj, value);
            }

            return obj;
        }

        private List<IDictionary<string, object>> ReadData(in IDataReader reader) {
            var data = new List<IDictionary<string, object>>();

            while (reader.Read()) {
                var array = new Dictionary<string, object>();
                for (var i = 0; i < reader.FieldCount; i++) {
                    var name = reader.GetName(i);
                    var value = reader.GetValue(i);
                    
                    array.Add(name, value.GetType() != typeof(DBNull) ? value : null);
                }
                data.Add(array);
            }

            return data;
        }

        public override string ToString() {
            return JsonConvert.SerializeObject(this.ToList());
        }
    }
}