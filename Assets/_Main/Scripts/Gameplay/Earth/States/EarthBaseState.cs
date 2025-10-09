using _Main.Scripts.FiniteStateMachine;

namespace _Main.Scripts.Gameplay.Earth.States
{
    public class EarthBaseState<T> : State<T>
    {
        protected EarthController Controller { get; private set; }

        public void Initialize(EarthController controller)
        {
            this.Controller = controller;
        }
    }
}