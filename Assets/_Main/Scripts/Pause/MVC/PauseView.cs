using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Pause
{
    public class PauseView : ManagedBehavior, IObserver,
        PauseView.IPauseView
    {
        public interface IPauseView
        {

        }
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case PauseObserverMessage.ExecuteDisable:
                    HandleExecuteDisable();
                    break;
                
                case PauseObserverMessage.RestartEarth:
                    HandleRestartEarth();
                    break;
                
                //=== Screens ===
                case PauseObserverMessage.Options:
                    HandleOptions();
                    break;
                
                case PauseObserverMessage.LoadMainMenu:
                    HandleMainMenu();
                    break;
                
                case PauseObserverMessage.GameMode:
                    HandleGameMode();
                    break;
            }
        }

        private void HandleRestartEarth()
        {
            EarthEventCaller.Restart();
        }

        #region Screens
        
        private void HandleMainMenu()
        {
            GameManager.Instance.LoadMainMenu();
        }

        private void HandleGameMode()
        {
            GameManager.Instance.LoadGameMode();
        }

        private void HandleOptions()
        {
            GameManager.Instance.LoadOptionsMenu();
        }
        
        #endregion

        private void HandleExecuteDisable()
        {
            GameScreenEventCaller.DisableScreen(ScreenType.Pause, EventRequestType.Granted);
        }
    }
}