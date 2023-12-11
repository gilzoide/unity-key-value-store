#include <stdlib.h>
#include <sqlite3.h>

// SQL statements
static const char SQL_CREATE_TABLE[] = "CREATE TABLE IF NOT EXISTS KeyValueStore (key TEXT NOT NULL PRIMARY KEY COLLATE BINARY, value BLOB)";
static const char SQL_SELECT[] = "SELECT value FROM KeyValueStore WHERE key = ?1";
static const char SQL_UPSERT[] = "INSERT INTO KeyValueStore(key, value) VALUES(?1, ?2) ON CONFLICT(key) DO UPDATE SET value = ?2";

// SQL statement binding indices
static const int SELECT_KEY_INDEX = 1;
static const int UPSERT_KEY_INDEX = 1;
static const int UPSERT_VALUE_INDEX = 2;

typedef struct {
	sqlite3 *db;
	sqlite3_stmt *stmt_select;
	sqlite3_stmt *stmt_upsert;
} KVS;

// MARK: Helper functions
static int SqliteKVS_create_table(KVS *kvs) {
	sqlite3_stmt *create_table;
	int result = sqlite3_prepare(kvs->db, SQL_CREATE_TABLE, -1, &create_table, NULL);
	if (result != SQLITE_OK) {
		return result;
	}

	result = sqlite3_step(create_table);
	sqlite3_finalize(create_table);

	return result;
}

static void SqliteKVS_prepare_select(KVS *kvs, const void *key_utf16) {
	if (kvs->stmt_select == NULL) {
		sqlite3_prepare(kvs->db, SQL_SELECT, -1, &kvs->stmt_select, NULL);
	}
	else {
		sqlite3_reset(kvs->stmt_select);
	}
	sqlite3_bind_text16(kvs->stmt_select, SELECT_KEY_INDEX, key_utf16, -1, SQLITE_STATIC);
}

static void SqliteKVS_prepare_upsert(KVS *kvs, const void *key_utf16) {
	if (kvs->stmt_upsert == NULL) {
		sqlite3_prepare(kvs->db, SQL_UPSERT, -1, &kvs->stmt_upsert, NULL);
	}
	else {
		sqlite3_reset(kvs->stmt_upsert);
	}
	sqlite3_bind_text16(kvs->stmt_upsert, UPSERT_KEY_INDEX, key_utf16, -1, SQLITE_STATIC);
}

// MARK: Open/Close
int SqliteKVS_open(KVS *kvs, const void *filename_utf16) {
	int result = sqlite3_open16(filename_utf16, &kvs->db);
	if (result != SQLITE_OK) {
		return result;
	}

	return SqliteKVS_create_table(kvs);
}

void SqliteKVS_close(KVS *kvs) {
	sqlite3_close(kvs->db);
	sqlite3_finalize(kvs->stmt_select);
	sqlite3_finalize(kvs->stmt_upsert);
	*kvs = (KVS){};
}

// MARK: Try Get functions
int SqliteKVS_try_get_int(KVS *kvs, const void *key_utf16, sqlite3_int64 *out_value) {
	SqliteKVS_prepare_select(kvs, key_utf16);
	int result = sqlite3_step(kvs->stmt_select);
	switch (result) {
		case SQLITE_DONE:
			return 0;

		case SQLITE_ROW:
			*out_value = sqlite3_column_int64(kvs->stmt_select, 0);
			return 1;

		default:
			return -result;
	}
}

int SqliteKVS_try_get_double(KVS *kvs, const void *key_utf16, double *out_value) {
	SqliteKVS_prepare_select(kvs, key_utf16);
	int result = sqlite3_step(kvs->stmt_select);
	switch (result) {
		case SQLITE_DONE:
			return 0;

		case SQLITE_ROW:
			*out_value = sqlite3_column_double(kvs->stmt_select, 0);
			return 1;

		default:
			return -result;
	}
}

int SqliteKVS_try_get_text(KVS *kvs, const void *key_utf16, const void **out_value, int *out_length) {
	SqliteKVS_prepare_select(kvs, key_utf16);
	int result = sqlite3_step(kvs->stmt_select);
	switch (result) {
		case SQLITE_DONE:
			return 0;

		case SQLITE_ROW:
			*out_value = sqlite3_column_text16(kvs->stmt_select, 0);
			*out_length = sqlite3_column_bytes16(kvs->stmt_select, 0);
			return 1;

		default:
			return -result;
	}
}

int SqliteKVS_try_get_bytes(KVS *kvs, const void *key_utf16, const void **out_value, int *out_length) {
	SqliteKVS_prepare_select(kvs, key_utf16);
	int result = sqlite3_step(kvs->stmt_select);
	switch (result) {
		case SQLITE_DONE:
			return 0;

		case SQLITE_ROW:
			*out_value = sqlite3_column_blob(kvs->stmt_select, 0);
			*out_length = sqlite3_column_bytes(kvs->stmt_select, 0);
			return 1;

		default:
			return -result;
	}
}

int SqliteKVS_has_key(KVS *kvs, const void *key_utf16) {
	SqliteKVS_prepare_select(kvs, key_utf16);
	return sqlite3_step(kvs->stmt_select) == SQLITE_ROW;
}

// MARK: Set functions
int SqliteKVS_set_int(KVS *kvs, const void *key_utf16, sqlite3_int64 value) {
	SqliteKVS_prepare_upsert(kvs, key_utf16);
	sqlite3_bind_int64(kvs->stmt_upsert, UPSERT_VALUE_INDEX, value);
	return sqlite3_step(kvs->stmt_upsert);
}

int SqliteKVS_set_double(KVS *kvs, const void *key_utf16, double value) {
	SqliteKVS_prepare_upsert(kvs, key_utf16);
	sqlite3_bind_double(kvs->stmt_upsert, UPSERT_VALUE_INDEX, value);
	return sqlite3_step(kvs->stmt_upsert);
}

int SqliteKVS_set_text(KVS *kvs, const void *key_utf16, const void *value_utf16, sqlite3_int64 length) {
	SqliteKVS_prepare_upsert(kvs, key_utf16);
	sqlite3_bind_text16(kvs->stmt_upsert, UPSERT_VALUE_INDEX, value_utf16, length, SQLITE_STATIC);
	return sqlite3_step(kvs->stmt_upsert);
}

int SqliteKVS_set_bytes(KVS *kvs, const void *key_utf16, const void *bytes, sqlite3_int64 length) {
	SqliteKVS_prepare_upsert(kvs, key_utf16);
	sqlite3_bind_blob64(kvs->stmt_upsert, UPSERT_VALUE_INDEX, bytes, length, SQLITE_STATIC);
	return sqlite3_step(kvs->stmt_upsert);
}

// MARK: Reset statements, to be called by C# after getting text/blob data
void SqliteKVS_reset_select(KVS *kvs) {
	sqlite3_reset(kvs->stmt_select);
}

void SqliteKVS_reset_upsert(KVS *kvs) {
	sqlite3_reset(kvs->stmt_upsert);
}
