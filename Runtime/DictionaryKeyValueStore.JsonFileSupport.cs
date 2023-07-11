#if HAVE_NEWTONSOFT_JSON
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Gilzoide.KeyValueStore
{
    public partial class DictionaryKeyValueStore : ISavableKeyValueStore, IFileKeyValueStore
    {
        public string FileName { get; set; }

        public DictionaryKeyValueStore(string fileName, bool loadData = true)
        {
            FileName = fileName;
            if (loadData && !string.IsNullOrEmpty(fileName))
            {
                Load();
            }
        }

        public void Load()
        {
            using (var fileStream = this.OpenFileReadStream())
            {
                if (fileStream == null)
                {
                    return;
                }

                using (var streamReader = new StreamReader(fileStream))
                {
                    dictionary = (Dictionary<string, object>) new JsonSerializer().Deserialize(streamReader, typeof(Dictionary<string, object>));
                }
            }
        }

        public void Save()
        {
            using (var fileStream = this.OpenFileWriteStream())
            {
                if (fileStream == null)
                {
                    return;
                }

                using (var streamWriter = new StreamWriter(fileStream))
                {
                    new JsonSerializer().Serialize(streamWriter, dictionary);
                }
            }
        }
    }
}
#endif