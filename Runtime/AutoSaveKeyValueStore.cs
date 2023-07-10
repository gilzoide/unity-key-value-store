using System;
using System.Threading.Tasks;

namespace Gilzoide.KeyValueStore
{
    public class AutoSaveKeyValueStore : ISavableKeyValueStore
    {
        public ISavableKeyValueStore WrappedKeyValueStore { get; }

        private bool isSaveScheduled = false;

        public AutoSaveKeyValueStore(ISavableKeyValueStore kvs)
        {
            if (kvs == null)
            {
                throw new ArgumentNullException(nameof(WrappedKeyValueStore));
            }
            WrappedKeyValueStore = kvs;
        }

        public bool HasKey(string key)
        {
            return WrappedKeyValueStore.HasKey(key);
        }

        public void DeleteKey(string key)
        {
            WrappedKeyValueStore.DeleteKey(key);
            Save();
        }

        public bool TryGetBool(string key, out bool value)
        {
            return WrappedKeyValueStore.TryGetBool(key, out value);
        }

        public bool TryGetBytes(string key, out byte[] value)
        {
            return WrappedKeyValueStore.TryGetBytes(key, out value);
        }

        public bool TryGetDouble(string key, out double value)
        {
            return WrappedKeyValueStore.TryGetDouble(key, out value);
        }

        public bool TryGetFloat(string key, out float value)
        {
            return WrappedKeyValueStore.TryGetFloat(key, out value);
        }

        public bool TryGetInt(string key, out int value)
        {
            return WrappedKeyValueStore.TryGetInt(key, out value);
        }

        public bool TryGetLong(string key, out long value)
        {
            return WrappedKeyValueStore.TryGetLong(key, out value);
        }

        public bool TryGetString(string key, out string value)
        {
            return WrappedKeyValueStore.TryGetString(key, out value);
        }

        public void SetBool(string key, bool value)
        {
            WrappedKeyValueStore.SetBool(key, value);
            Save();
        }

        public void SetInt(string key, int value)
        {
            WrappedKeyValueStore.SetInt(key, value);
            Save();
        }

        public void SetLong(string key, long value)
        {
            WrappedKeyValueStore.SetLong(key, value);
            Save();
        }

        public void SetFloat(string key, float value)
        {
            WrappedKeyValueStore.SetFloat(key, value);
            Save();
        }

        public void SetDouble(string key, double value)
        {
            WrappedKeyValueStore.SetDouble(key, value);
            Save();
        }

        public void SetString(string key, string value)
        {
            WrappedKeyValueStore.SetString(key, value);
            Save();
        }

        public void SetBytes(string key, byte[] value)
        {
            WrappedKeyValueStore.SetBytes(key, value);
            Save();
        }

        public async void Save()
        {
            if (isSaveScheduled)
            {
                return;
            }

            isSaveScheduled = true;
            await Task.Yield();
            try
            {
                WrappedKeyValueStore.Save();
            }
            finally
            {
                isSaveScheduled = false;
            }
        }
    }
}
