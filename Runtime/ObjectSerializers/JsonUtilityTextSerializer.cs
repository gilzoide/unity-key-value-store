using System;
using UnityEngine;

namespace Gilzoide.KeyValueStore.ObjectSerializers
{
    public class JsonUtilityTextSerializer : ITextSerializer
    {
        public bool PrettyPrint { get; set; }

        public bool TryDeserializeObject<T>(string text, out T value)
        {
            try
            {
                value = JsonUtility.FromJson<T>(text);
                return true;
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
        }

        public string SerializeObject<T>(T obj)
        {
            return JsonUtility.ToJson(obj, PrettyPrint);
        }
    }
}
