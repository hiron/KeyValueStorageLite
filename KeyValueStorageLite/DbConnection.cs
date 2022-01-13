using Microsoft.Data.Sqlite;

namespace KeyValueStorageLite
{
    internal class DbConnection : IDbConnection
    {
        private readonly SqliteConnection _connection;

        public DbConnection(string connectionString)
        {
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
        }

        protected DbConnection(SqliteConnection connection)
        {
            _connection = connection;
        }

        public void Set(string key, string value)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText =
                    $"INSERT OR REPLACE INTO `KeyValueItem` ( \"Key\" , \"Value\" ) VALUES ('{key}', '{value}');";
                cmd.ExecuteNonQuery();
            }
        }

        public string? Get(string key)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT ( \"Value\" ) FROM `KeyValueItem` WHERE \"Key\" = '{key}';";
                using (var reader = cmd.ExecuteReader())
                {
                    int nameIndex = reader.GetOrdinal("Value");
                    while (reader.Read())
                    {
                        var value = reader.GetString(nameIndex);
                        return value;
                    }
                }
            }
            return default;
        }

        public IEnumerable<KeyValue> GetAll()
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM `KeyValueItem`;";
                using (var reader = cmd.ExecuteReader())
                {
                    int keyIndex = reader.GetOrdinal("Key");
                    int nameIndex = reader.GetOrdinal("Value");
                    while (reader.Read())
                    {
                        var key = reader.GetString(keyIndex);
                        var value = reader.GetString(nameIndex);
                        yield return new KeyValue(key, value);
                    }
                }
            }
        }

        public void Flush(IEnumerable<KeyValue> data)
        {
            using (var cmd = _connection.CreateCommand())
            {
                foreach (var pair in data)
                {
                    cmd.CommandText =
                        $"INSERT OR REPLACE INTO `KeyValueItem` ( \"Key\" , \"Value\" ) VALUES ('{pair.Key}', '{pair.Value}');";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Remove(string key)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = $"delete from KeyValueItem where Key = '{key}'";
                cmd.ExecuteNonQuery();
            }
        }

        public void CreateStructure()
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS `KeyValueItem` ( \"Key\" varchar primary key not null , \"Value\" varchar );";
                cmd.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool managed)
        {
            if (managed)
                _connection.Dispose();

        }
    }
}
