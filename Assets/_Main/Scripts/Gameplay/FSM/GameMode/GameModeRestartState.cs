namespace _Main.Scripts.Gameplay.FSM.GameMode
{
    public class GameModeRestartState<T> : GameModeStateBase<T>
    {
        public override void Awake()
        {
            Controller.GameRestart();
        }
    }
}