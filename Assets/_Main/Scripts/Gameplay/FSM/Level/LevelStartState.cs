using UnityEngine;

namespace _Main.Scripts.Gameplay.FSM.Level
{
    public class LevelStartState<T> : LevelBaseState<T>
    {
        private readonly Timer _startTimer = new Timer();
        
        public override void Awake()
        {
            Controller.TriggerStart();
            _startTimer.Set(GameValues.StartGameCount + 1);
            _startTimer.OnEnd += StartTimer_OnEndHandler;
        }

        public override void Execute()
        {
            _startTimer.Run();
        }
        
        private void StartTimer_OnEndHandler()
        {
            Controller.PlayTransition();
        }
    }
}