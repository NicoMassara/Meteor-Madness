using System;
using _Main.Scripts.Localization;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Options.MVC
{
    public class OptionsMenuUIView : ManagedBehavior, IObserver
    {
        [SerializeField] private OptionsMenuUIPanelSelector uiPanelSelector;
        
        private OptionsMenuUIComponents _uiComponents;
        private GameObject _currentPanel;
        
        public event Action OnBackToMenu;
        public event Action OnLanguageButtonPressed;

        private void Awake()
        {
            GetUiComponents().MainMenuButton.onClick.AddListener(() =>
            {
                OnBackToMenu?.Invoke();
                PlayButtonSound(UISoundType.Back);
            });
            
            GetUiComponents().LanguageButton.onClick.AddListener(() =>
            {
                OnLanguageButtonPressed?.Invoke();
                PlayButtonSound(UISoundType.Accept);
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
        
        private void PlayButtonSound(UISoundType buttonType)
        {
            SoundEventCaller.PlayUIButton(buttonType);
        }
        
        private OptionsMenuUIComponents GetUiComponents()
        {
            return _uiComponents ??= _uiComponents = uiPanelSelector.GetPanelData();
        }
    }
}