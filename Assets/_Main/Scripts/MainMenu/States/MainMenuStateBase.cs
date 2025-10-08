using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.MainMenu.MVC;

namespace _Main.Scripts.MainMenu.States
{
    public class MainMenuStateBase<T> : State<T>
    {
        protected MainMenuController Controller { get; private set; }

        public void Initialize(MainMenuController controller)
        {
            Controller = controller;
        }
    }
}