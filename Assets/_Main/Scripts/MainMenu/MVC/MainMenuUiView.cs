using System;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Interfaces.Vibration;
using _Main.Scripts.Menu;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.MainMenu.MVC
{
    public class MainMenuUiView : ManagedBehavior, IMainMenuUISounds, IMainMenuUiVibration
    {
        [SerializeField] private MainMenuUiPanelComponents uiPanelSelector;
        
        private MainMenuUiComponents _uiComponents;


        // Buttons Actions
        public event Action OnConfirmButtonClicked;
        public event Action OnCancelButtonClicked;
        public event Action OnBackButtonClicked;
        public event Action OnFirstPlayScreenPlay;

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
            
            #region Buttons
            
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
            
            GetUiComponents().FirstGame_Play.onClick.AddListener(() =>
            {
                OnFirstPlayScreenPlay?.Invoke();
            });
            
            GetUiComponents().FirstGame_Tutorial.onClick.AddListener(() =>
            {
                OnTutorialOpen?.Invoke();
            });
            
            GetUiComponents().FirstGame_Close.onClick.AddListener(() =>
            {
                OnBackToMenu?.Invoke();
            });
            
            #endregion
            
        }
        
        private MainMenuUiComponents GetUiComponents()
        {
            return _uiComponents ??= uiPanelSelector.GetPanelData();
        }
    }
}