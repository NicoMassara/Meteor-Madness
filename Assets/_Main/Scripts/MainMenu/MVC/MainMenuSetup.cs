using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.MainMenu.MVC
{
    [RequireComponent(typeof(MainMenuUiView))]
    [RequireComponent(typeof(MainMenuView))]
    public class MainMenuSetup : ManagedBehavior
    {
        private MainMenuUiView _ui;
        private MainMenuView _view;
        private MainMenuMotor _motor;
        private MainMenuController _controller;

        private void Awake()
        {
            _ui = GetComponent<MainMenuUiView>();
            _view = GetComponent<MainMenuView>();

            _motor = new MainMenuMotor();
            _controller = new MainMenuController(_motor);
            
            _motor.Subscribe(_ui);
            _motor.Subscribe(_view);
            
            SetViewHandlers();
            
            GameEventCaller.Subscribe<GameScreenEvents.SetScreen>(EventBus_GameScreen_SetScreen);
        }

        private void Start()
        {
            _controller.Initialize();
        }

        private void EnableMainMenu()
        {
            _controller.TransitionToInitial();
        }

        private void DisableMainMenu()
        {
            _controller.TransitionToDisable();
        }

        #region ViewHandlers

        private void SetViewHandlers()
        {
            _view.OnMainMenuEnable += EnableMainMenu;
            //
            _ui.OnGameModeStarted += () => _controller.TriggerGameMode();
            _ui.OnTutorialStarted += () => _controller.TriggerTutorial();
            _ui.OnLoreOpen += () => _controller.TransitionToLore();
            _ui.OnLoreClosed += () => _controller.TransitionToInitial();
            _ui.OnExit += () => _controller.TriggerQuit();
        }

        #endregion
        
        #region EventBus

        private void EventBus_GameScreen_SetScreen(GameScreenEvents.SetScreen input)
        {
            if (input.ScreenType == ScreenType.MainMenu &&
                input.IsEnable)
            {
                _controller.TransitionToEnable();
            }
            else
            {
                DisableMainMenu();
            }
        }

        #endregion
    }
}