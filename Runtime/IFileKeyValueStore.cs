using System.IO;
using UnityEngine;

namespace Gilzoide.KeyValueStore
{
    public interface IFilenameProvider
    {
        string FileName { get; }
    }

    public interface IFileKeyValueStore : IKeyValueStore, IFilenameProvider {}

    public static class IFilenameProviderExtensions
    {
        public static string GetPersistentPath(this IFilenameProvider filenameProvider)
        {
            string filename = filenameProvider.FileName;
            return string.IsNullOrEmpty(filename) || Path.IsPathRooted(filename)
                ? filename
                : $"{Application.persistentDataPath}/{filename}";
        }

        public static FileStream OpenFileReadStream(this IFilenameProvider filenameProvider)
        {
            string filePath = filenameProvider.GetPersistentPath();
            return string.IsNullOrEmpty(filePath) || !File.Exists(filePath)
                ? null
                : File.OpenRead(filePath);
        }

        public static FileStream OpenFileWriteStream(this IFilenameProvider filenameProvider, FileMode fileMode = FileMode.Create)
        {
            string filePath = filenameProvider.GetPersistentPath();
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return File.Open(filePath, fileMode, FileAccess.Write);
        }
    }
}
