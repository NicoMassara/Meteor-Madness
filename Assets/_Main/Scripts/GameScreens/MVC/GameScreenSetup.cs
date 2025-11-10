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
        private GameScreenView _view;

        private void Awake()
        {
            _motor = new GameScreenMotor();
            _view = GetComponent<GameScreenView>();
            
            _motor.Subscribe(_view);
            
            SetEventBus();
            
            LocalizationEvents.OnLocalizationLoaded += Localization_OnLocalizationLoadedHandler;
        }

        private void SelectNewScreen(ScreenType screenType)
        {
            _motor.SelectNewScreen((int)screenType);
        }

        private void TransitionToNewScreen()
        {
            _motor.LoadCurrentScreen();
        }

        private void Localization_OnLocalizationLoadedHandler()
        {
            LocalizationEvents.OnLocalizationLoaded -= Localization_OnLocalizationLoadedHandler;
            _motor.LoadScreenByIndex((int)ScreenType.MainMenu);
        }
        
        #region EventBus

        private void SetEventBus()
        {
            GameEventCaller.Subscribe<GameScreenEvents.EnableScreen>(EventBus_GameScreen_Enable);
            GameEventCaller.Subscribe<GameScreenEvents.DisableScreen>(EventBus_GameScreen_Disable);
        }

        private void EventBus_GameScreen_Disable(GameScreenEvents.DisableScreen input)
        {
            if (input.RequestType == EventRequestType.Granted)
            {
                TransitionToNewScreen();
            }
        }

        private void EventBus_GameScreen_Enable(GameScreenEvents.EnableScreen input)
        {
            if (input.RequestType == EventRequestType.Requested)
            {
                SelectNewScreen(input.ScreenType);
            }
        }
        #endregion
    }
}