using _Main.Scripts.Observer;

namespace _Main.Scripts.Pause
{
    public class PauseMotor : ObservableComponent
    {
        private bool _isGoingToMainMenu;
        
        public void StartDisable()
        {
            NotifyAll(PauseObserverMessage.StartDisable);
        }

        public void Enable()
        {
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