using _Main.Scripts.Observer;

namespace _Main.Scripts.Tutorial.MVC
{
    public class TutorialMotor : ObservableComponent
    {
        
        public void Start()
        {
            NotifyAll(TutorialObserverMessage.Start);
        }

        public void Movement()
        {
            NotifyAll(TutorialObserverMessage.Movement);
        }

        public void Ability()
        {
            NotifyAll(TutorialObserverMessage.Ability);
        }

        public void Finish()
        {
            NotifyAll(TutorialObserverMessage.Finish);
        }
        
        public void Disable()
        {
            NotifyAll(TutorialObserverMessage.Disable);
        }

        public void Enable()
        {
            NotifyAll(TutorialObserverMessage.Enable);
        }

        public void SpawnExtraMeteors()
        {
            NotifyAll(TutorialObserverMessage.ExtraMeteors);
        }
    }
}