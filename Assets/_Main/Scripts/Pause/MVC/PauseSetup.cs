using _Main.Scripts.Managers;
using _Main.Scripts.GlobalEvents;
using NicolasMassara.CustomTimerManager;
using UnityEngine;

namespace _Main.Scripts.Pause
{
    [RequireComponent(typeof(PauseUIView))]
    [RequireComponent(typeof(PauseViewAnimation))]
    [RequireComponent(typeof(PauseView))]
    public class PauseSetup : MonoBehaviour
    {
        private PauseUIView.IPauseViewUI _ui;
        private PauseView.IPauseView _view;
        private PauseViewAnimation.IPauseViewAnimation _animation;
        private PauseMotor _motor;

        private void Awake()
        {
            BootEvents.OnSubSystemRequestInitialize += Initialize;
            
        }

        private void Initialize()
        {
            BootEvents.OnSubSystemRequestInitialize -= Initialize;
            
            _motor = new PauseMotor();
            var view = GetComponent<PauseView>();
            var ui = GetComponent<PauseUIView>();
            var anim = GetComponent<PauseViewAnimation>();
            
            _motor.Subscribe(ui);
            _motor.Subscribe(anim);
            _motor.Subscribe(view);

            _view = view;
            _ui = ui;
            _animation = anim;
            
            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
            EarthEventSubscriber.Restart(EventsBus_Earth_Restart);
            
            SetViewHandlers();
            
            BootEvents.SubSystemInitialized();
        }

        private void SetViewHandlers()
        {
            _ui.OnMainMenuButtonPressed += _motor.TriggerLoadMainMenu;
            _ui.OnOptionsButtonPressed += _motor.TriggerOptionsMenu;
            _ui.OnResumeButtonPressed += _motor.TriggerGameMode;
            //
            _animation.OnPanelOpened += _motor.Enable;
            _animation.OnPanelClosed += _motor.ExecuteDisable;
        }
        
        #region Enable / Disable

        private void DisablePause()
        {
            _motor.StartDisable();
            UnSubscribeEventBus();
        }
        
        private void EnablePause()
        {
            _motor.Initialize();
            SubscribeEventBus();
        }

        #endregion

        #region Event Bus

        private void SubscribeEventBus()
        {
            EarthEventSubscriber.RestartFinished(EventBus_Earth_Restart_Finished);
        }
        
        private void UnSubscribeEventBus()
        {

        }

        private void EventBus_Earth_Restart_Finished(EarthEvents.RestartFinished input)
        {
            _motor.ExecuteDisable();
            
            EarthEventUnSubscriber.RestartFinished(EventBus_Earth_Restart_Finished);
        }


        #region GameScreens
        
        private void EventsBus_Earth_Restart(EarthEvents.Restart input)
        {
            TimerManager.Add(new TimerData(0.5f, EarthEventCaller.RestartFinished));
        }
        
        private void EventBus_GameScreen_Disable(GameScreenEvents.DisableScreen input)
        {
            if(input.ScreenType != ScreenType.Pause) return;
            
            if (input.RequestType == EventRequestType.Requested)
            {
                DisablePause();
            }
        }
        
        private void EventBus_GameScreen_Enable(GameScreenEvents.EnableScreen input)
        {
            if(input.ScreenType != ScreenType.Pause) return;
            
            if (input.RequestType == EventRequestType.Granted)
            {
                EnablePause();
            }
        }
        
        #endregion
        
        #endregion
    }
}