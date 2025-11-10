using _Main.Scripts.Localization;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.GameScreens
{
    [RequireComponent(typeof(GameScreenView))]
    public class GameScreenSetup : ManagedBehavior
    {
        private GameScreenMotor _motor;
        private GameScreenController _controller;
        private GameScreenView _view;

        private void Awake()
        {
            _motor = new GameScreenMotor();
            _controller = new GameScreenController(_motor);
            _view = GetComponent<GameScreenView>();
            
            _motor.Subscribe(_view);
            
            SetEventBus();
            
            _controller.Initialize();

            LocalizationEvents.OnLocalizationLoaded += () =>
            {
                _controller.TransitionToMainMenu();
            };
        }

        /*private void Start()
        {
            _controller.TransitionToMainMenu();
        }*/

        #region EventBus

        private void SetEventBus()
        {
            GameEventCaller.Subscribe<GameScreenEvents.SetScreen>(EventBus_OnSetGameScreen);
        }

        private void EventBus_OnSetGameScreen(GameScreenEvents.SetScreen input)
        {
            if(input.IsEnable == true) return;
            
            switch (input.ScreenType)
            {
                case ScreenType.MainMenu:
                    _controller.TransitionToMainMenu();
                    break;
                case ScreenType.GameMode:
                    _controller.TransitionToGameplay();
                    break;
                case ScreenType.Tutorial:
                    _controller.TransitionToTutorial();
                    break;
                case ScreenType.Cosmetic:
                    _controller.TransitionToCosmetic();
                    break;
                default:
                    Debug.LogWarning("GameScene Index is out of range.");
                    break;
            }
            
        }
        #endregion
    }
}