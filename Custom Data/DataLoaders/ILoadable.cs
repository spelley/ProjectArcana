public interface ILoadable<T>
{
    string id { get; }
    string loadType { get; }
    T GetSaveData();
    bool LoadFromSaveData(T saveData);
}