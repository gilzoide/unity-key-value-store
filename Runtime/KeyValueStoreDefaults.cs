using Gilzoide.KeyValueStore.ObjectSerializers;

namespace Gilzoide.KeyValueStore
{
    public static class KeyValueStoreDefaults
    {
        public static IObjectSerializer DefaultObjectSerializer { get; set; } = new JsonUtilityTextSerializer();
    }
}
