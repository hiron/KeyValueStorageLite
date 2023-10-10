using Xunit;

namespace KeyValueStorageLite.Tests
{
    public class KeyValueStorageCoreTests
    {
        [Fact]
        public void SetThenReadFromDb()
        {
            var storage = new KeyValueStorage(true, "test1");
            Assert.Null(storage.Get<string>("key1"));
            storage.Set("key1", "value");
            Assert.Equal("value", storage.Get<string>("key1"));
            storage.Remove("key1");
            Assert.Null(storage.Get<string>("key11"));
            Assert.Equal("value2", storage.GetOrCreate<string>("key1", () => "value2"));
            Assert.Equal("value2", storage.Get<string>("key1"));
        }
        [Fact]
        public void ReadAllRecordsFromDb()
        {
            var storage = new KeyValueStorage(true, "test2");
            storage.Set("key1", "value1");
            storage.Set("key2", "value2");
            var items = storage.GetAll();
            Assert.Equal("value1", items["key1"]);
            Assert.Equal("value2", items["key2"]);
        }
        [Fact]
        public void FillAndGetRecordFromDb()
        {
            var storage = new KeyValueStorage(true, "test3");
            storage.Do(delegate(IDbConnection db)
            {
                db.Set("key1", "value1");
                db.Set("key2", "value2");
            });
            Assert.Equal("value1", storage.Get<string>("key1"));
        }
        [Fact]
        public void WriteStringAsObject()
        {
            var storage = new KeyValueStorage(true, "test4");
            storage.Set<object>("key1", "2.2.999.0");
            Assert.Equal("2.2.999.0", storage.Get<string>("key1"));
        }
    }
}