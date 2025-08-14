using _Main.Scripts.FiniteStateMachine;

namespace _Main.Scripts.UI.FSM.Level
{
    public abstract class LevelUIBaseState<T> : State<T>
    {
        protected LevelUIController Controller { get; private set; }

        public void Initialize(LevelUIController levelUIController)
        {
            Controller = levelUIController;
        }
    }
}