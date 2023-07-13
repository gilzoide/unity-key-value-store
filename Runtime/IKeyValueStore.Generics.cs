using System;
using Gilzoide.KeyValueStore.ObjectSerializers;

namespace Gilzoide.KeyValueStore
{
    public static partial class IKeyValueStoreExtensions
    {
        public static bool TryGetObject<T>(this IKeyValueStore kvs, string key, out T value)
        {
            switch (KeyValueStoreDefaults.DefaultObjectSerializer)
            {
                case ITextSerializer<T> specializedTextSerializer:
                    return kvs.TryGetObject<T>(specializedTextSerializer, key, out value);
                case IBinarySerializer<T> specializedBinarySerializer:
                    return kvs.TryGetObject<T>(specializedBinarySerializer, key, out value);
                case ITextSerializer textSerializer:
                    return kvs.TryGetObject<T>(textSerializer, key, out value);
                case IBinarySerializer binarySerializer:
                    return kvs.TryGetObject<T>(binarySerializer, key, out value);
                default:
                    throw new InvalidCastException("Expected default object serializer to be either ITextSerializer or IBinarySerializer type.");
            }
        }
        public static T GetObject<T>(this IKeyValueStore kvs, string key, T defaultValue = default)
        {
            return kvs.TryGetObject<T>(key, out T value) ? value : defaultValue;
        }
        public static void SetObject<T>(this IKeyValueStore kvs, string key, T obj)
        {
            switch (KeyValueStoreDefaults.DefaultObjectSerializer)
            {
                case ITextSerializer<T> specializedTextSerializer:
                    kvs.SetObject(specializedTextSerializer, key, obj);
                    break;
                case IBinarySerializer<T> specializedBinarySerializer:
                    kvs.SetObject(specializedBinarySerializer, key, obj);
                    break;
                case ITextSerializer textSerializer:
                    kvs.SetObject(textSerializer, key, obj);
                    break;
                case IBinarySerializer binarySerializer:
                    kvs.SetObject(binarySerializer, key, obj);
                    break;
                default:
                    throw new InvalidCastException("Expected default object serializer to be either ITextSerializer or IBinarySerializer type.");
            }
        }

        public static bool TryGetObject<T>(this IKeyValueStore kvs, ITextSerializer serializer, string key, out T value)
        {
            value = default;
            return kvs.TryGetString(key, out string s)
                && serializer.TryDeserializeObject<T>(s, out value);
        }
        public static T GetObject<T>(this IKeyValueStore kvs, ITextSerializer serializer, string key, T defaultValue = default)
        {
            return kvs.TryGetObject<T>(serializer, key, out T value) ? value : defaultValue;
        }
        public static void SetObject<T>(this IKeyValueStore kvs, ITextSerializer serializer, string key, T obj)
        {
            kvs.SetString(key, serializer.SerializeObject(obj));
        }

        public static bool TryGetObject<T>(this IKeyValueStore kvs, ITextSerializer<T> serializer, string key, out T value)
        {
            value = default;
            return kvs.TryGetString(key, out string s)
                && serializer.TryDeserializeObject(s, out value);
        }
        public static T GetObject<T>(this IKeyValueStore kvs, ITextSerializer<T> serializer, string key, T defaultValue = default)
        {
            return kvs.TryGetObject<T>(serializer, key, out T value) ? value : defaultValue;
        }
        public static void SetObject<T>(this IKeyValueStore kvs, ITextSerializer<T> serializer, string key, T obj)
        {
            kvs.SetString(key, serializer.SerializeObject(obj));
        }

        public static bool TryGetObject<T>(this IKeyValueStore kvs, IBinarySerializer serializer, string key, out T value)
        {
            value = default;
            return kvs.TryGetBytes(key, out byte[] b)
                && serializer.TryDeserializeObject<T>(b, out value);
        }
        public static T GetObject<T>(this IKeyValueStore kvs, IBinarySerializer serializer, string key, T defaultValue = default)
        {
            return kvs.TryGetObject<T>(serializer, key, out T value) ? value : defaultValue;
        }
        public static void SetObject<T>(this IKeyValueStore kvs, IBinarySerializer serializer, string key, T obj)
        {
            kvs.SetBytes(key, serializer.SerializeObject(obj));
        }

        public static bool TryGetObject<T>(this IKeyValueStore kvs, IBinarySerializer<T> serializer, string key, out T value)
        {
            value = default;
            return kvs.TryGetBytes(key, out byte[] b)
                && serializer.TryDeserializeObject(b, out value);
        }
        public static T GetObject<T>(this IKeyValueStore kvs, IBinarySerializer<T> serializer, string key, T defaultValue = default)
        {
            return kvs.TryGetObject<T>(serializer, key, out T value) ? value : defaultValue;
        }
        public static void SetObject<T>(this IKeyValueStore kvs, IBinarySerializer<T> serializer, string key, T obj)
        {
            kvs.SetBytes(key, serializer.SerializeObject(obj));
        }
    }
}