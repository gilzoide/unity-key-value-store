namespace Gilzoide.KeyValueStore.ObjectSerializers
{
    public interface IBinarySerializer : IObjectSerializer
    {
        byte[] SerializeObject<T>(T obj);
        T DeserializeObject<T>(byte[] bytes);
    }
}