using System.IO;
using UnityEngine;

namespace Gilzoide.KeyValueStore
{
    public interface IFilenameProvider
    {
        string FileName { get; }
        string Extension { get; }
    }

    public interface IFileKeyValueStore : IFilenameProvider, IKeyValueStore {}

    public static class IFilenameProviderExtensions
    {
        public static string GetPersistentPath(this IFilenameProvider kvs)
        {
            string filename = kvs.FileName;
            return Path.IsPathRooted(filename)
                ? filename
                : $"{Application.persistentDataPath}/{Path.ChangeExtension(filename, kvs.Extension)}";
        }

        public static void EnsureDirectoryExists(this IFilenameProvider kvs)
        {
            string directoryPath = Path.GetDirectoryName(kvs.GetPersistentPath());
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
