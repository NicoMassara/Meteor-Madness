using _Main.Scripts.FiniteStateMachine;

namespace _Main.Scripts.UI.FSM.Level
{
    public abstract class LevelUIBaseState<T> : State<T>
    {
        protected LevelUI Controller { get; private set; }

        public void Initialize(LevelUI levelUI)
        {
            Controller = levelUI;
        }
    }
}