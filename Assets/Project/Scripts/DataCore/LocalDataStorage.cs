using System.IO;
using LWFlo.Constants;
using UnityEngine;

namespace LWFlo
{
    public class LocalDataStorage : ILocalDataStorage
    {
        public bool Has()
        {
            var path = GetPersistentPath();
            var exists = File.Exists(path);

            return exists;
        }
        
        public void Store(GameData gameData)
        {
            var path = GetPersistentPath();

            if (File.Exists(path))
                File.Delete(path);

            var gameDataByte = gameData.EncryptData();
            File.WriteAllBytes(path, gameDataByte);
        }
        
        public void Clear()
        {
            var path = GetPersistentPath();
            if (File.Exists(path))
                File.Delete(path);
        }

        public GameData Fetch()
        {
            var path = GetPersistentPath();
            var bytes = File.ReadAllBytes(path);

            var data = bytes.DecryptData();
            return data;
        }
        
        private static string GetPersistentPath()
        {
            return Path.Combine(Application.persistentDataPath, FileNames.PLAYER_SAVES);
        }
    }
}