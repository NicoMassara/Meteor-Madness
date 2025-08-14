using UnityEngine;

namespace _Main.Scripts.Gameplay.FSM.Level
{
    public class LevelDeathState<T> : LevelBaseState<T>
    {
        private float _timer;
        private int _sleepTime = 5;
        
        public override void Awake()
        {
            _timer = _sleepTime;
        }

        public override void Execute()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                Controller.StartTransition();
            }
        }

        public override void Sleep()
        {
            Controller.RestartLevel();
        }
    }
}