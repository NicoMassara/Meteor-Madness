using _Main.Scripts.Managers;
using NicolasMassara.CustomTimerManager;
using NicolasMassara.CustomUpdateManager;
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
            
            ModuleLoaderEvents.OnModulesLoaded += ModuleLoader_OnModulesLoaded;
        }

        private void SelectNewScreen(ScreenType screenType)
        {
            _motor.SelectNewScreen((int)screenType);
        }

        private void TransitionToNewScreen()
        {
            _motor.LoadCurrentScreen();
        }

        private void ModuleLoader_OnModulesLoaded()
        {
            ModuleLoaderEvents.OnModulesLoaded -= ModuleLoader_OnModulesLoaded;

            TimerManager.Add(new TimerData(Time.unscaledDeltaTime, 
                () => {
                _motor.LoadScreenByIndex((int)ScreenType.GameMode);
                }));
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