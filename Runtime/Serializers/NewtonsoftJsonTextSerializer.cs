#if HAVE_NEWTONSOFT_JSON
using Newtonsoft.Json;

namespace Gilzoide.KeyValueStore.Serializers
{
    public class NewtonsoftJsonTextSerializer : ITextSerializer
    {
        public JsonSerializerSettings JsonSettings { get; set; } = new JsonSerializerSettings();

        public T DeserializeObject<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text, JsonSettings);
        }

        public string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, JsonSettings);
        }
    }
}
#endif