namespace _Main.Scripts.GameScreens.States
{
    public class GameScreenTutorialState<T> : GameScreenStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetActiveTutorial();
        }
    }
}