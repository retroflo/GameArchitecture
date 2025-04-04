using UnityEngine;

namespace LWFlo
{
    public class AppView : MonoBehaviour
    {
        [SerializeField] private Canvas _uiMenuRoot;
        [SerializeField] private Canvas _uiGameplayRoot;

        public Canvas GetUIMenuRoot()
        {
            return _uiMenuRoot;
        }
        
        public Canvas GetUIGameplayRoot()
        {
            return _uiGameplayRoot;
        }
    }
}