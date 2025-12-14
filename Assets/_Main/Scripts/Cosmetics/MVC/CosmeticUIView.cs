using System;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Interfaces.Vibration;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticUIView : ManagedBehavior, ICosmeticUISounds, ICosmeticUIVibration
    {
        [SerializeField] private CosmeticUIPanelSelector uiSelector;
        
        private CosmeticUIComponents _uiComponents;
        private GameObject _currentPanel;
        
        public event Action OnMainMenuButtonPressed;
        public event Action<int> OnSkinSelected;

        private void Awake()
        {
            GetUiComponents().MainMenuButton.onClick.AddListener(() =>
            {
                OnMainMenuButtonPressed?.Invoke();
            });
            
            GetUiComponents().ButtonSelector.OnSkinSelected += (value) =>
            {
                OnSkinSelected?.Invoke(value);
            };
        }
        
        private CosmeticUIComponents GetUiComponents()
        {
            return _uiComponents ??= uiSelector.GetPanelData();
        }
    }
}