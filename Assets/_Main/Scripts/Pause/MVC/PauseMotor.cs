using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Pause
{
    public class PauseMotor : ObservableComponent
    {
        private bool _isGoingToMainMenu;
        private bool _isEnable;
        
        public void StartDisable()
        {
            NotifyAll(PauseObserverMessage.StartDisable);
        }

        public void Enable()
        {
            if(_isEnable) return;
            
            _isEnable = true;
            NotifyAll(PauseObserverMessage.Enable);
        }

        public void Initialize()
        {
            NotifyAll(PauseObserverMessage.Initialize);
        }

        public void ExecuteDisable()
        {
            if (_isGoingToMainMenu)
            {
                TryRestartEarth();
                return;
            }
            
            _isEnable = false;
            NotifyAll(PauseObserverMessage.ExecuteDisable);
        }

        private void TryRestartEarth()
        {
            NotifyAll(PauseObserverMessage.RestartEarth);
            _isGoingToMainMenu = false;
        }

        #region Screens
        
        public void TriggerOptionsMenu()
        {
            NotifyAll(PauseObserverMessage.Options);
        }
        
        public void TriggerLoadMainMenu()
        {
            _isGoingToMainMenu = true;
            NotifyAll(PauseObserverMessage.LoadMainMenu);
        }
        
        public void TriggerGameMode()
        {
            NotifyAll(PauseObserverMessage.GameMode);
        }
        
        #endregion
    }
}