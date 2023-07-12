using System;
using System.IO;
using System.Xml.Serialization;

namespace Gilzoide.KeyValueStore.ObjectSerializers
{
    public class XmlTextSerializer : ITextSerializer
    {
        public bool TryDeserializeObject<T>(string text, out T value)
        {
            try
            {
                using (var reader = new StringReader(text))
                {
                    value = (T) (new XmlSerializer(typeof(T)).Deserialize(reader));
                    return true;
                }
            }
            catch (Exception)
            {
                value = default;
                return false;
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
