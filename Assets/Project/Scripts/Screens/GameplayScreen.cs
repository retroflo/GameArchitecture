using System;
using UnityEngine;
using UnityEngine.UI;

namespace LWFlo
{
    public class GameplayScreen : MonoBehaviour
    {
        public event Action<string> OnButtonClicked;

        public void RegisterAllButtons()
        {
            var buttons = transform.GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                var buttonName = button.gameObject.name;
                
                button.onClick.AddListener(() =>
                {
                    OnButtonClicked?.Invoke(buttonName);
                });
            }
        }

        public void DisposeScreen()
        {
            Destroy(gameObject);
        }
    }
}