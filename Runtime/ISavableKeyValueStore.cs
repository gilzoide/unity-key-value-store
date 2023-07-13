namespace Gilzoide.KeyValueStore
{
    public interface ISavable
    {
        void Load();
        void Save();
    }

    public interface ISavableKeyValueStore : IKeyValueStore, ISavable {}
}
