using System;

namespace KeyValueStorageLite
{
    public class KeyValueCache : KeyValueStorageBase
    {
        public KeyValueCache(bool inMemory = false, string databaseName = "storage.db", string? password = null)
        : base(inMemory, databaseName, password)
        {
        }

        public IDictionary<string, string> Load()
        {
            using (var db = CreateConnection())
            {
                return db.GetAll().ToDictionary(tuple => tuple.Key, tuple => tuple.Value);
            }
        }

        public void Flush(IDictionary<string, string> data)
        {
            using (var db = CreateConnection())
            {
                db.Flush(data.Select(x => new KeyValue(x.Key, x.Value)));
            }
        }
    }
}
