#if HAVE_NEWTONSOFT_JSON
using System.Collections.Generic;
using System.IO;
using Gilzoide.KeyValueStore.Utils;
using Newtonsoft.Json;

namespace Gilzoide.KeyValueStore
{
    public partial class DictionaryKeyValueStore : AStreamSavableFile, ISavableKeyValueStore, IStreamSavableKeyValueStore, IFileKeyValueStore
    {
        public override void Load(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                dictionary = (Dictionary<string, object>) new JsonSerializer().Deserialize(streamReader, typeof(Dictionary<string, object>));
            }
        }

        public override void Save(Stream stream)
        {
            using (var streamWriter = new StreamWriter(stream))
            {
                new JsonSerializer().Serialize(streamWriter, dictionary);
            }
        }
    }
}
#endif
