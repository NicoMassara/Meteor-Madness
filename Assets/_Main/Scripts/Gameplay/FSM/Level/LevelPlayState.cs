namespace _Main.Scripts.Gameplay.FSM.Level
{
    public class LevelPlayState<T> : LevelBaseState<T>
    {
        public override void Awake()
        {
            GameManager.Instance.CanPlay = true;
            Controller.OnEnd += OnEndHandler;
            Controller.OnShieldHit += OnMeteorHitHandler;
            Controller.OnEarthHit += OnMeteorHitHandler;
            
            Controller.SpawnMeteor();
        }

        public override void Execute()
        {
            
        }

        public override void Sleep()
        {
            Controller.OnShieldHit -= OnMeteorHitHandler;
            Controller.OnEarthHit -= OnMeteorHitHandler;
            GameManager.Instance.CanPlay = false;
        }
        
        private void OnMeteorHitHandler(int arg0)
        {
            Controller.SpawnMeteor();
        }

        
        private void OnEndHandler(int points)
        {
            Controller.OnEnd -= OnEndHandler;

            Controller.DeathTransition();
        }
    }
}