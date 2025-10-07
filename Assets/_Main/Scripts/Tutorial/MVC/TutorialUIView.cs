using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;

namespace _Main.Scripts.Tutorial.MVC
{
    public class TutorialUIView : ManagedBehavior, IObserver
    {
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case TutorialObserverMessage.Start:
                    break;
                case TutorialObserverMessage.FirstMovement:
                    break;
                case TutorialObserverMessage.SecondMovement:
                    break;
                case TutorialObserverMessage.AbilityTest:
                    break;
                case TutorialObserverMessage.Finish:
                    break;
            }
        }
    }
}