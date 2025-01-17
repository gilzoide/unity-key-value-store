using System;
using Gilzoide.KeyValueStore.ObjectSerializers;

namespace Gilzoide.KeyValueStore
{
    public static partial class IKeyValueStoreExtensions
    {
        public static bool TryGetObject<T>(this IKeyValueStore kvs, string key, out T value)
        {
            return TryGetObject(kvs, key, out value, ObjectSerializerMap.DefaultSerializerMap);
        }
        public static bool TryGetObject<T>(this IKeyValueStore kvs, string key, out T value, ObjectSerializerMap serializerMap)
        {
            switch (serializerMap.GetObjectSerializer<T>())
            {
                case ITextSerializer<T> specializedTextSerializer:
                    return kvs.TryGetObject(key, out value, specializedTextSerializer);
                case IBinarySerializer<T> specializedBinarySerializer:
                    return kvs.TryGetObject(key, out value, specializedBinarySerializer);
                case ITextSerializer textSerializer:
                    return kvs.TryGetObject(key, out value, textSerializer);
                case IBinarySerializer binarySerializer:
                    return kvs.TryGetObject(key, out value, binarySerializer);
                default:
                    throw new InvalidCastException("Expected default object serializer to be either ITextSerializer or IBinarySerializer type.");
            }
        }
        public static bool TryGetObject<T>(this IKeyValueStore kvs, string key, out T value, ITextSerializer serializer)
        {
            value = default;
            return kvs.TryGetString(key, out string s)
                && serializer.TryDeserializeObject(s, out value);
        }
        public static bool TryGetObject<T>(this IKeyValueStore kvs, string key, out T value, ITextSerializer<T> serializer)
        {
            value = default;
            return kvs.TryGetString(key, out string s)
                && serializer.TryDeserializeObject(s, out value);
        }
        public static bool TryGetObject<T>(this IKeyValueStore kvs, string key, out T value, IBinarySerializer serializer)
        {
            value = default;
            return kvs.TryGetBytes(key, out byte[] b)
                && serializer.TryDeserializeObject(b, out value);
        }
        public static bool TryGetObject<T>(this IKeyValueStore kvs, string key, out T value, IBinarySerializer<T> serializer)
        {
            value = default;
            return kvs.TryGetBytes(key, out byte[] b)
                && serializer.TryDeserializeObject(b, out value);
        }

        public static T GetObject<T>(this IKeyValueStore kvs, string key, T defaultValue = default)
        {
            return kvs.TryGetObject(key, out T value) ? value : defaultValue;
        }
        public static T GetObject<T>(this IKeyValueStore kvs, string key, ITextSerializer serializer, T defaultValue = default)
        {
            return kvs.TryGetObject(key, out T value, serializer) ? value : defaultValue;
        }
        public static T GetObject<T>(this IKeyValueStore kvs, string key, ITextSerializer<T> serializer, T defaultValue = default)
        {
            return kvs.TryGetObject(key, out T value, serializer) ? value : defaultValue;
        }
        public static T GetObject<T>(this IKeyValueStore kvs, string key, IBinarySerializer serializer, T defaultValue = default)
        {
            return kvs.TryGetObject(key, out T value, serializer) ? value : defaultValue;
        }
        public static T GetObject<T>(this IKeyValueStore kvs, string key, IBinarySerializer<T> serializer, T defaultValue = default)
        {
            return kvs.TryGetObject(key, out T value, serializer) ? value : defaultValue;
        }


        public static void SetObject<T>(this IKeyValueStore kvs, string key, T value)
        {
            kvs.SetObject(key, value, ObjectSerializerMap.DefaultSerializerMap);
        }
        public static void SetObject<T>(this IKeyValueStore kvs, string key, T value, ObjectSerializerMap serializerMap)
        {
            switch (serializerMap.GetObjectSerializer<T>())
            {
                case ITextSerializer<T> specializedTextSerializer:
                    kvs.SetObject(key, value, specializedTextSerializer);
                    break;
                case IBinarySerializer<T> specializedBinarySerializer:
                    kvs.SetObject(key, value, specializedBinarySerializer);
                    break;
                case ITextSerializer textSerializer:
                    kvs.SetObject(key, value, textSerializer);
                    break;
                case IBinarySerializer binarySerializer:
                    kvs.SetObject(key, value, binarySerializer);
                    break;
                default:
                    throw new InvalidCastException("Expected default object serializer to be either ITextSerializer or IBinarySerializer type.");
            }
        }
        public static void SetObject<T>(this IKeyValueStore kvs, string key, T value, ITextSerializer serializer)
        {
            kvs.SetString(key, serializer.SerializeObject(value));
        }
        public static void SetObject<T>(this IKeyValueStore kvs, string key, T value, ITextSerializer<T> serializer)
        {
            kvs.SetString(key, serializer.SerializeObject(value));
        }
        public static void SetObject<T>(this IKeyValueStore kvs, string key, T value, IBinarySerializer serializer)
        {
            kvs.SetBytes(key, serializer.SerializeObject(value));
        }
        public static void SetObject<T>(this IKeyValueStore kvs, string key, T value, IBinarySerializer<T> serializer)
        {
            kvs.SetBytes(key, serializer.SerializeObject(value));
        }
    }
}
