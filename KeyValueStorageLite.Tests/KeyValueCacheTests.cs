using System.Collections.Generic;
using Xunit;

namespace KeyValueStorageLite.Tests
{
    public class KeyValueCacheTests
    {
        [Fact]
        public void FlushThanLoadRecords()
        {
            var cache = new KeyValueCache(true, "testCache1");
            var data = new Dictionary<string, string> {{"key1", "value1"}, {"key2", "value2"}};
            cache.Flush(data);
            var items = cache.Load();
            Assert.Equal("value1", items["key1"]);
            Assert.Equal("value2", items["key2"]);
        }
    }
}
