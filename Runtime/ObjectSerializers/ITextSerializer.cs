namespace Gilzoide.KeyValueStore.ObjectSerializers
{
    public interface ITextSerializer : IObjectSerializer
    {
        string SerializeObject<T>(T obj);
        T DeserializeObject<T>(string text);
    }
}