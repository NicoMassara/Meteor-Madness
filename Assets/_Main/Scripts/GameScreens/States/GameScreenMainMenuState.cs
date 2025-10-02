namespace _Main.Scripts.GameScreens.States
{
    public class GameScreenMainMenuState<T> : GameScreenStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetActiveMainMenu();
        }
    }
}