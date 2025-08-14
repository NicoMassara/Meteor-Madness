namespace _Main.Scripts.Gameplay.FSM.Level
{
    public class LevelPlayState<T> : LevelBaseState<T>
    {
        public override void Awake()
        {
            GameManager.Instance.CanPlay = true;
            Controller.OnDeath += OnDeathHandler;
        }

        public override void Execute()
        {
            Controller.RunSpawnTimer();
            if (Controller.HasSpawnTimerEnd())
            {
                Controller.SpawnMeteor();
                Controller.RestartSpawnTimer();
            }
        }

        public override void Sleep()
        {
            GameManager.Instance.CanPlay = false;
            Controller.RestartSpawnTimer();
        }
        
        private void OnDeathHandler(int points)
        {
            Controller.OnDeath -= OnDeathHandler;
            Controller.DeathTransition();
        }
    }
}