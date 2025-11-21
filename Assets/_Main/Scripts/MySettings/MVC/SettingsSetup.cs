using _Main.Scripts.Managers;
using UnityEngine;

namespace _Main.Scripts.MySettings.MVC
{
    [RequireComponent(typeof(SettingsView))]
    [RequireComponent(typeof(SettingsUiView))]
    public class SettingsSetup : MonoBehaviour
    {
        private SettingsView _view;
        private SettingsUiView _ui;
        private SettingsMotor _motor;

        private void Awake()
        {
            _motor = new SettingsMotor();
            
            _view = GetComponent<SettingsView>();
            _ui = GetComponent<SettingsUiView>();
            
            _motor.Subscribe(_view);
            _motor.Subscribe(_ui);
            
            
            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
        }
        
        private void Start()
        {
            SetViewHandlers();
            SetUIHandlers();
        }

        private void SetViewHandlers()
        {
            _view.OnEnable += () =>
            {
                _motor.Initial();
            };
        }

        private void SetUIHandlers()
        {
            _ui.OnVolumeChanged += (value) =>
            {
                _motor.Volume(value);
            };
            
            _ui.OnLanguageChanged += (value) =>
            {
                _motor.Language(value);
            };
            
            _ui.OnVibrationChanged += (value) =>
            {
                _motor.Vibration(value);
            };
            
            _ui.OnBackButtonPressed += () =>
            {
                _motor.Close();
            };
        }



        #region Event Bus

        #region GameScreen

        private void EventBus_GameScreen_Disable(GameScreenEvents.DisableScreen input)
        {
            if(input.ScreenType != ScreenType.OptionsMenu) return;
            
            if (input.RequestType == EventRequestType.Requested)
            {
                _motor.Disable();
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