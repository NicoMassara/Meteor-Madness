using MeteorMadness.GlobalValues;
using MeteorMadness.GlobalValues.Events;
using MeteorMadness.Managers;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Settings
{
    [RequireComponent(typeof(SettingsView))]
    [RequireComponent(typeof(SettingsUiView))]
    [RequireComponent(typeof(SettingsViewAnimation))]
    public class SettingsSetup : MonoBehaviour
    {
        private SettingsView _view;
        private SettingsUiView _ui;
        private SettingsViewAnimation _animator;
        private SettingsMotor _motor;

        private void Awake()
        {
            _motor = new SettingsMotor();
            
            _view = GetComponent<SettingsView>();
            _ui = GetComponent<SettingsUiView>();
            _animator = GetComponent<SettingsViewAnimation>();
            
            _motor.Subscribe(_view);
            _motor.Subscribe(_ui);
            _motor.Subscribe(_animator);
            
            SetViewHandlers();
            
            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
        }

        private void SetViewHandlers()
        {
            _view.OnEnable += () => _motor.Initial();
            //
            _ui.OnVolumeChanged += (value) => _motor.Volume(value);
            _ui.OnLanguageChanged += (value) => _motor.Language(value);
            _ui.OnVibrationChanged += (value) => _motor.Vibration(value);
            _ui.OnBackButtonPressed += () => _motor.Close();
            //
            _animator.OnPanelOpened += () => AdsEvents.Banner_TriggerShow();;
            _animator.OnPanelClosed += () => _motor.ExecuteDisable();
        }
        
        #region Event Bus

        #region GameScreen

        private void EventBus_GameScreen_Disable(GameScreenEvents.DisableScreen input)
        {
            if(input.ScreenType != ScreenType.OptionsMenu) return;
            
            if (input.RequestType == EventRequestType.Requested)
            {
                _motor.StartDisable();
            }
        }

        private void EventBus_GameScreen_Enable(GameScreenEvents.EnableScreen input)
        {
            if(input.ScreenType != ScreenType.OptionsMenu) return;
            
            if (input.RequestType == EventRequestType.Granted)
            {
                _motor.Enable();
            }
        }

        #endregion

        #endregion
    }
}