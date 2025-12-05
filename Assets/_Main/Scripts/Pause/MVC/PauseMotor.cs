using _Main.Scripts.Observer;

namespace _Main.Scripts.Pause
{
    public class PauseMotor : ObservableComponent
    {
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
        
    }
}