using Microsoft.Data.Sqlite;

namespace KeyValueStorageLite
{
    public class KeyValueStorage
    {
        private readonly Action<IKeyValueStorageOptions> _getOptions;
        private readonly IKeyValueItemSerializer _serializer;
        private readonly object _sync = new();
        private readonly Lazy<IDictionary<string, string>> _cache;
        private bool _createCalled;

        public KeyValueStorage(Action<IKeyValueStorageOptions> getOptions)
        {
            _getOptions = getOptions ?? throw new ArgumentNullException(nameof(getOptions));
            _serializer = new KeyValueItemSystemTextJsonSerializer();
            _cache = new Lazy<IDictionary<string, string>>(GetAll);
        }

        public T? Get<T>(string key)
        {
            ArgumentNullException.ThrowIfNull(key);
            lock (_sync)
            {
                var cache = _cache.Value;
                if (cache.TryGetValue(key, out var value))
                {
                    if (typeof(T) == typeof(string))
                        return (T)(object)value;
                    return _serializer.GetValue<T>(value);
                }

                return default;
            }
        }

        public IDictionary<string, string> GetAll()
        {
            using (var db = CreateConnection())
            {
                return db.GetAll().ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
            }

        }

        public void Set<T>(string key, T value)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(value);
            lock (_sync)
            {
                string? sValue = typeof(T) == typeof(string) ? value as string : _serializer.SerializeToString(value);
                if (sValue != null)
                {
                    _cache.Value[key] = sValue;
                    using (var db = CreateConnection())
                    {
                        db.Set(key, sValue);
                    }
                }
            }
        }

        public T? GetOrCreate<T>(string key, Func<T> create)
        {
            lock (_sync)
            {
                T? value = Get<T>(key);
                if (EqualityComparer<T>.Default.Equals(value, default))
                {
                    value = create();
                    if (value != null)
                        Set(key, value);
                }

                return value;
            }
        }

        public void Remove(string key)
        {
            ArgumentNullException.ThrowIfNull(key);
            lock (_sync)
            {
                _cache.Value.Remove(key);
                using (var db = CreateConnection())
                {
                    db.Remove(key);
                }
            }
        }

        public void Do(Action<IDbConnection> action)
        {
            using (var db = CreateConnection())
            {
                action(db);
            }
        }

        private IDbConnection CreateConnection()
        {
            var options = new KeyValueStorageOptions();
            _getOptions.Invoke(options);

            if (options.InMemory)
            {
                var connection = new MemoryConnection();
                CreateTable(connection);

                return connection;
            }

            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = options.DatabaseName,
                Mode = SqliteOpenMode.ReadWriteCreate
            };
            if (options.Encrypted && !string.IsNullOrEmpty(options.Password))
            {
                builder.Password = options.Password;
            }

            var connectionString = builder.ToString();
            var db = new DbConnection(connectionString);
            CreateTable(db);
            
            return db;
        }

        private void CreateTable(DbConnection db)
        {
            if (!_createCalled)
            {
                _createCalled = true;
                db.CreateStructure();
            }
        }
    }
}