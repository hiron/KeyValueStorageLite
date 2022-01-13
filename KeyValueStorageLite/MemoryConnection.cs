
using Microsoft.Data.Sqlite;

namespace KeyValueStorageLite
{
    internal class MemoryConnection : DbConnection
    {
        private static readonly IDictionary<string, SqliteConnection> ConnectionCache =
            new Dictionary<string, SqliteConnection>();

        public MemoryConnection(string id, string connectionString) : base(GetSharedConnection(id, connectionString))
        {
        }

        private static SqliteConnection GetSharedConnection(string id, string connectionString)
        {
            if (!ConnectionCache.TryGetValue(id, out var conn))
            {
                conn = new SqliteConnection(connectionString);
                conn.Open();
                ConnectionCache.Add(id, conn);
            }

            return conn;
        }

        protected override void Dispose(bool managed)
        {
        }
    }
}
