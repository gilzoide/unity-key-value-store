using UnityEngine;

namespace Gilzoide.KeyValueStore.ObjectSerializers
{
    public class JsonUtilityTextSerializer : ITextSerializer
    {
        public bool PrettyPrint { get; set; }

        public T DeserializeObject<T>(string text)
        {
            return JsonUtility.FromJson<T>(text);
        }

        public string SerializeObject<T>(T obj)
        {
            return JsonUtility.ToJson(obj, PrettyPrint);
        }
    }
}
