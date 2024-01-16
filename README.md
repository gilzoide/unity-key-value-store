# Key-Value Store for Unity
[![openupm](https://img.shields.io/npm/v/com.gilzoide.key-value-store?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.gilzoide.key-value-store/)

Simple to use Key-Value Store interface and implementations for Unity, suitable for save systems.

Key-Value Stores provide a simple and effective way of persisting arbitrary data mapped to string keys.
Possible implementations of Key-Value Stores include, but are not limited to:
[C# Dictionary](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2),
[PlayerPrefs](https://docs.unity3d.com/ScriptReference/PlayerPrefs.html),
[LiteDB](https://github.com/mbdavid/LiteDB) / [UltraLiteDB](https://github.com/rejemy/UltraLiteDB),
[FASTER KV](https://github.com/microsoft/FASTER),
[LMDB](https://www.symas.com/lmdb),
[SQLite](https://www.sqlite.org/),
[iCloud KVS](https://developer.apple.com/documentation/foundation/nsubiquitouskeyvaluestore),
[macOS/iOS Keychain](https://developer.apple.com/documentation/security/keychain_services/keychain_items),
[Android Keystore](https://developer.android.com/training/articles/keystore)
...


## Features
- Simple to use `IKeyValueStore` interface with methods for setting, getting and deleting key-value pairs
- Custom serialization of complex objects (class/struct types).
  You can set the default serializer for all objects, as well as one serializer per object type.
  Choose between serializing to text or binary format by implementing either `ITextSerializer<>` or `IBinarySerializer<>` interfaces.
- Wrapper for automatically saving data from Key-Value Stores


## Implementations
Key-Value Stores:
- [DictionaryKeyValueStore](Runtime/DictionaryKeyValueStore.cs): stores data in a `Dictionary<string, object>`.
  May be persisted in disk if [Json.NET](https://www.newtonsoft.com/json) is installed in the project, for example via the [Newtonsoft Json Unity Package](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@latest).
  Supports compression using [GZip](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream?view=netstandard-2.1).
- [PlayerPrefsKeyValueStore](Runtime/PlayerPrefsKeyValueStore.cs): stores data in Unity's [PlayerPrefs](https://docs.unity3d.com/ScriptReference/PlayerPrefs.html) class
- (external) [SqliteKeyValueStore](https://github.com/gilzoide/unity-key-value-store-sqlite): stores data using the [SQLite](https://sqlite.org) database engine.

Object serializers:
- [JsonUtilityTextSerializer](Runtime/ObjectSerializers/JsonUtilityTextSerializer.cs): the default serializer, uses Unity's [JsonUtility](https://docs.unity3d.com/ScriptReference/JsonUtility.html)
- [NewtonsoftJsonTextSerializer](Runtime/ObjectSerializers/NewtonsoftJsonTextSerializer.cs): uses [Json.NET](https://www.newtonsoft.com/json), if it is installed in your project
- [XmlTextSerializer](Runtime/ObjectSerializers/XmlTextSerializer.cs): uses C#'s [XmlSerializer](https://learn.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlserializer?view=netstandard-2.1)
- [StructBinarySerializer](Runtime/ObjectSerializers/StructBinarySerializer.cs): serializes any unmanaged struct to binary using Unity's [UnsafeUtility](https://docs.unity3d.com/ScriptReference/Unity.Collections.LowLevel.Unsafe.UnsafeUtility.html).
  Does not support classes.
  Unity 2018.1+ only.
- [UnityMathTextSerializer](Runtime/ObjectSerializers/UnityMathTextSerializer.cs): serializes Unity math structs, like `Vector2` and `Matrix4x4`, to text as a list of numbers separated by commas.
  Unity 2018.1+ only.


## How to install
Either:
- Use the [openupm registry](https://openupm.com/) and install this package using the [openupm-cli](https://github.com/openupm/openupm-cli):
  ```
  openupm add com.gilzoide.key-value-store
  ```
- Install using the [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html) with the following URL:
  ```
  https://github.com/gilzoide/unity-key-value-store.git#1.0.0-preview1
  ```
- Clone this repository or download a snapshot of it directly inside your project's `Assets` or `Packages` folder.


## Basic usage
```cs
using Gilzoide.KeyValueStore;
using UnityEngine;

// 1. Instantiate one of the IKeyValueStore implementations
IKeyValueStore kvs = new DictionaryKeyValueStore
{
    FilePath = Application.persistentDataPath + "/MySaveFile.json",
};


// 2. (optional) If Key-Value Store supports saving, load previous data
if (kvs is ISavableKeyValueStore savableKvs)
{
    savableKvs.Load();
}


// 3. Set/Get/Delete values
kvs.SetBool("finishedTutorial", true);
kvs.SetString("username", "gilzoide");

Debug.Log("Checking if values exist: " + kvs.HasKey("username"));
Debug.Log("Getting values: " + kvs.GetInt("username"));
Debug.Log("Getting values with fallback: " + kvs.GetString("username", "default username"));
// Like C# Dictionary, this idiom returns a bool if the key is found
if (kvs.TryGetString("someKey", out string foundValue))
{
    Debug.Log("'someKey' exists: " + foundValue);
}

kvs.DeleteKey("someKey");


// 4. (optional) If the Key-Value Store supports saving, save the data
if (kvs is ISavableKeyValueStore savableKvs)
{
    savableKvs.Save();
}
```


## Automatic saving
```cs
using Gilzoide.KeyValueStore;
using UnityEngine;

// 1. Instantiate one of the ISavableKeyValueStore implementations
ISavableKeyValueStore wrappedKvs = new DictionaryKeyValueStore
{
    FilePath = Application.persistentDataPath + "/MySaveFile.json",
};


// 2. Wrap the store into an AutoSaveKeyValueStoreWrapper and use it instead
IKeyValueStore kvs = new AutoSaveKeyValueStoreWrapper(wrappedKvs);


// 3. Whenever you set or delete values, `wrappedKvs.Save()` is automatically
// called in the next frame. Notice that we skip a frame to avoid making lots
// of I/O if you change lots of data in a single frame.
kvs.SetBool("will this be saved automatically next frame?", true);
```


## Class/struct serialization
```cs
using System.Collections.Generic;
using Gilzoide.KeyValueStore;
using Gilzoide.KeyValueStore.ObjectSerializers;
using UnityEngine;

IKeyValueStore kvs = new DictionaryKeyValueStore();

// 1. Set/Get objects
kvs.SetObject("aVector", new Vector3(1, 2, 3));
if (kvs.TryGetObject("lastPosition", out Vector3 lastPosition))
{
    Debug.Log("Last position was: " + lastPosition);
}


// 2. (optional) Use custom serializers
var newtonsoftJsonSerializer = new NewtonsoftJsonSerializer();
kvs.SetObject(newtonsoftJsonSerializer, "jsonList", new List<int> { 1, 3, 5, 9 });
List<int> anotherJsonList = kvs.GetObject<List<int>>(newtonsoftJsonSerializer, "someKey");


// 3. (optional) Add serializers to the default serializer map
// This makes them be used even when not passed explicitly
ObjectSerializerMap.DefaultSerializerMap.SetObjectSerializer<List<int>>(newtonsoftJsonSerializer);

kvs.SetObject("jsonList", new List<int> { 1, 3, 5, 9 });
List<int> yetAnotherJsonList = kvs.GetObject<List<int>>("someKey");


// 4. (optional) Use your own ObjectSerializerMap
var unityMathTextSerializer = new UnityMathTextSerializer();
var serializerMap = new ObjectSerializerMap
{
    DefaultSerializer = newtonsoftJsonSerializer,
    TypeToSerializerMap = new Dictionary<Type, IObjectSerializer>
    {
        [typeof(Vector2)] = unityMathTextSerializer,
        [typeof(Vector3)] = unityMathTextSerializer,
        [typeof(Vector4)] = unityMathTextSerializer,
    },
};

kvs.SetObject(serializerMap, "uses UnityMathTextSerializer", new Vector2(10, 5));
kvs.SetObject(serializerMap, "uses NewtonsoftJsonSerializer", new int[] { 1, 2, 3 });

// (tip) to avoid the annoyance of adding all math types manually, call this:
unityMathTextSerializer.RegisterInSerializerMap(serializerMap);
```