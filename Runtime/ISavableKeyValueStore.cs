namespace Gilzoide.KeyValueStore
{
    public interface ISavable
    {
        void Save();
    }

    public interface ISavableKeyValueStore : IKeyValueStore, ISavable {}
}
