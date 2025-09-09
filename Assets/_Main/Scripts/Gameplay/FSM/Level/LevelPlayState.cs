namespace _Main.Scripts.Gameplay.FSM.Level
{
    public class LevelPlayState<T> : LevelBaseState<T>
    {
        private readonly Timer _spawnTimer = new Timer();
        
        public override void Awake()
        {
            GameManager.Instance.CanPlay = true;
            Controller.OnEnd += OnEndHandler;
            _spawnTimer.OnEnd += SpawnTimer_OnEndHandler;
            _spawnTimer.Set(0.25f);
        }

        public override void Execute()
        {
            _spawnTimer.Run();
        }

        public override void Sleep()
        {
            GameManager.Instance.CanPlay = false;
        }

        private void SetTimer()
        {
            _spawnTimer.Set(GameTimeValues.MeteorSpawnDelay);
        }

        #region Handlers

        private void OnEndHandler(int points)
        {
            Controller.OnEnd -= OnEndHandler;
            _spawnTimer.OnEnd -= SpawnTimer_OnEndHandler;

            Controller.DeathTransition();
        }

        private void SpawnTimer_OnEndHandler()
        {
            Controller.SpawnMeteor();
            SetTimer();
        }

        #endregion
        

    }
}