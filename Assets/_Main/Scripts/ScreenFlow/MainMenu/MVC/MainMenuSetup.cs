using MeteorMadness.Contracts;
using MeteorMadness.Managers;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Menu
{
    [RequireComponent(typeof(MainMenuViewAnimation))]
    [RequireComponent(typeof(MainMenuUiView))]
    [RequireComponent(typeof(MainMenuView))]
    public class MainMenuSetup : ManagedBehavior
    {
        private MainMenuUiView _ui;
        private MainMenuView _view;
        private MainMenuViewAnimation _animation;
        //
        private MainMenuMotor _motor;
        private MainMenuController.IIMainMenuController _controller;

        private void Awake()
        {
            _ui = GetComponent<MainMenuUiView>();
            _view = GetComponent<MainMenuView>();
            _animation = GetComponent<MainMenuViewAnimation>();

            _motor = new MainMenuMotor();
            _controller = new MainMenuController(_motor);
            
            _motor.Subscribe(_animation);
            _motor.Subscribe(_view);
            _motor.Subscribe(_ui);
            
            SetViewHandlers();
            
            _controller.Initialize();
            
            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
        }
        

        private void EnableMainMenu()
        {
            _controller.TransitionToEnable();
        }

        private void DisableMainMenu()
        {
            _controller.TransitionToDisable();
        }

        #region ViewHandlers

        private void SetViewHandlers()
        {
            _view.OnMainMenuEnable += () => _controller.TransitionToMenu();
            _view.OnFirstGame += (value) => _controller.SetHasPlayed(value);
            //
            _ui.OnFirstPlayScreenPlay += () =>
            {
                _controller.SetHasPlayed(true);
                _controller.TriggerGameMode();
            };
            _ui.OnGameModeTriggered += () => _controller.TriggerGameMode();
            _ui.OnTutorialTriggered += () =>
            {
                // Flips HasPlayed
                GameManager.Instance.FlagsController.GetHasPlayed();
                _controller.TriggerTutorial();
            };
            _ui.OnLoreOpen += () => _controller.TransitionToLore();
            _ui.OnBackToMenu += () => _controller.TransitionToMenu();
            _ui.OnExit += () => _controller.TriggerQuit();
            _ui.OnCreditsOpen += () => _controller.TransitionToCredits();
            _ui.OnTutorialOpen += () => _controller.TransitionToTutorial();
            _ui.OnOptionsOpen += () => _controller.TriggerOptions();
            _ui.OnCosmeticTriggered += () => _controller.TriggerCosmetic();
            _ui.OnStatsOpen += () => _controller.TriggerStats();
            //
            _animation.OnPanelClosed += () => _controller.ExecuteDisable();
            _animation.OnMainPanelOpened += () => _controller.MainPanelOpened();
        }

        #endregion
        
        #region EventBus
        
        private void EventBus_GameScreen_Disable(GameScreenEvents.DisableScreen input)
        {
            if(input.ScreenType != ScreenType.MainMenu) return;
            
            if (input.RequestType == EventRequestType.Requested)
            {
                DisableMainMenu();
            }
        }

        private void EventBus_GameScreen_Enable(GameScreenEvents.EnableScreen input)
        {
            if(input.ScreenType != ScreenType.MainMenu) return;
            
            if (input.RequestType == EventRequestType.Granted)
            {
                EnableMainMenu();
            }
        }
        
        #endregion
    }
}