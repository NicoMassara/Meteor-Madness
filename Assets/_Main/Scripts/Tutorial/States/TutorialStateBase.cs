using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Tutorial.MVC;

namespace _Main.Scripts.Tutorial.States
{
    public class TutorialStateBase<T> : State<T>
    {
        protected TutorialController Controller { get; private set; }

        public void Initialize(TutorialController controller)
        {
            Controller = controller;
        }
    }
}