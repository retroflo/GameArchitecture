namespace LWFlo
{
    public interface ILocalDataStorage
    {
        void Store(GameData gameData);
        void Clear();
        bool Has();
        public GameData Fetch();
    }
}