namespace _Main.Scripts.Gameplay.FSM.GameMode
{
    public class GameModeStartState<T> : GameModeStateBase<T>
    {
        public override void Awake()
        {
            Controller.RestartValues();
            Controller.StartCountdown();
        }

        public override void Execute(float deltaTime)
        {
            Controller.HandleCountdownTimer(deltaTime);
        }

        public override void Sleep()
        {
            Controller.StartGameplay();
        }
    }
}