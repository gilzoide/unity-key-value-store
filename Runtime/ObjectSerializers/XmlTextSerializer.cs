using System.IO;
using System.Xml.Serialization;

namespace Gilzoide.KeyValueStore.ObjectSerializers
{
    public class XmlTextSerializer : ITextSerializer
    {
        public T DeserializeObject<T>(string text)
        {
            using (var reader = new StringReader(text))
            {
                return (T) (new XmlSerializer(typeof(T)).Deserialize(reader));
            }
        }

        public string SerializeObject<T>(T obj)
        {
            using (var writer = new StringWriter())
            {
                new XmlSerializer(typeof(T)).Serialize(writer, obj);
                return writer.ToString();
            }
        }
    }
}
