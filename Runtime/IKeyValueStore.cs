namespace Gilzoide.KeyValueStore
{
    public interface IKeyValueStore
    {
        bool HasKey(string key);
        void DeleteKey(string key);
        void DeleteAll();

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

    public static partial class IKeyValueStoreExtensions
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
    }
}
