using _Main.Scripts.FiniteStateMachine;

namespace _Main.Scripts.GameScreens.States
{
    public class GameScreenStateBase<T> : State<T>
    {
        protected GameScreenController Controller { get; private set; }

        public void Initialize(GameScreenController controller)
        {
            Controller = controller;
        }
    }
}