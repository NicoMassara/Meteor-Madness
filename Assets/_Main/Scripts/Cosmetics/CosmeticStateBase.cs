using _Main.Scripts.Cosmetics.MVC;
using _Main.Scripts.FiniteStateMachine;

namespace _Main.Scripts.Cosmetics
{
    public class CosmeticStateBase<T> : State<T>
    {
        protected CosmeticController Controller { get; private set; }

        public void Initialize(CosmeticController controller)
        {
            Controller = controller;
        }
    }
}