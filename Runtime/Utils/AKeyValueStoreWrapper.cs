using System;

namespace Gilzoide.KeyValueStore.Utils
{
    public abstract class AKeyValueStoreWrapper<TKVS> : IKeyValueStore
        where TKVS : IKeyValueStore
    {
        public TKVS WrappedKeyValueStore { get; }

        public AKeyValueStoreWrapper(TKVS kvs)
        {
            if (kvs == null)
            {
                throw new ArgumentNullException(nameof(WrappedKeyValueStore));
            }
            WrappedKeyValueStore = kvs;
        }

        public virtual bool HasKey(string key)
        {
            return WrappedKeyValueStore.HasKey(key);
        }

        public virtual void DeleteKey(string key)
        {
            WrappedKeyValueStore.DeleteKey(key);
        }

        public virtual void DeleteAll()
        {
            WrappedKeyValueStore.DeleteAll();
        }

        public virtual bool TryGetBool(string key, out bool value)
        {
            return WrappedKeyValueStore.TryGetBool(key, out value);
        }

        public virtual bool TryGetBytes(string key, out byte[] value)
        {
            return WrappedKeyValueStore.TryGetBytes(key, out value);
        }

        public virtual bool TryGetDouble(string key, out double value)
        {
            return WrappedKeyValueStore.TryGetDouble(key, out value);
        }

        public virtual bool TryGetFloat(string key, out float value)
        {
            return WrappedKeyValueStore.TryGetFloat(key, out value);
        }

        public virtual bool TryGetInt(string key, out int value)
        {
            return WrappedKeyValueStore.TryGetInt(key, out value);
        }

        public virtual bool TryGetLong(string key, out long value)
        {
            return WrappedKeyValueStore.TryGetLong(key, out value);
        }

        public virtual bool TryGetString(string key, out string value)
        {
            return WrappedKeyValueStore.TryGetString(key, out value);
        }

        public virtual void SetBool(string key, bool value)
        {
            WrappedKeyValueStore.SetBool(key, value);
        }

        public virtual void SetInt(string key, int value)
        {
            WrappedKeyValueStore.SetInt(key, value);
        }

        public virtual void SetLong(string key, long value)
        {
            WrappedKeyValueStore.SetLong(key, value);
        }

        public virtual void SetFloat(string key, float value)
        {
            WrappedKeyValueStore.SetFloat(key, value);
        }

        public virtual void SetDouble(string key, double value)
        {
            WrappedKeyValueStore.SetDouble(key, value);
        }

        public virtual void SetString(string key, string value)
        {
            WrappedKeyValueStore.SetString(key, value);
        }

        public virtual void SetBytes(string key, byte[] value)
        {
            WrappedKeyValueStore.SetBytes(key, value);
        }
    }
}
