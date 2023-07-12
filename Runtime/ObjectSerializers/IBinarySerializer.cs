namespace Gilzoide.KeyValueStore.ObjectSerializers
{
    public interface IBinarySerializer : IObjectSerializer
    {
        byte[] SerializeObject<T>(T obj);
        bool TryDeserializeObject<T>(byte[] bytes, out T value);
    }

    public interface IBinarySerializer<T> : IObjectSerializer
    {
        byte[] SerializeObject(T obj);
        bool TryDeserializeObject(byte[] bytes, out T value);
    }
}