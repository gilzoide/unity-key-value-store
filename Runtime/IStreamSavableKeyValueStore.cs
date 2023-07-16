using System.IO;

namespace Gilzoide.KeyValueStore
{
    public interface IStreamSavable
    {
        void Load(Stream stream);
        void Save(Stream stream);
    }

    public interface IStreamSavableKeyValueStore : IKeyValueStore, IStreamSavable {}
}
