using System;
using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Settings
{
    public class SettingsView : MonoBehaviour, IObserver
    {
        public event Action OnEnable;
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case SettingsObserverMessage.Enable:
                    HandleEnable();
                    break;
                case SettingsObserverMessage.Disable:
                    HandleDisable();
                    break;
                case SettingsObserverMessage.Initial:
                    HandleInitial();
                    break;
                case SettingsObserverMessage.Close:
                    HandleClose();
                    break;
                case SettingsObserverMessage.Language:
                    HandleLanguage((int)args[0]);
                    break;
#if UNITY_ANDROID
                case SettingsObserverMessage.Vibration:
                    HandleVibration((bool)args[0]);
                    break;
#endif
                case SettingsObserverMessage.Volume:
                    HandleVolume((float)args[0]);
                    break;
            }
        }

        #region Enable/Disable
        
        private void HandleEnable()
        {
            OnEnable?.Invoke();
        }

        private void HandleDisable()
        {
            GameScreenEventCaller.DisableScreen(ScreenType.OptionsMenu, EventRequestType.Granted);
        }

        private void HandleInitial()
        {

        }
        
        private void HandleClose()
        {
            SettingsManager.Instance.SaveSettings();
            GameManager.Instance.LoadLastScreen();
        }

        #endregion

        #region Changers

        private void HandleLanguage(int languageIndex)
        {
            SettingsManager.Instance.SetLanguageIndex(languageIndex);
        }

#if UNITY_ANDROID

        private void HandleVibration(bool isEnable)
        {

            SettingsManager.Instance.SetVibration(isEnable);
        }
        
#endif

        private void HandleVolume(float volume)
        {
            SettingsManager.Instance.SetMasterVolume(volume);
        }

        #endregion
        
    }
}