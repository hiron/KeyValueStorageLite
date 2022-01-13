namespace KeyValueStorageLite;

public record KeyValue ( string Key, string Value );

public interface IDbConnection : IDisposable
{
    void Set(string key, string value);
    string? Get(string key);
    void Remove(string key);

    IEnumerable<KeyValue> GetAll();
    void Flush(IEnumerable<KeyValue> data);
}