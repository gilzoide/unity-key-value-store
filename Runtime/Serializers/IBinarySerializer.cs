namespace Gilzoide.KeyValueStore.Serializers
{
    public interface IBinarySerializer : ISerializer
    {
        byte[] SerializeObject<T>(T obj);
        T DeserializeObject<T>(byte[] bytes);
    }
}