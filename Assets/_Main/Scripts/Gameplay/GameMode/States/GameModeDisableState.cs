namespace _Main.Scripts.Gameplay.GameMode.States
{
    public class GameModeDisableState<T> : GameModeStateBase<T>
    {
        public override void Awake()
        {
            Controller.DisableGameMode();
        }
    }
}