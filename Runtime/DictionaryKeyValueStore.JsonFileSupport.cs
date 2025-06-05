#if HAVE_NEWTONSOFT_JSON
using System.Collections.Generic;
using System.IO;
using Gilzoide.KeyValueStore.Utils;
using Newtonsoft.Json;

namespace Gilzoide.KeyValueStore
{
    public partial class DictionaryKeyValueStore : AStreamSavableFile, ISavableKeyValueStore, IStreamSavableKeyValueStore, IFileKeyValueStore
    {
        public readonly JsonSerializer JsonSerializer = JsonSerializer.CreateDefault();

        public override void Load(Stream stream)
        {
            using StreamReader streamReader = new(stream);
            _dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(new JsonTextReader(streamReader));
        }

        public override void Save(Stream stream)
        {
            using StreamWriter streamWriter = new(stream);
            JsonSerializer.Serialize(streamWriter, _dictionary);
        }
    }
}
#endif
