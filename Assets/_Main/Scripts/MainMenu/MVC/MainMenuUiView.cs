using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.MainMenu.MVC
{
    public class MainMenuUiView : ManagedBehavior, IObserver
    {
        [Header("Main Panel")]
        [SerializeField] private GameObject mainPanel;
        [Header("Sub Panels")]
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject lorePanel;
        [Header("Sounds")] 
        [SerializeField] private SoundBehavior buttonSound;
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button tutorialButton;
        [SerializeField] private Button loreButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button backButton;

        private GameObject _currentPanel;

        public event Action OnGameModeStarted;
        public event Action OnTutorialStarted;
        public event Action OnLoreOpen;
        public event Action OnLoreClosed;
        public event Action OnExit;

        private void Awake()
        {
            playButton.onClick.AddListener(() =>
            {
                OnGameModeStarted?.Invoke();
                buttonSound.PlaySound();
            });
            tutorialButton.onClick.AddListener(() =>
            {
                OnTutorialStarted?.Invoke();
                buttonSound.PlaySound();
            });
            loreButton.onClick.AddListener(() =>
            {
                OnLoreOpen?.Invoke();
                buttonSound.PlaySound();
            });
            backButton.onClick.AddListener(() =>
            {
                OnLoreClosed?.Invoke();
                buttonSound.PlaySound();
            });
            exitButton.onClick.AddListener(() =>
            {
                OnExit?.Invoke();
                buttonSound.PlaySound();
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
        
        private void HandleEnable()
        {
            mainPanel.SetActive(true);
        }
        
        private void HandleDisable()
        {
            DisableActivePanel();
            mainPanel.SetActive(false);
        }
        
        private void HandleMainMenu()
        {
            SetActivePanel(menuPanel);
        }

        private void HandleLoreMenu()
        {
            SetActivePanel(lorePanel);
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