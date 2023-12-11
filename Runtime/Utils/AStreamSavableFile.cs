using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Gilzoide.KeyValueStore.Utils
{
    public abstract class AStreamSavableFile : ISavable, IStreamSavable, IFilePathProvider
    {
        public string FilePath { get; set; }
        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.NoCompression;
        public ICryptoTransform Encryptor { get; set; } = null;
        public ICryptoTransform Decryptor { get; set; } = null;

        public SymmetricAlgorithm EncryptionAlgorithm
        {
            set
            {
                Encryptor = value.CreateEncryptor();
                Decryptor = value.CreateDecryptor();
            }
        }

        public void Load()
        {
            FileStream fileStream = this.OpenFileReadStream();
            if (fileStream == null)
            {
                return;
            }

            Stream stream = fileStream;
            using (fileStream)
            using (WrapDecompress(ref stream))
            using (WrapDecrypt(ref stream))
            {
                Load(stream);
            }
        }

        public void Save()
        {
            FileStream fileStream = this.OpenFileWriteStream();
            if (fileStream == null)
            {
                return;
            }

            Stream stream = fileStream;
            using (fileStream)
            using (WrapCompress(ref stream))
            using (WrapEncrypt(ref stream))
            {
                Save(stream);
            }
        }

        public abstract void Load(Stream stream);
        public abstract void Save(Stream stream);

        protected IDisposable WrapDecompress(ref Stream stream)
        {
            if (CompressionLevel == CompressionLevel.NoCompression)
            {
                return null;
            }
            else
            {
                stream = new GZipStream(stream, CompressionMode.Decompress);
                return stream;
            }
        }

        protected IDisposable WrapCompress(ref Stream stream)
        {
            if (CompressionLevel == CompressionLevel.NoCompression)
            {
                return null;
            }
            else
            {
                stream = new GZipStream(stream, CompressionLevel);
                return stream;
            }
        }

        protected IDisposable WrapDecrypt(ref Stream stream)
        {
            if (Decryptor == null)
            {
                return null;
            }
            else
            {
                stream = new CryptoStream(stream, Decryptor, CryptoStreamMode.Read);
                return stream;
            }
        }

        protected IDisposable WrapEncrypt(ref Stream stream)
        {
            if (Encryptor == null)
            {
                return null;
            }
            else
            {
                stream = new CryptoStream(stream, Encryptor, CryptoStreamMode.Write);
                return stream;
            }
        }
    }
}
