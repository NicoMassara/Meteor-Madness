using System;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Menu;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.MainMenu.MVC
{
    public class MainMenuUiView : ManagedBehavior, IObserver, IMainMenuUISounds
    {
        [SerializeField] private MainMenuUiPanelSelector uiPanelSelector;
        
        private MainMenuUiComponents _uiComponents;
        
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
            }
        }
        
        private void HandleTutorialMenu()
        {
            GetUiComponents().SetActivePanel(GetUiComponents().TutorialPanel);
        }

        private void HandleEnable()
        {
            GetUiComponents().SetActivePanel(GetUiComponents().MenuPanel);
        }
        
        private void HandleDisable()
        {
            GetUiComponents().DisableAllPanels();
        }
        
        private void HandleMainMenu()
        {
            GetUiComponents().SetActivePanel(GetUiComponents().MenuPanel);
        }
        
        private void HandleCreditsMenu()
        {
            GetUiComponents().SetActivePanel(GetUiComponents().CreditsPanel);
        }

        private void HandleLoreMenu()
        {
            GetUiComponents().SetActivePanel(GetUiComponents().LorePanel);
        }
        
        private MainMenuUiComponents GetUiComponents()
        {
            return _uiComponents ??= uiPanelSelector.GetPanelData();
        }
    }
}