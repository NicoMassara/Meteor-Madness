namespace _Main.Scripts.GameScreens.States
{
    public class GameScreenGameplayState<T> : GameScreenStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetActiveGameplay();
        }
    }
}