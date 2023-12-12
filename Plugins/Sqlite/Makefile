EMCC ?= emcc
ANDROID_NDK_ROOT ?=

ifeq ($(DEBUG),1)
	CFLAGS += -O0 -g -DDEBUG
else
	CFLAGS += -O3 -DNDEBUG
endif

BUILD_DIRS = \
	build/windows/x86_64 build/windows/x86 \
	build/linux/x86_64 build/linux/x86 \
	build/macos build/ios build/tvos \
	build/android/arm64 build/android/arm32 build/android/x86 build/android/x86_64 \
	build/webgl

$(BUILD_DIRS):
	mkdir -p $@

# Windows
build/windows/%/libsqlitekvs.dll: CFLAGS += -DEMBED_SQLITE
build/windows/%/libsqlitekvs.dll: LINKFLAGS += -shared
build/windows/%/libsqlitekvs.dll: src~/SqliteKVS.c src~/sqlite3.c | build/windows/%
	$(CC) -o $@ $< $(CFLAGS) $(LINKFLAGS)

# macOS
build/macos/libsqlitekvs.dylib: CFLAGS += -arch arm64 -arch x86_64
build/macos/libsqlitekvs.dylib: LINKFLAGS += -shared -lsqlite3
build/macos/libsqlitekvs.dylib: src~/SqliteKVS.c | build/macos
	$(CC) -o $@ $< $(CFLAGS) $(LINKFLAGS)

# iOS
build/ios/libsqlitekvs.dylib: CFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk iphoneos)
build/ios/libsqlitekvs.dylib: LINKFLAGS += -shared -lsqlite3
build/ios/libsqlitekvs.dylib: src~/SqliteKVS.c | build/ios
	$(CC) -o $@ $< $(CFLAGS) $(LINKFLAGS)

# tvOS
build/tvos/libsqlitekvs.dylib: CFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk appletvos)
build/tvos/libsqlitekvs.dylib: LINKFLAGS += -shared -lsqlite3
build/tvos/libsqlitekvs.dylib: src~/SqliteKVS.c | build/tvos
	$(CC) -o $@ $< $(CFLAGS) $(LINKFLAGS)

# Android
build/android/%/libsqlitekvs.so: CFLAGS += -fPIC -DEMBED_SQLITE
build/android/%/libsqlitekvs.so: LINKFLAGS += -shared
build/android/%/libsqlitekvs.so: src~/SqliteKVS.c src~/sqlite3.c | build/android/%
	$(CC) -o $@ $< $(CFLAGS) $(LINKFLAGS)

build/android/arm64/libsqlitekvs.so: CC = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/aarch64-linux-android21-clang)
build/android/arm32/libsqlitekvs.so: CC = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/armv7a-linux-androideabi19-clang)
build/android/x86_64/libsqlitekvs.so: CC = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/x86_64-linux-android21-clang)
build/android/x86/libsqlitekvs.so: CC = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/i686-linux-android19-clang)

# WebGL
build/webgl/libsqlitekvs.bc: CFLAGS += -DEMBED_SQLITE
build/webgl/libsqlitekvs.bc: src~/SqliteKVS.c src~/sqlite3.c | build/webgl
	$(EMCC) -o $@ $< $(CFLAGS) -c -emit-llvm

# Targets
windows-x86_64: build/windows/x86_64/libsqlitekvs.dll
windows-x86: build/windows/x86/libsqlitekvs.dll

windows-mingw-x86_64: CC = x86_64-w64-mingw32-gcc
windows-mingw-x86_64: LINKFLAGS += -static-libgcc
windows-mingw-x86_64: build/windows/x86_64/libsqlitekvs.dll

windows-mingw-x86: CC = i686-w64-mingw32-gcc
windows-mingw-x86: LINKFLAGS += -static-libgcc
windows-mingw-x86: build/windows/x86/libsqlitekvs.dll

macos-universal: build/macos/libsqlitekvs.dylib
ios-universal: build/ios/libsqlitekvs.dylib
tvos-universal: build/tvos/libsqlitekvs.dylib

android-arm64: build/android/arm64/libsqlitekvs.so
android-arm32: build/android/arm32/libsqlitekvs.so
android-x86_64: build/android/x86_64/libsqlitekvs.so
android-x86: build/android/x86/libsqlitekvs.so

webgl: build/webgl/libsqlitekvs.bc

all-apple: macos-universal ios-universal tvos-universal
all-android: android-arm64 android-arm32 android-x86_64 android-x86
all-windows: windows-x86_64 windows-x86
all-windows-mingw: windows-mingw-x86_64 windows-mingw-x86