using UnityEngine;

namespace _Main.Scripts.Gameplay.FSM.Level
{
    public class LevelDeathState<T> : LevelBaseState<T>
    {
        private readonly Timer _destructionTimer  = new Timer();
        
        public override void Awake()
        {
            _destructionTimer.Set(GameTimeValues.DestructionTimeOnDeath);
            _destructionTimer.OnEnd += DestructionTimer_OnEndHandler;
        }

        public override void Execute()
        {
            _destructionTimer.Run();
        }

        public override void Sleep()
        {
            Controller.RestartLevel();
        }
        
        private void DestructionTimer_OnEndHandler()
        {
            Controller.TriggerEarthDestruction();
        }
    }
}