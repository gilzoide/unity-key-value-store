#include <stdlib.h>
#include <stdint.h>

#ifdef EMBED_SQLITE
#include "sqlite3.h"
#include "sqlite3.c"
const char sqlite3_version[] = SQLITE_VERSION;
#else
#include <sqlite3.h>
#endif

// SQL statements
#define SQL_CREATE_TABLE "CREATE TABLE IF NOT EXISTS KeyValueStore (key TEXT NOT NULL PRIMARY KEY COLLATE BINARY, value BLOB)"
#define SQL_SELECT "SELECT value FROM KeyValueStore WHERE key = ?1"
#define SQL_UPSERT "INSERT INTO KeyValueStore(key, value) VALUES(?1, ?2) ON CONFLICT(key) DO UPDATE SET value = ?2"
#define SQL_DELETE_KEY "DELETE FROM KeyValueStore WHERE key = ?1"
#define SQL_DELETE_ALL "DELETE FROM KeyValueStore"
#define SQL_BEGIN "BEGIN"
#define SQL_COMMIT "COMMIT"

// SQL statement binding indices
#define SELECT_KEY_INDEX 1
#define UPSERT_KEY_INDEX 1
#define UPSERT_VALUE_INDEX 2
#define DELETE_KEY_INDEX 1

typedef struct {
	sqlite3 *db;
	sqlite3_stmt *stmt_select;
	sqlite3_stmt *stmt_upsert;
	sqlite3_stmt *stmt_delete_key;
	sqlite3_stmt *stmt_delete_all;
	sqlite3_stmt *stmt_begin;
	sqlite3_stmt *stmt_commit;
	uint8_t is_in_transaction;
	uint8_t is_pending_commit;
} KVS;

// MARK: Helper functions
static int SqliteKVS_create_table(KVS *kvs) {
	sqlite3_stmt *create_table;
	int result = sqlite3_prepare(kvs->db, SQL_CREATE_TABLE, sizeof(SQL_CREATE_TABLE), &create_table, NULL);
	if (result != SQLITE_OK) {
		return result;
	}

	result = sqlite3_step(create_table);
	sqlite3_finalize(create_table);

	return result;
}

static void SqliteKVS_prepare_select(KVS *kvs, const void *key_utf16) {
	if (kvs->stmt_select == NULL) {
		sqlite3_prepare(kvs->db, SQL_SELECT, sizeof(SQL_SELECT), &kvs->stmt_select, NULL);
	}
	else {
		sqlite3_reset(kvs->stmt_select);
	}
	sqlite3_bind_text16(kvs->stmt_select, SELECT_KEY_INDEX, key_utf16, -1, SQLITE_STATIC);
}

static void SqliteKVS_prepare_upsert(KVS *kvs, const void *key_utf16) {
	if (kvs->stmt_upsert == NULL) {
		sqlite3_prepare(kvs->db, SQL_UPSERT, sizeof(SQL_UPSERT), &kvs->stmt_upsert, NULL);
	}
	else {
		sqlite3_reset(kvs->stmt_upsert);
	}
	sqlite3_bind_text16(kvs->stmt_upsert, UPSERT_KEY_INDEX, key_utf16, -1, SQLITE_STATIC);
}
static void SqliteKVS_reset_upsert(KVS *kvs) {
	sqlite3_reset(kvs->stmt_upsert);
}

static void SqliteKVS_prepare_delete_key(KVS *kvs, const void *key_utf16) {
	if (kvs->stmt_delete_key == NULL) {
		sqlite3_prepare(kvs->db, SQL_DELETE_KEY, sizeof(SQL_DELETE_KEY), &kvs->stmt_delete_key, NULL);
	}
	else {
		sqlite3_reset(kvs->stmt_delete_key);
	}
	sqlite3_bind_text16(kvs->stmt_delete_key, DELETE_KEY_INDEX, key_utf16, -1, SQLITE_STATIC);
}
static void SqliteKVS_reset_delete_key(KVS *kvs) {
	sqlite3_reset(kvs->stmt_delete_key);
}

static void SqliteKVS_prepare_delete_all(KVS *kvs) {
	if (kvs->stmt_delete_all == NULL) {
		sqlite3_prepare(kvs->db, SQL_DELETE_ALL, sizeof(SQL_DELETE_ALL), &kvs->stmt_delete_all, NULL);
	}
	else {
		sqlite3_reset(kvs->stmt_delete_all);
	}
}
static void SqliteKVS_reset_delete_all(KVS *kvs) {
	sqlite3_reset(kvs->stmt_delete_all);
}

