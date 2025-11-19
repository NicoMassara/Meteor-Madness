using System;
using _Main.Scripts.Menu;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.MainMenu.MVC
{
    public class MainMenuUiView : ManagedBehavior, IObserver
    {
        [SerializeField] private MainMenuUiPanelSelector uiPanelSelector;
        
        private MainMenuUiComponents _uiComponents;

        private GameObject _currentPanel;

        // Options Actions
        public event Action<float> OnVolumeSliderMoved;
        public event Action<int> OnLanguageChanged;
#if UNITY_ANDROID
        public event Action<bool> OnVibrationToggled;
#endif
        
        // Buttons Actions
        public event Action OnConfirmButtonClicked;
        public event Action OnCancelButtonClicked;
        public event Action OnBackButtonClicked;

        // Screens Actions
        public event Action OnGameModeTriggered;
        public event Action OnTutorialTriggered;
        public event Action OnCosmeticTriggered;
        public event Action OnTutorialOpen;
        public event Action OnCreditsOpen;
        public event Action OnOptionsOpen;
        public event Action OnLoreOpen;
        public event Action OnBackToMenu;
        public event Action OnExit;

        private void Start()
        {
            // --- Actions ---
            
            #region Screens
            
            GetUiComponents().PlayButton.onClick.AddListener(() =>
            {
                OnGameModeTriggered?.Invoke();
                OnConfirmButtonClicked?.Invoke();
            });
            GetUiComponents().OpenTutorialButton.onClick.AddListener(() =>
            {
                OnTutorialTriggered?.Invoke();
                OnConfirmButtonClicked?.Invoke();
            });
            
            GetUiComponents().TutorialButton.onClick.AddListener(() =>
            {
                OnTutorialOpen?.Invoke();
                OnConfirmButtonClicked?.Invoke();
            });
            
            GetUiComponents().LoreButton.onClick.AddListener(() =>
            {
                OnLoreOpen?.Invoke();
                OnConfirmButtonClicked?.Invoke();
            });
            
            GetUiComponents().CosmeticButton.onClick.AddListener(() =>
            {
                OnCosmeticTriggered?.Invoke();
                OnConfirmButtonClicked?.Invoke();
            });
            
            GetUiComponents().CreditsButton.onClick.AddListener(() =>
            {
                OnCreditsOpen?.Invoke();
                OnConfirmButtonClicked?.Invoke();
            });
            
            GetUiComponents().OptionsButton.onClick.AddListener(() =>
            {
                OnOptionsOpen?.Invoke();
                OnConfirmButtonClicked?.Invoke();
            });
            
            foreach (var backButton in GetUiComponents().BackButtons)
            {
                backButton.onClick.AddListener(() =>
                {
                    OnBackToMenu?.Invoke();
                    OnBackButtonClicked?.Invoke();
                });
            }

            GetUiComponents().QuitButton.onClick.AddListener(() =>
            {
                OnCancelButtonClicked?.Invoke();
                OnExit?.Invoke();
            });
            #endregion
            
            // --- Options ---

            #region Options
            
            GetUiComponents().VolumeSlider.OnChanged += (value) =>
            {
                OnConfirmButtonClicked?.Invoke();
                OnVolumeSliderMoved?.Invoke(value);
            };
            

            
            GetUiComponents().LanguageSelector.OnChanged += (value) =>
            {
                OnConfirmButtonClicked?.Invoke();
                OnLanguageChanged?.Invoke(value);
            };

#if UNITY_ANDROID

            GetUiComponents().VibrationToggle.OnChanged += (value) =>
            {
                OnConfirmButtonClicked?.Invoke();
                OnVibrationToggled?.Invoke(value);
            };
#endif
            
            #endregion
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
                case MainMenuObserverMessage.TutorialMenu:
                    HandleTutorialMenu();
                    break;
                case MainMenuObserverMessage.CreditsMenu:
                    HandleCreditsMenu();
                    break;
                case MainMenuObserverMessage.OptionsMenu:
                    HandleOptionsMenu();
                    break;
            }
        }
        
        private void HandleTutorialMenu()
        {
            SetActivePanel(GetUiComponents().TutorialPanel);
        }

        private MainMenuUiComponents GetUiComponents()
        {
            return _uiComponents ??= uiPanelSelector.GetPanelData();
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
        
        private void HandleCreditsMenu()
        {
            SetActivePanel(GetUiComponents().CreditsPanel);
        }

        private void HandleLoreMenu()
        {
            SetActivePanel(GetUiComponents().LorePanel);
        }
        
        private void HandleOptionsMenu()
        {
            SetActivePanel(GetUiComponents().OptionsPanel);
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