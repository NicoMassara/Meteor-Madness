namespace _Main.Scripts.Gameplay.GameMode.States
{
    public class GameModeRestartState<T> : GameModeStateBase<T>
    {
        public override void Awake()
        {
            Controller.GameRestart();
        }
    }
}