static void SqliteKVS_prepare_begin(KVS *kvs) {
	if (kvs->stmt_begin == NULL) {
		sqlite3_prepare(kvs->db, SQL_BEGIN, sizeof(SQL_BEGIN), &kvs->stmt_begin, NULL);
	}
	else {
		sqlite3_reset(kvs->stmt_begin);
	}
}
static void SqliteKVS_reset_begin(KVS *kvs) {
	sqlite3_reset(kvs->stmt_begin);
}

static void SqliteKVS_prepare_commit(KVS *kvs) {
	if (kvs->stmt_commit == NULL) {
		sqlite3_prepare(kvs->db, SQL_COMMIT, sizeof(SQL_COMMIT), &kvs->stmt_commit, NULL);
	}
	else {
		sqlite3_reset(kvs->stmt_commit);
	}
}
static void SqliteKVS_reset_commit(KVS *kvs) {
	sqlite3_reset(kvs->stmt_commit);
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
	sqlite3_finalize(kvs->stmt_select);
	sqlite3_finalize(kvs->stmt_upsert);
	sqlite3_finalize(kvs->stmt_delete_key);
	sqlite3_finalize(kvs->stmt_delete_all);
	sqlite3_finalize(kvs->stmt_begin);
	sqlite3_finalize(kvs->stmt_commit);
	sqlite3_close(kvs->db);
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
	int result = sqlite3_step(kvs->stmt_upsert);
	SqliteKVS_reset_upsert(kvs);
	return result;
}

int SqliteKVS_set_double(KVS *kvs, const void *key_utf16, double value) {
	SqliteKVS_prepare_upsert(kvs, key_utf16);
	sqlite3_bind_double(kvs->stmt_upsert, UPSERT_VALUE_INDEX, value);
	int result = sqlite3_step(kvs->stmt_upsert);
	SqliteKVS_reset_upsert(kvs);
	return result;
}

int SqliteKVS_set_text(KVS *kvs, const void *key_utf16, const void *value_utf16, sqlite3_int64 length) {
	SqliteKVS_prepare_upsert(kvs, key_utf16);
	sqlite3_bind_text16(kvs->stmt_upsert, UPSERT_VALUE_INDEX, value_utf16, length, SQLITE_STATIC);
	int result = sqlite3_step(kvs->stmt_upsert);
	SqliteKVS_reset_upsert(kvs);
	return result;
}

int SqliteKVS_set_bytes(KVS *kvs, const void *key_utf16, const void *bytes, sqlite3_int64 length) {
	SqliteKVS_prepare_upsert(kvs, key_utf16);
	sqlite3_bind_blob64(kvs->stmt_upsert, UPSERT_VALUE_INDEX, bytes, length, SQLITE_STATIC);
	int result = sqlite3_step(kvs->stmt_upsert);
	SqliteKVS_reset_upsert(kvs);
	return result;
}

// MARK: Delete functions
int SqliteKVS_delete_key(KVS *kvs, const void *key_utf16) {
	SqliteKVS_prepare_delete_key(kvs, key_utf16);
	int result = sqlite3_step(kvs->stmt_delete_key);
	SqliteKVS_reset_delete_key(kvs);
	return result;
}

int SqliteKVS_delete_all(KVS *kvs) {
	SqliteKVS_prepare_delete_all(kvs);
	int result = sqlite3_step(kvs->stmt_delete_all);
	SqliteKVS_reset_delete_all(kvs);
	return result;
}

// MARK: Reset statement, to be called by C# after getting text/blob data
void SqliteKVS_reset_select(KVS *kvs) {
	sqlite3_reset(kvs->stmt_select);
}

// MARK: Transaction support
int SqliteKVS_begin(KVS *kvs) {
	SqliteKVS_prepare_begin(kvs);
	int result = sqlite3_step(kvs->stmt_begin);
	SqliteKVS_reset_begin(kvs);
	return result;
}

int SqliteKVS_commit(KVS *kvs) {
	SqliteKVS_prepare_commit(kvs);
	int result = sqlite3_step(kvs->stmt_commit);
	SqliteKVS_reset_commit(kvs);
	return result;
}

// MARK: Arbitrary SQL support, use with care!
int SqliteKVS_run_sql(KVS *kvs, const char *sql, void (*error_callback)(const char *str), int (*row_callback)(void *, int, char**, char**)) {
	char *err;
	int result = sqlite3_exec(kvs->db, sql, row_callback, kvs, &err);
	if (err) {
		if (error_callback) {
			error_callback(err);
		}
		sqlite3_free(err);
	}
	return result;
}
