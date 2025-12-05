using System;
using _Main.Scripts.Managers;
using UnityEngine;

namespace _Main.Scripts.Pause
{
    public class PauseSetup : MonoBehaviour
    {
        private PauseViewUI.IPauseViewUI _ui;
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
            var ui = GetComponent<PauseViewUI>();
            var anim = GetComponent<PauseViewAnimation>();
            
            _motor.Subscribe(ui);
            _motor.Subscribe(anim);

            _ui = ui;
            _animation = anim;
            
            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
            
            SetViewHandlers();
        }

        private void SetViewHandlers()
        {
            _ui.OnMainMenuButtonPressed += GameManager.Instance.LoadMainMenu;
            _ui.OnOptionsButtonPressed += GameManager.Instance.LoadOptionsMenu;
            _ui.OnResumeButtonPressed += GameManager.Instance.LoadGameMode;
            //
            _animation.OnPanelOpened += _motor.Enable;
            _animation.OnPanelClosed += () =>
            {
                GameScreenEventCaller.DisableScreen(ScreenType.Pause, EventRequestType.Granted);
            };
        }

        #region Enable / Disable

        private void DisablePause()
        {
            _motor.StartDisable();
        }
        
        private void EnablePause()
        {
            _motor.Initialize();
        }

        #endregion

        #region Event Bus
        
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
    }
}