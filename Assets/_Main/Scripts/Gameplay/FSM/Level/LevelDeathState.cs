using UnityEngine;

namespace _Main.Scripts.Gameplay.FSM.Level
{
    public class LevelDeathState<T> : LevelBaseState<T>
    {
        private readonly Timer _destructionTimer  = new Timer();
        private readonly Timer _shakeTimer  = new Timer();
        
        public override void Awake()
        {
            Controller.EndLevel();
            Controller.StartEarthShake();
            _shakeTimer.Set(GameTimeValues.DeathShakeTime);
            _shakeTimer.OnEnd += ShakeTimer_OnEndHandler;
        }

        public override void Execute()
        {
            _shakeTimer.Run();
            _destructionTimer.Run();
        }

        public override void Sleep()
        {
            _destructionTimer.OnEnd -= DestructionTimer_OnEndHandler;
            _shakeTimer.OnEnd -= ShakeTimer_OnEndHandler;
            Controller.RestartLevel();
        }
        
        private void ShakeTimer_OnEndHandler()
        {
            _destructionTimer.Set(GameTimeValues.DestructionTimeOnDeath);
            Controller.StopEarthShake();
            _destructionTimer.OnEnd += DestructionTimer_OnEndHandler;
        }
        
        private void DestructionTimer_OnEndHandler()
        {

            Controller.TriggerEarthDestruction();
        }
    }
}