using System.IO;
using System.IO.Compression;

namespace Gilzoide.KeyValueStore.Utils
{
    public abstract class AStreamSavableFile : ISavable, IStreamSavable, IFilePathProvider
    {
        public string FilePath { get; set; }
        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.NoCompression;

        public void Load()
        {
            using (var fileStream = this.OpenFileReadStream())
            {
                if (fileStream == null)
                {
                    return;
                }

                if (CompressionLevel == CompressionLevel.NoCompression)
                {
                    Load(fileStream);
                }
                else
                {
                    using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                    {
                        Load(gzipStream);
                    }
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

                if (CompressionLevel == CompressionLevel.NoCompression)
                {
                    Save(fileStream);
                }
                else
                {
                    using (var gzipStream = new GZipStream(fileStream, CompressionLevel))
                    {
                        Save(gzipStream);
                    }
                }
            }
        }

        public abstract void Load(Stream stream);
        public abstract void Save(Stream stream);
    }
}
