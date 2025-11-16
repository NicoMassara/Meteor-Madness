using System;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Options.MVC
{
    public class OptionsMenuUIView : ManagedBehavior, IObserver
    {
        [SerializeField] private OptionsMenuUIPanelSelector uiPanelSelector;
        
        private OptionsMenuUIComponents _uiComponents;
        private GameObject _currentPanel;
        
        public event Action OnAcceptButtonPressed;
        public event Action OnBackButtonPressed;
        
        public event Action OnBackToMenu;
        public event Action OnLanguageButtonPressed;

        private void Awake()
        {
            GetUiComponents().MainMenuButton.onClick.AddListener(() =>
            {
                OnBackToMenu?.Invoke();
                OnBackButtonPressed?.Invoke();
            });
            
            GetUiComponents().LanguageButton.onClick.AddListener(() =>
            {
                OnLanguageButtonPressed?.Invoke();
                OnAcceptButtonPressed?.Invoke();
            });
        }

        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case OptionsMenuObserverMessage.Enable:
                    HandleEnable();
                    break;
                case OptionsMenuObserverMessage.Disable:
                    HandleDisable();
                    break;
                case OptionsMenuObserverMessage.Initialize:
                    HandleInitialize();
                    break;
            }
        }

        private void HandleEnable()
        {
            GetUiComponents().MainPanel.SetActive(true);
        }
        private void HandleDisable()
        {
            DisableActivePanel();
            GetUiComponents().MainPanel.SetActive(false);
        }
        
        private void HandleInitialize()
        {

        }
        
        private void SetActivePanel(GameObject panelObject)
        {
            _currentPanel?.SetActive(false);
            _currentPanel = panelObject;
            _currentPanel.SetActive(true);
        }

        private void DisableActivePanel()
        {
            _currentPanel?.SetActive(false);
        }
        
        private OptionsMenuUIComponents GetUiComponents()
        {
            return _uiComponents ??= _uiComponents = uiPanelSelector.GetPanelData();
        }
    }
}