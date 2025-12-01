using System;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticUIView : ManagedBehavior, IObserver, ICosmeticUISounds
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
                Debug.Log("Here");
                OnMainMenuButtonPressed?.Invoke();
            });
            
            GetUiComponents().ButtonSelector.OnSkinSelected += OnSkinSelected;
        }

        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case CosmeticObserverMessage.Enable:
                    HandleEnable();
                    break;
                case CosmeticObserverMessage.Disable:
                    HandleDisable();
                    break;
            }
        }
        
        private void HandleEnable()
        {
            GetUiComponents().SetActivePanel(GetUiComponents().MainPanel);
        }

        private void HandleDisable()
        {
            GetUiComponents().DisableAllPanels();
        }
        
        private CosmeticUIComponents GetUiComponents()
        {
            return _uiComponents ??= uiSelector.GetPanelData();
        }
    }
}