using System.Threading.Tasks;
using Gilzoide.KeyValueStore.Utils;

namespace Gilzoide.KeyValueStore
{
    public class AutoSaveKeyValueStoreWrapper : AKeyValueStoreWrapper<ISavableKeyValueStore>, ISavableKeyValueStore
    {
        private bool isSaveScheduled = false;

        public AutoSaveKeyValueStoreWrapper(ISavableKeyValueStore kvs) : base(kvs)
        {
        }

        public override void DeleteKey(string key)
        {
            base.DeleteKey(key);
            SaveNextFrame();
        }

        public override void DeleteAll()
        {
            base.DeleteAll();
            SaveNextFrame();
        }

        public override void SetBool(string key, bool value)
        {
            base.SetBool(key, value);
            SaveNextFrame();
        }

        public override void SetInt(string key, int value)
        {
            base.SetInt(key, value);
            SaveNextFrame();
        }

        public override void SetLong(string key, long value)
        {
            base.SetLong(key, value);
            SaveNextFrame();
        }

        public override void SetFloat(string key, float value)
        {
            base.SetFloat(key, value);
            SaveNextFrame();
        }

        public override void SetDouble(string key, double value)
        {
            base.SetDouble(key, value);
            SaveNextFrame();
        }

        public override void SetString(string key, string value)
        {
            base.SetString(key, value);
            SaveNextFrame();
        }

        public override void SetBytes(string key, byte[] value)
        {
            base.SetBytes(key, value);
            SaveNextFrame();
        }

        public void Load()
        {
            WrappedKeyValueStore.Load();
        }

        public void Save()
        {
            WrappedKeyValueStore.Save();
        }

        private async void SaveNextFrame()
        {
            if (isSaveScheduled)
            {
                return;
            }

            isSaveScheduled = true;
            try
            {
                await Task.Yield();
                WrappedKeyValueStore.Save();
            }
            finally
            {
                isSaveScheduled = false;
            }
        }
    }
}
