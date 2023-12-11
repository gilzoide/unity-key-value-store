using System;
using System.Runtime.InteropServices;

namespace Gilzoide.KeyValueStore
{
    [StructLayout(LayoutKind.Sequential)]
    public class SqliteKeyValueStore : IKeyValueStore, IDisposable
    {
        #region Native functions

        private const string SqliteKvsDll = "sqlitekvs";

        [DllImport(SqliteKvsDll, CharSet = CharSet.Unicode)]
        private static extern int SqliteKVS_open([Out] SqliteKeyValueStore kvs, string filename);

        [DllImport(SqliteKvsDll)]
        private static extern void SqliteKVS_close([In, Out] SqliteKeyValueStore kvs);

        [DllImport(SqliteKvsDll, CharSet = CharSet.Unicode)]
        private static extern int SqliteKVS_try_get_int([In, Out] SqliteKeyValueStore kvs, string key, out long value);

        [DllImport(SqliteKvsDll, CharSet = CharSet.Unicode)]
        private static extern int SqliteKVS_try_get_double([In, Out] SqliteKeyValueStore kvs, string key, out double value);

        [DllImport(SqliteKvsDll, CharSet = CharSet.Unicode)]
        private static extern int SqliteKVS_try_get_text([In, Out] SqliteKeyValueStore kvs, string key, out IntPtr utf16, out int length);

        [DllImport(SqliteKvsDll, CharSet = CharSet.Unicode)]
        private static extern int SqliteKVS_try_get_bytes([In, Out] SqliteKeyValueStore kvs, string key, out IntPtr bytes, out int length);

        [DllImport(SqliteKvsDll, CharSet = CharSet.Unicode)]
        private static extern int SqliteKVS_set_int([In, Out] SqliteKeyValueStore kvs, string key, long value);

        [DllImport(SqliteKvsDll, CharSet = CharSet.Unicode)]
        private static extern int SqliteKVS_set_double([In, Out] SqliteKeyValueStore kvs, string key, double value);

        [DllImport(SqliteKvsDll, CharSet = CharSet.Unicode)]
        private static extern int SqliteKVS_set_text([In, Out] SqliteKeyValueStore kvs, string key, string value, long length);

        [DllImport(SqliteKvsDll, CharSet = CharSet.Unicode)]
        private static extern int SqliteKVS_set_bytes([In, Out] SqliteKeyValueStore kvs, string key, byte[] bytes, long length);

        [DllImport(SqliteKvsDll)]
        private static extern void SqliteKVS_reset_select(SqliteKeyValueStore kvs);

        [DllImport(SqliteKvsDll)]
        private static extern void SqliteKVS_reset_upsert(SqliteKeyValueStore kvs);

        #endregion

        public SqliteKeyValueStore(string filename)
        {
            SqliteKVS_open(this, filename);
        }

        private IntPtr _db = IntPtr.Zero;
        private IntPtr _stmtSelect = IntPtr.Zero;
        private IntPtr _stmtUpsert = IntPtr.Zero;

        public void DeleteAll()
        {
            throw new NotImplementedException();
        }

        public void DeleteKey(string key)
        {
            throw new NotImplementedException();
        }

        public bool HasKey(string key)
        {
            throw new NotImplementedException();
        }

        public void SetBool(string key, bool value)
        {
            SetLong(key, value ? 1 : 0);
        }

        public void SetBytes(string key, byte[] value)
        {
            SqliteKVS_set_bytes(this, key, value, value?.Length ?? 0);
            SqliteKVS_reset_upsert(this);
        }

        public void SetDouble(string key, double value)
        {
            SqliteKVS_set_double(this, key, value);
            SqliteKVS_reset_upsert(this);
        }

        public void SetFloat(string key, float value)
        {
            SetDouble(key, value);
        }

        public void SetInt(string key, int value)
        {
            SetLong(key, value);
        }

        public void SetLong(string key, long value)
        {
            SqliteKVS_set_int(this, key, value);
            SqliteKVS_reset_upsert(this);
        }

        public void SetString(string key, string value)
        {
            SqliteKVS_set_text(this, key, value, value?.Length * sizeof(char) ?? 0);
            SqliteKVS_reset_upsert(this);
        }

        public bool TryGetBool(string key, out bool value)
        {
            if (TryGetLong(key, out long longValue))
            {
                value = longValue != 0;
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
            int result = SqliteKVS_try_get_bytes(this, key, out IntPtr bytes, out int length);
            if (result == 1)
            {
                value = new byte[length];
                Marshal.Copy(bytes, value, 0, length);
            }
            else
            {
                value = null;
            }
            SqliteKVS_reset_select(this);
            return result == 1;
        }

        public bool TryGetDouble(string key, out double value)
        {
            int result = SqliteKVS_try_get_double(this, key, out value);
            SqliteKVS_reset_select(this);
            return result == 1;
        }

        public bool TryGetFloat(string key, out float value)
        {
            if (TryGetDouble(key, out double doubleValue))
            {
                value = (float) doubleValue;
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
            if (TryGetLong(key, out long longValue))
            {
                value = (int) longValue;
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
            int result = SqliteKVS_try_get_int(this, key, out value);
            SqliteKVS_reset_select(this);
            return result == 1;
        }

        public bool TryGetString(string key, out string value)
        {
            int result = SqliteKVS_try_get_bytes(this, key, out IntPtr utf16, out int length);
            if (result == 1)
            {
                value = Marshal.PtrToStringUni(utf16, length / sizeof(char));
            }
            else
            {
                value = null;
            }
            SqliteKVS_reset_select(this);
            return result == 1;
        }

        public void Dispose()
        {
            SqliteKVS_close(this);
        }
    }
}
