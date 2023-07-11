namespace Gilzoide.KeyValueStore.ObjectSerializers
{
    public interface ITextSerializer : IObjectSerializer
    {
        string SerializeObject<T>(T obj);
        T DeserializeObject<T>(string text);
    }

    public interface ITextSerializer<T> : IObjectSerializer
    {
        string SerializeObject(T obj);
        T DeserializeObject(string text);
    }
}