using System;
using System.Collections.Generic;

namespace Gilzoide.KeyValueStore.ObjectSerializers
{
    public class ObjectSerializerMap
    {
        public static ObjectSerializerMap DefaultSerializerMap { get; } = new ObjectSerializerMap();

        public IDictionary<Type, IObjectSerializer> TypeToSerializerMap { get; set; } = new Dictionary<Type, IObjectSerializer>();
        public IObjectSerializer DefaultSerializer
        {
            get => defaultSerializer;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(DefaultSerializer));
                }
                defaultSerializer = value;
            }
        }

        private IObjectSerializer defaultSerializer = new JsonUtilityTextSerializer();

        public IObjectSerializer GetObjectSerializer(Type type)
        {
            return TypeToSerializerMap.TryGetValue(type, out IObjectSerializer serializer)
                ? serializer
                : DefaultSerializer;
        }
        public IObjectSerializer GetObjectSerializer<T>()
        {
            return GetObjectSerializer(typeof(T));
        }

        public void SetObjectSerializer(Type type, IObjectSerializer serializer)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }
            TypeToSerializerMap[type] = serializer;
        }
        public void SetObjectSerializer<T>(IObjectSerializer serializer)
        {
            SetObjectSerializer(typeof(T), serializer);
        }
    }
}
