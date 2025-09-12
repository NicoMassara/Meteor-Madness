using _Main.Scripts.Managers;

namespace _Main.Scripts.Gameplay.FSM.GameMode
{
    public class GameModeGameplayState<T> : GameModeStateBase<T>
    {
        private readonly Timer _timer = new Timer();

        public override void Awake()
        {
            _timer.Set(1f);
            _timer.OnEnd += Timer_OnEndHandler;
        }

        public override void Execute(float deltaTime)
        {
            _timer.Run(deltaTime);
        }

        public override void Sleep()
        {
            _timer.OnEnd -= Timer_OnEndHandler;
        }

        private void Timer_OnEndHandler()
        {
            _timer.Set(GameTimeValues.MeteorSpawnDelay);
            Controller.SpawnSingleMeteor();
        }
    }
}