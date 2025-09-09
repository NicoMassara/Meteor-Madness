using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay.Shield;

namespace _Main.Scripts.Gameplay.FSM.Shield
{
    public class ShieldBaseState<T> : State<T>
    {
        protected ShieldController Controller { get; private set; }

        public void Initialize(ShieldController controller)
        {
            Controller = controller;
        }
    }
}