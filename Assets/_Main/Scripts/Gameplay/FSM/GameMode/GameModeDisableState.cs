namespace _Main.Scripts.Gameplay.FSM.GameMode
{
    public class GameModeDisableState<T> : GameModeStateBase<T>
    {
        public override void Awake()
        {
            Controller.DisableGameMode();
        }
    }
}