using System;
using System.Globalization;
using UnityEditor;

namespace Gilzoide.KeyValueStore.Editor
{
    public class EditorPrefsKeyValueStore : IKeyValueStore
    {
        public bool HasKey(string key)
        {
            return EditorPrefs.HasKey(key);
        }

        public void DeleteKey(string key)
        {
            EditorPrefs.DeleteKey(key);
        }

        public void DeleteAll()
        {
            EditorPrefs.DeleteAll();
        }

        public bool TryGetBool(string key, out bool value)
        {
            if (HasKey(key))
            {
                value = EditorPrefs.GetInt(key, 0) != 0;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public bool TryGetInt(string key, out int value)
        {
            if (HasKey(key))
            {
                value = EditorPrefs.GetInt(key, 0);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public bool TryGetLong(string key, out long value)
        {
            if (TryGetString(key, out string longAsString)
                && long.TryParse(longAsString, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public bool TryGetFloat(string key, out float value)
        {
            if (HasKey(key))
            {
                value = EditorPrefs.GetFloat(key, 0);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public bool TryGetDouble(string key, out double value)
        {
            if (TryGetString(key, out string doubleAsString)
                && double.TryParse(doubleAsString, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public bool TryGetString(string key, out string value)
        {
            if (HasKey(key))
            {
                value = EditorPrefs.GetString(key, "");
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public bool TryGetBytes(string key, out byte[] value)
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

        public void SetBool(string key, bool value)
        {
            EditorPrefs.SetInt(key, value ? 1 : 0);
        }

        public void SetInt(string key, int value)
        {
            EditorPrefs.SetInt(key, value);
        }

        public void SetLong(string key, long value)
        {
            EditorPrefs.SetString(key, value.ToString(CultureInfo.InvariantCulture));
        }

        public void SetFloat(string key, float value)
        {
            EditorPrefs.SetFloat(key, value);
        }

        public void SetDouble(string key, double value)
        {
            EditorPrefs.SetString(key, value.ToString(CultureInfo.InvariantCulture));
        }

        public void SetString(string key, string value)
        {
            EditorPrefs.SetString(key, value);
        }

        public void SetBytes(string key, byte[] value)
        {
            EditorPrefs.SetString(key, Convert.ToBase64String(value));
        }
    }
}
