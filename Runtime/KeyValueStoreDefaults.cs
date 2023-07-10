using Gilzoide.KeyValueStore.Serializers;

namespace Gilzoide.KeyValueStore
{
    public static class KeyValueStoreDefaults
    {
        public static ISerializer DefaultObjectSerializer { get; set; } = new JsonUtilityTextSerializer();
    }
}