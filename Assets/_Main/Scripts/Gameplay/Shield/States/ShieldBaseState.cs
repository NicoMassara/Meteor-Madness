using _Main.Scripts.FiniteStateMachine;

namespace _Main.Scripts.Gameplay.Shield.States
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