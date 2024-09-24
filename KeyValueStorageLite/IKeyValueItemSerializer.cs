namespace KeyValueStorageLite
{
    public interface IKeyValueItemSerializer
    {
        T? GetValue<T>(string? stringValue);
        string? SerializeToString(object? value);
    }
}
