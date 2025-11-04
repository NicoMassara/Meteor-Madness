using System;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Menu;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.MainMenu.MVC
{
    public class MainMenuUiView : ManagedBehavior, IObserver
    {
        [SerializeField] private MainMenuUiPanelSelector uiPanelSelector;
        
        private MainMenuUiComponents _uiComponents;

        private GameObject _currentPanel;

        public event Action OnGameModeStarted;
        public event Action OnTutorialStarted;
        public event Action OnLoreOpen;
        public event Action OnLoreClosed;
        public event Action OnExit;

        private void Awake()
        {
            GetUiComponents().PlayButton.onClick.AddListener(() =>
            {
                OnGameModeStarted?.Invoke();
                PlayButtonSound();
            });
            GetUiComponents().TutorialButton.onClick.AddListener(() =>
            {
                OnTutorialStarted?.Invoke();
                PlayButtonSound();
            });
            GetUiComponents().LoreButton.onClick.AddListener(() =>
            {
                OnLoreOpen?.Invoke();
                PlayButtonSound();
            });
            GetUiComponents().BackButton.onClick.AddListener(() =>
            {
                OnLoreClosed?.Invoke();
                PlayButtonSound();
            });
            GetUiComponents().QuitButton.onClick.AddListener(() =>
            {
                OnExit?.Invoke();
                PlayButtonSound();
            });
        }
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case MainMenuObserverMessage.Enable:
                    HandleEnable();
                    break;
                case MainMenuObserverMessage.Disable:
                    HandleDisable();
                    break;
                case MainMenuObserverMessage.MainMenu:
                    HandleMainMenu();
                    break;
                case MainMenuObserverMessage.LoreMenu:
                    HandleLoreMenu();
                    break;
            }
        }

        private MainMenuUiComponents GetUiComponents()
        {
            return _uiComponents ??= uiPanelSelector.GetPanelData();
        }

        private void PlayButtonSound()
        {
            SoundEventCaller.PlayUIButton(UISoundType.Accept);
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
        
        private void HandleMainMenu()
        {
            SetActivePanel(GetUiComponents().MenuPanel);
        }

        private void HandleLoreMenu()
        {
            SetActivePanel(GetUiComponents().LorePanel);
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
    }
}