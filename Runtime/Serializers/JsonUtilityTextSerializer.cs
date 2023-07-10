using UnityEngine;

namespace Gilzoide.KeyValueStore.Serializers
{
    public class JsonUtilityTextSerializer : ITextSerializer
    {
        public T DeserializeObject<T>(string text)
        {
            return JsonUtility.FromJson<T>(text);
        }

        public string SerializeObject<T>(T obj)
        {
            return JsonUtility.ToJson(obj);
        }
    }
}