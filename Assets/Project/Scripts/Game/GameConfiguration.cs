using Library.SerializableDictionary;
using UnityEngine;

namespace LWFlo.Game
{
    [CreateAssetMenu(menuName = "LWFlo/Configuration/Game Configuration")]
    public class GameConfiguration : ScriptableObject
    {
        public SerializableDictionaryBase<GameStateNames, BaseScene> sceneData;
    }
}