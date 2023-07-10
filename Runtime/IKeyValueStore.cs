using System;
using Gilzoide.KeyValueStore.ObjectSerializers;

namespace Gilzoide.KeyValueStore
{
    public interface IKeyValueStore
    {
        bool HasKey(string key);
        void DeleteKey(string key);

        bool TryGetBool(string key, out bool value);
        bool TryGetInt(string key, out int value);
        bool TryGetLong(string key, out long value);
        bool TryGetFloat(string key, out float value);
        bool TryGetDouble(string key, out double value);
        bool TryGetString(string key, out string value);
        bool TryGetBytes(string key, out byte[] value);

        void SetBool(string key, bool value);
        void SetInt(string key, int value);
        void SetLong(string key, long value);
        void SetFloat(string key, float value);
        void SetDouble(string key, double value);
        void SetString(string key, string value);
        void SetBytes(string key, byte[] value);
    }

    public static class IKeyValueStoreExtensions
    {
        public static bool GetBool(this IKeyValueStore kvs, string key, bool defaultValue = default)
        {
            return kvs.TryGetBool(key, out bool value) ? value : defaultValue;
        }
        public static int GetInt(this IKeyValueStore kvs, string key, int defaultValue = default)
        {
            return kvs.TryGetInt(key, out int value) ? value : defaultValue;
        }
        public static long GetLong(this IKeyValueStore kvs, string key, long defaultValue = default)
        {
            return kvs.TryGetLong(key, out long value) ? value : defaultValue;
        }
        public static float GetFloat(this IKeyValueStore kvs, string key, float defaultValue = default)
        {
            return kvs.TryGetFloat(key, out float value) ? value : defaultValue;
        }
        public static double GetDouble(this IKeyValueStore kvs, string key, double defaultValue = default)
        {
            return kvs.TryGetDouble(key, out double value) ? value : defaultValue;
        }
        public static string GetString(this IKeyValueStore kvs, string key, string defaultValue = default)
        {
            return kvs.TryGetString(key, out string value) ? value : defaultValue;
        }
        public static byte[] GetBytes(this IKeyValueStore kvs, string key, byte[] defaultValue = default)
        {
            return kvs.TryGetBytes(key, out byte[] value) ? value : defaultValue;
        }

        public static bool TryGetObject<T>(this IKeyValueStore kvs, string key, out T value)
        {
            switch (KeyValueStoreDefaults.DefaultObjectSerializer)
            {
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
            if (kvs.TryGetString(key, out string s))
            {
                value = serializer.DeserializeObject<T>(s);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        public static T GetObject<T>(this IKeyValueStore kvs, ITextSerializer serializer, string key, T defaultValue = default)
        {
            return kvs.TryGetObject<T>(serializer, key, out T value) ? value : defaultValue;
        }
        public static void SetObject<T>(this IKeyValueStore kvs, ITextSerializer serializer, string key, T obj)
        {
            kvs.SetString(key, serializer.SerializeObject(obj));
        }

        public static bool TryGetObject<T>(this IKeyValueStore kvs, IBinarySerializer serializer, string key, out T value)
        {
            if (kvs.TryGetBytes(key, out byte[] b))
            {
                value = serializer.DeserializeObject<T>(b);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        public static T GetObject<T>(this IKeyValueStore kvs, IBinarySerializer serializer, string key, T defaultValue = default)
        {
            return kvs.TryGetObject<T>(serializer, key, out T value) ? value : defaultValue;
        }
        public static void SetObject<T>(this IKeyValueStore kvs, IBinarySerializer serializer, string key, T obj)
        {
            kvs.SetBytes(key, serializer.SerializeObject(obj));
        }
    }
}
