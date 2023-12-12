using System;
using System.Collections.Generic;

namespace Gilzoide.KeyValueStore
{
    public partial class DictionaryKeyValueStore : IKeyValueStore
    {
        protected Dictionary<string, object> _dictionary = new Dictionary<string, object>();

        public bool HasKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void DeleteKey(string key)
        {
            _dictionary.Remove(key);
        }

        public void DeleteAll()
        {
            _dictionary.Clear();
        }

        public bool TryGetBool(string key, out bool value)
        {
            try
            {
                if (_dictionary.TryGetValue(key, out object obj))
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
                if (_dictionary.TryGetValue(key, out object obj))
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
                if (_dictionary.TryGetValue(key, out object obj))
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
                if (_dictionary.TryGetValue(key, out object obj))
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
                if (_dictionary.TryGetValue(key, out object obj))
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
                if (_dictionary.TryGetValue(key, out object obj))
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
            _dictionary[key] = value;
        }

        public void SetInt(string key, int value)
        {
            _dictionary[key] = value;
        }

        public void SetLong(string key, long value)
        {
            _dictionary[key] = value;
        }

        public void SetFloat(string key, float value)
        {
            _dictionary[key] = value;
        }

        public void SetDouble(string key, double value)
        {
            _dictionary[key] = value;
        }

        public void SetString(string key, string value)
        {
            _dictionary[key] = value;
        }

        public void SetBytes(string key, byte[] value)
        {
            _dictionary[key] = Convert.ToBase64String(value);
        }
    }
}
