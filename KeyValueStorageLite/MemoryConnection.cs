
using Microsoft.Data.Sqlite;

namespace KeyValueStorageLite
{
    internal class MemoryConnection : DbConnection
    {
        private static SqliteConnection? _sharedConnection;

        public MemoryConnection() : base(GetSharedConnection())
        {
        }

        private static SqliteConnection GetSharedConnection()
        {
            if (_sharedConnection == null)
            {
                _sharedConnection = new SqliteConnection("DataSource=:memory:;Cache=Shared");
                _sharedConnection.Open();
            }

            return _sharedConnection;
        }

        protected override void Dispose(bool managed)
        {
        }
    }
}
