using System.Threading.Tasks;

namespace Gilzoide.KeyValueStore
{
    public class AutoSaveKeyValueStore : AKeyValueStoreWrapper<ISavableKeyValueStore>, ISavableKeyValueStore
    {
        private bool isSaveScheduled = false;

        public AutoSaveKeyValueStore(ISavableKeyValueStore kvs) : base(kvs)
        {
        }

        public override void DeleteKey(string key)
        {
            base.DeleteKey(key);
            Save();
        }

        public override void SetBool(string key, bool value)
        {
            base.SetBool(key, value);
            Save();
        }

        public override void SetInt(string key, int value)
        {
            base.SetInt(key, value);
            Save();
        }

        public override void SetLong(string key, long value)
        {
            base.SetLong(key, value);
            Save();
        }

        public override void SetFloat(string key, float value)
        {
            base.SetFloat(key, value);
            Save();
        }

        public override void SetDouble(string key, double value)
        {
            base.SetDouble(key, value);
            Save();
        }

        public override void SetString(string key, string value)
        {
            base.SetString(key, value);
            Save();
        }

        public override void SetBytes(string key, byte[] value)
        {
            base.SetBytes(key, value);
            Save();
        }

        public void Load()
        {
            WrappedKeyValueStore.Load();
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
