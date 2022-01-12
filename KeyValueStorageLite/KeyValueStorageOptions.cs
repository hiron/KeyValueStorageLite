using System;

namespace KeyValueStorageLite
{
    public class KeyValueStorageOptions : IKeyValueStorageOptions
    {
        public string? DatabaseName { get; set; }
        public bool InMemory { get; set; }
        public bool Encrypted { get; set; }
        public string? Password { get; set; }
    }
}
