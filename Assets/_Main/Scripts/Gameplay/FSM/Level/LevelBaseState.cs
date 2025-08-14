using _Main.Scripts.FiniteStateMachine;

namespace _Main.Scripts.Gameplay.FSM.Level
{
    public abstract class LevelBaseState<T> : State<T>
    {
        protected LevelController Controller { get; private set; }

        public void Initialize(LevelController levelController)
        {
            this.Controller = levelController;
        }
    }
}