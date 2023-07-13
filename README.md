# Unity Key-Value Store
Simple to use Key-Value Store interface and implementations for Unity, suitable for save systems.

Key-Value Stores provide a simple and effective way of persisting arbitrary data mapped to string keys.
Possible implementations of Key-Value Stores include, but are not limited to:
- [C# Dictionary<string, object>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)
- [PlayerPrefs](https://docs.unity3d.com/ScriptReference/PlayerPrefs.html)
- [LiteDB](https://github.com/mbdavid/LiteDB) / [UltraLiteDB](https://github.com/rejemy/UltraLiteDB)
- [FASTER KV](https://github.com/microsoft/FASTER)
- [LMDB](https://www.symas.com/lmdb)
- [SQLite](https://www.sqlite.org/)
- [iCloud KVS](https://developer.apple.com/documentation/foundation/nsubiquitouskeyvaluestore)
- [macOS/iOS Keychain](https://developer.apple.com/documentation/security/keychain_services/keychain_items)
- [Android Keystore](https://developer.android.com/training/articles/keystore)


## Features
- Simple to use `IKeyValueStore` interface with methods for setting, getting and deleting key-value pairs
- Key-Value Store wrappers for [automatic saving](Runtime/AutoSaveKeyValueStore.cs), (TODO) compressing and/or encrypting save files


## Basic usage
```cs
using Gilzoide.KeyValueStore;

// 1. Instantiate one of the IKeyValueStore implementations
string savePath = Application.persistentDataPath + "/MySaveFile.json";
IKeyValueStore kvs = new DictionaryKeyValueStore(savePath);

// 2. (optional) If Key-Value Store supports saving, load previous data
if (kvs is ISavableKeyValueStore savableKvs)
{
    savableKvs.Load();
}

// 3. Set/Get/Delete values
kvs.SetBool("finishedTutorial", true);
kvs.SetString("username", "gilzoide");

Debug.Log("Checking for values: " + kvs.HasKey("username"));
Debug.Log("Getting values: " + kvs.GetString("username"));
Debug.Log("Getting values with fallback: " + kvs.GetString("username", "default username"));
// Like C# Dictionary, this idiom returns a bool if the key is found
if (kvs.TryGetString("someKey", out string foundValue))
{
    Debug.Log("'someKey' exists: " + foundValue);
}

kvs.DeleteKey("someKey");

// 4. (optional) If the Key-Value Store supports saving, save the data
//    Tip: the AutoSaveKeyValueStore implementation schedules Save()
//         automatically when you call `Set*` or `DeleteKey` in it
if (kvs is ISavableKeyValueStore savableKvs)
{
    savableKvs.Save();
}
```