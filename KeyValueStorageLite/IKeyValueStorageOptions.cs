using System;

namespace KeyValueStorageLite
{
    public interface IKeyValueStorageOptions
    {
        string? DatabaseName { get; set; }

        bool InMemory { get; set; }

        bool Encrypted { get; set; }

        string? Password { get; set; }
    }
}
