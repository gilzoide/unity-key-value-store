namespace Gilzoide.KeyValueStore.ObjectSerializers
{
    public interface ITextSerializer : ISerializer
    {
        string SerializeObject<T>(T obj);
        T DeserializeObject<T>(string text);
    }
}