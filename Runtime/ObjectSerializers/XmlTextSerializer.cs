using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Gilzoide.KeyValueStore.ObjectSerializers
{
    public class XmlTextSerializer : ITextSerializer
    {
        private readonly static Dictionary<Type, XmlSerializer> xmlSerializerCache = new Dictionary<Type, XmlSerializer>();
        public XmlSerializer GetCachedXmlSerializer(Type type)
        {
            if (!xmlSerializerCache.TryGetValue(type, out XmlSerializer xmlSerializer))
            {
                xmlSerializer = new XmlSerializer(type);
                xmlSerializerCache[type] = xmlSerializer;
            }
            return xmlSerializer;
        }

        public bool TryDeserializeObject<T>(string text, out T value)
        {
            try
            {
                using (var reader = new StringReader(text))
                {
                    value = (T) GetCachedXmlSerializer(typeof(T)).Deserialize(reader);
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
                GetCachedXmlSerializer(typeof(T)).Serialize(writer, obj);
                return writer.ToString();
            }
        }
    }
}
