namespace Gilzoide.KeyValueStore.ObjectSerializers
{
    public interface IBinarySerializer : IObjectSerializer
    {
        byte[] SerializeObject<T>(T obj);
        T DeserializeObject<T>(byte[] bytes);
    }

    public interface IBinarySerializer<T> : IObjectSerializer
    {
        byte[] SerializeObject(T obj);
        T DeserializeObject(byte[] bytes);
    }
}