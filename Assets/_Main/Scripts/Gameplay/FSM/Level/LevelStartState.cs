using UnityEngine;

namespace _Main.Scripts.Gameplay.FSM.Level
{
    public class LevelStartState<T> : LevelBaseState<T>
    {
        private float _startTimer;
        
        public override void Awake()
        {
            Controller.TriggerStart();
            _startTimer = GameValues.StartGameCount + 1;
        }
        
        public override void Execute()
        {
            _startTimer -= Time.deltaTime;
            if (_startTimer <= 0)
            {
                Controller.PlayTransition();
            }
        }

        public override void Sleep()
        {

        }
    }
}