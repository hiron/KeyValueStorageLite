using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace KeyValueStorageLite
{
    public class KeyValueStorageBase
    {
        private readonly bool _inMemory;
        private readonly string _databaseName;
        private readonly string? _password;
        private bool _createCalled;

        protected KeyValueStorageBase(bool inMemory, string databaseName, string? password)
        {
            _inMemory = inMemory;
            _databaseName = databaseName;
            _password = password;
        }

        protected IDbConnection CreateConnection()
        {
            if (_inMemory)
            {
                var builder2 = new SqliteConnectionStringBuilder
                {
                    DataSource = _databaseName,
                    Mode = SqliteOpenMode.Memory,
                    Cache = SqliteCacheMode.Private,
                    Pooling = false
                };
                var connection = new MemoryConnection(_databaseName, builder2.ToString());
                CreateTable(connection);

                return connection;
            }

            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = _databaseName,
                Mode = SqliteOpenMode.ReadWriteCreate
            };
            if (!string.IsNullOrEmpty(_password))
            {
                builder.Password = _password;
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
