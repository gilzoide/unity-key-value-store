namespace Gilzoide.KeyValueStore.Serializers
{
    public interface ITextSerializer : ISerializer
    {
        string SerializeObject<T>(T obj);
        T DeserializeObject<T>(string text);
    }
}