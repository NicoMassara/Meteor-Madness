using _Main.Scripts.Managers;
using NicolasMassara.CustomTimerManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.GameScreens
{
    [RequireComponent(typeof(GameScreenView))]
    public class GameScreenSetup : ManagedBehavior
    {
        [SerializeField] private ScreenType defaultScreen = ScreenType.MainMenu;
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
        
        private void TransitionToLastScreen()
        {
            _motor.LoadLastScreen();
        }

        private void ModuleLoader_OnModulesLoaded()
        {
            ModuleLoaderEvents.OnModulesLoaded -= ModuleLoader_OnModulesLoaded;
            
            TimerManager.Add(new TimerData(Time.unscaledDeltaTime, 
                () => { _motor.ZoomIn(); }));
        }
        
        #region EventBus

        private void SetEventBus()
        {
            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
            GameScreenEventSubscriber.GoToLastScreen(EventBus_GameScreen_GoToLastScreen);
            CameraEventSubscriber.NotifyZoomFinished(EventBus_Camera_ZoomFinished);
        }

        private void EventBus_Camera_ZoomFinished(CameraEvents.ZoomFinished input)
        {
            CameraEventUnSubscriber.NotifyZoomFinished(EventBus_Camera_ZoomFinished);
            //
            
            TimerManager.Add(new TimerData(0.25f, 
                () => { _motor.LoadScreenByIndex((int)ScreenType.MainMenu); }));
        }

        private void EventBus_GameScreen_GoToLastScreen(GameScreenEvents.LastScreen input)
        {
            TransitionToLastScreen();
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