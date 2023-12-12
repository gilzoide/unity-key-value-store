using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using UnityEngine;

namespace Gilzoide.KeyValueStore
{
    [StructLayout(LayoutKind.Sequential)]
    public class SqliteKeyValueStore : IKeyValueStore, IDisposable
    {
        #region Native functions

#if UNITY_WEBGL && !UNITY_EDITOR
        private const string SqliteKvsDll = "__Internal";
#else
        private const string SqliteKvsDll = "sqlitekvs";
#endif

        [DllImport(SqliteKvsDll, CharSet = CharSet.Unicode)]
        private static extern int SqliteKVS_open([In, Out] SqliteKeyValueStore kvs, string filename);

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
        private static extern int SqliteKVS_has_key([In, Out] SqliteKeyValueStore kvs, string key);

        [DllImport(SqliteKvsDll, CharSet = CharSet.Unicode)]
        private static extern int SqliteKVS_set_int([In, Out] SqliteKeyValueStore kvs, string key, long value);

        [DllImport(SqliteKvsDll, CharSet = CharSet.Unicode)]
        private static extern int SqliteKVS_set_double([In, Out] SqliteKeyValueStore kvs, string key, double value);

        [DllImport(SqliteKvsDll, CharSet = CharSet.Unicode)]
        private static extern int SqliteKVS_set_text([In, Out] SqliteKeyValueStore kvs, string key, string value, long length);

        [DllImport(SqliteKvsDll, CharSet = CharSet.Unicode)]
        private static extern int SqliteKVS_set_bytes([In, Out] SqliteKeyValueStore kvs, string key, byte[] bytes, long length);

        [DllImport(SqliteKvsDll, CharSet = CharSet.Unicode)]
        private static extern int SqliteKVS_delete_key([In, Out] SqliteKeyValueStore kvs, string key);

        [DllImport(SqliteKvsDll)]
        private static extern int SqliteKVS_delete_all([In, Out] SqliteKeyValueStore kvs);

        [DllImport(SqliteKvsDll)]
        private static extern int SqliteKVS_begin([In, Out] SqliteKeyValueStore kvs);

        [DllImport(SqliteKvsDll)]
        private static extern int SqliteKVS_commit([In, Out] SqliteKeyValueStore kvs);

        [DllImport(SqliteKvsDll)]
        private static extern void SqliteKVS_reset_select(SqliteKeyValueStore kvs);

        [DllImport(SqliteKvsDll, CharSet = CharSet.Ansi)]
        private static extern int SqliteKVS_run_sql([In, Out] SqliteKeyValueStore kvs, string sql, SqlErrorDelegate errorCallback, SqlRowDelegate rowCallback);

        #endregion

        #region Native callbacks

        private delegate void SqlErrorDelegate(IntPtr errorUtf8);
        unsafe private delegate int SqlRowDelegate(IntPtr kvs, int columnCount, IntPtr* values, IntPtr* columnNames);

        private static List<string> _sqlReturn = new List<string>();
        private static string _sqlError;

        [MonoPInvokeCallback(typeof(SqlErrorDelegate))]
        private static void SqlErrorCallback(IntPtr errorUtf8)
        {
            _sqlError = Marshal.PtrToStringUTF8(errorUtf8);
        }

        [MonoPInvokeCallback(typeof(SqlRowDelegate))]
        unsafe private static int SqlRowCallback(IntPtr kvs, int columnCount, IntPtr* values, IntPtr* columnNames)
        {
            for (int i = 0; i < columnCount; i++)
            {
                string value = Marshal.PtrToStringUTF8(values[i]);
                _sqlReturn.Add(value);
            }
            return 0;
        }

        #endregion

        public SqliteKeyValueStore(string filename)
        {
            SqliteKVS_open(this, filename);
        }

        ~SqliteKeyValueStore()
        {
            Dispose();
        }

#pragma warning disable IDE0044
        private IntPtr _db = IntPtr.Zero;
        private IntPtr _stmtSelect = IntPtr.Zero;
        private IntPtr _stmtUpsert = IntPtr.Zero;
        private IntPtr _stmtDeleteKey = IntPtr.Zero;
        private IntPtr _stmtDeleteAll = IntPtr.Zero;
        private IntPtr _stmtBegin = IntPtr.Zero;
        private IntPtr _stmtCommit = IntPtr.Zero;
        private bool _isInTransaction = false;
        private bool _isPendingCommit = false;
#pragma warning restore IDE0044

        public void DeleteAll()
        {
            EnsureTransaction();
            SqliteKVS_delete_all(this);
            ScheduleCommit();
        }

        public void DeleteKey(string key)
        {
            EnsureTransaction();
            SqliteKVS_delete_key(this, key);
            ScheduleCommit();
        }

        public bool HasKey(string key)
        {
            EnsureTransaction();
            int result = SqliteKVS_has_key(this, key);
            SqliteKVS_reset_select(this);
            return result == 1;
        }

        public void SetBool(string key, bool value)
        {
            SetLong(key, value ? 1 : 0);
        }

        public void SetBytes(string key, byte[] value)
        {
            EnsureTransaction();
            SqliteKVS_set_bytes(this, key, value, value?.Length ?? 0);
            ScheduleCommit();
        }

        public void SetDouble(string key, double value)
        {
            EnsureTransaction();
            SqliteKVS_set_double(this, key, value);
            ScheduleCommit();
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
            EnsureTransaction();
            SqliteKVS_set_int(this, key, value);
            ScheduleCommit();
        }

        public void SetString(string key, string value)
        {
            EnsureTransaction();
            SqliteKVS_set_text(this, key, value, value?.Length * sizeof(char) ?? 0);
            ScheduleCommit();
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
            EnsureTransaction();
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
            EnsureTransaction();
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
            EnsureTransaction();
            int result = SqliteKVS_try_get_int(this, key, out value);
            SqliteKVS_reset_select(this);
            return result == 1;
        }

        public bool TryGetString(string key, out string value)
        {
            EnsureTransaction();
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
            Commit();
            SqliteKVS_close(this);
        }

        unsafe public void Pragma(string pragma, List<string> out_values = null)
        {
            Debug.Assert(!pragma.Contains(";"), "Pragma strings must not contain ';'");
            if (!pragma.TrimStart().StartsWith("pragma ", true, CultureInfo.InvariantCulture))
            {
                pragma = "PRAGMA " + pragma;
            }
            if (out_values != null)
            {
                RunSql(pragma, SqlRowCallback);
                out_values.AddRange(_sqlReturn);
                _sqlReturn.Clear();
            }
            else
            {
                RunSql(pragma);
            }
        }

        public void Vacuum()
        {
            Commit();
            RunSql("VACUUM");
        }

        private void EnsureTransaction()
        {
            if (!_isInTransaction)
            {
                _isInTransaction = true;
                SqliteKVS_begin(this);
            }
        }

        private void Commit()
        {
            if (_isInTransaction)
            {
                _isInTransaction = false;
                SqliteKVS_commit(this);
            }
        }

        private async void ScheduleCommit()
        {
            if (_isPendingCommit)
            {
                return;
            }

            _isPendingCommit = true;
            try
            {
                await Task.Yield();
                Commit();
            }
            finally
            {
                _isPendingCommit = false;
            }
        }

        private void RunSql(string sql, SqlRowDelegate sqlRowDelegate = null)
        {
            _sqlError = null;
            SqliteKVS_run_sql(this, sql, SqlErrorCallback, sqlRowDelegate);
            if (_sqlError != null)
            {
                throw new InvalidOperationException(_sqlError);
            }
        }
    }
}
