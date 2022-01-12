namespace KeyValueStorageLite;

public interface IDbConnection : IDisposable
{
    void Set(string key, string value);
    string? Get(string key);
    void Remove(string key);

    IEnumerable<Tuple<string, string>> GetAll();
}