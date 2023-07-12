namespace Gilzoide.KeyValueStore.ObjectSerializers
{
    public interface ITextSerializer : IObjectSerializer
    {
        string SerializeObject<T>(T obj);
        bool TryDeserializeObject<T>(string text, out T value);
    }

    public interface ITextSerializer<T> : IObjectSerializer
    {
        string SerializeObject(T obj);
        bool TryDeserializeObject(string text, out T value);
    }
}