using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gilzoide.KeyValueStore
{
    public class DictionaryKeyValueStore : ISavableKeyValueStore, IFileKeyValueStore
    {
        public string FileName { get; set; }
        public string Extension => "json";

        public DictionaryKeyValueStore() {}
        public DictionaryKeyValueStore(string fileName)
        {
            FileName = fileName;
        }

        private Dictionary<string, object> dictionary = new Dictionary<string, object>();

        public void Load()
        {
#if HAVE_NEWTONSOFT_JSON
            if (string.IsNullOrEmpty(FileName) || !File.Exists(this.GetPersistentPath()))
            {
                return;
            }

            using (var textReader = File.OpenText(this.GetPersistentPath()))
            {
                dictionary.Clear();
                new Newtonsoft.Json.JsonSerializer().Populate(textReader, dictionary);
            }
#endif
        }

        public void Save()
        {
#if HAVE_NEWTONSOFT_JSON
            if (string.IsNullOrEmpty(FileName))
            {
                return;
            }

            this.EnsureDirectoryExists();
            using (var textWriter = File.CreateText(this.GetPersistentPath()))
            {
                new Newtonsoft.Json.JsonSerializer().Serialize(textWriter, dictionary);
            }
#endif
        }

        public bool HasKey(string key)
        {
            return dictionary.ContainsKey(key);
        }

        public void DeleteKey(string key)
        {
            dictionary.Remove(key);
        }

        public bool TryGetBool(string key, out bool value)
        {
            try
            {
                if (dictionary.TryGetValue(key, out object obj))
                {
                    value = Convert.ToBoolean(obj);
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }
            catch (SystemException)
            {
                value = default;
                return false;
            }
        }

        public bool TryGetInt(string key, out int value)
        {
            try
            {
                if (dictionary.TryGetValue(key, out object obj))
                {
                    value = Convert.ToInt32(obj);
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }
            catch (SystemException)
            {
                value = default;
                return false;
            }
        }

        public bool TryGetLong(string key, out long value)
        {
            try
            {
                if (dictionary.TryGetValue(key, out object obj))
                {
                    value = Convert.ToInt64(obj);
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }
            catch (SystemException)
            {
                value = default;
                return false;
            }
        }

        public bool TryGetFloat(string key, out float value)
        {
            try
            {
                if (dictionary.TryGetValue(key, out object obj))
                {
                    value = Convert.ToSingle(obj);
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }
            catch (SystemException)
            {
                value = default;
                return false;
            }
        }

        public bool TryGetDouble(string key, out double value)
        {
            try
            {
                if (dictionary.TryGetValue(key, out object obj))
                {
                    value = Convert.ToDouble(obj);
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }
            catch (SystemException)
            {
                value = default;
                return false;
            }
        }

        public bool TryGetString(string key, out string value)
        {
            try
            {
                if (dictionary.TryGetValue(key, out object obj))
                {
                    value = obj.ToString();
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }
            catch (SystemException)
            {
                value = default;
                return false;
            }
        }

        public bool TryGetBytes(string key, out byte[] value)
        {
            try
            {
                if (TryGetString(key, out string s))
                {
                    value = Convert.FromBase64String(s);
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }
            catch (SystemException)
            {
                value = default;
                return false;
            }
        }

        public void SetBool(string key, bool value)
        {
            dictionary[key] = value;
        }

        public void SetInt(string key, int value)
        {
            dictionary[key] = value;
        }

        public void SetLong(string key, long value)
        {
            dictionary[key] = value;
        }

        public void SetFloat(string key, float value)
        {
            dictionary[key] = value;
        }

        public void SetDouble(string key, double value)
        {
            dictionary[key] = value;
        }

        public void SetString(string key, string value)
        {
            dictionary[key] = value;
        }

        public void SetBytes(string key, byte[] value)
        {
            dictionary[key] = Convert.ToBase64String(value);
        }
    }
}
