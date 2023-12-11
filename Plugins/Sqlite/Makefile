ifeq ($(DEBUG),1)
	CFLAGS += -O0 -g -DDEBUG
else
	CFLAGS += -O3 -DNDEBUG
endif

Build/macOS/libsqlitekvs.dylib: CFLAGS += -arch arm64 -arch x86_64
Build/macOS/libsqlitekvs.dylib: LINKFLAGS += -shared -lsqlite3
Build/macOS/libsqlitekvs.dylib: SqliteKVS.c
	$(CC) -o $@ $< $(CFLAGS) $(LINKFLAGS)


# Targets
macos-universal: Build/macOS/libsqlitekvs.dylib