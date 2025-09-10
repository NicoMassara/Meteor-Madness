using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay.GameMode;

namespace _Main.Scripts.Gameplay.FSM.GameMode
{
    public class GameModeStateBase<T> : State<T>
    {
        protected GameModeController Controller { get; private set; }

        public void Initialize(GameModeController controller)
        {
            this.Controller = controller;
        }
    }
}