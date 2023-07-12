#if HAVE_NEWTONSOFT_JSON
using System;
using Newtonsoft.Json;

namespace Gilzoide.KeyValueStore.ObjectSerializers
{
    public class NewtonsoftJsonTextSerializer : ITextSerializer
    {
        public JsonSerializerSettings JsonSettings { get; set; } = new JsonSerializerSettings();

        public bool TryDeserializeObject<T>(string text, out T value)
        {
            try
            {
                value = JsonConvert.DeserializeObject<T>(text, JsonSettings);
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
            return JsonConvert.SerializeObject(obj, JsonSettings);
        }
    }
}
#endif