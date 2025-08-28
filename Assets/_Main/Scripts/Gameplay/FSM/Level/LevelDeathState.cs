using UnityEngine;

namespace _Main.Scripts.Gameplay.FSM.Level
{
    public class LevelDeathState<T> : LevelBaseState<T>
    {
        private readonly Timer _destructionTimer  = new Timer();
        private readonly Timer _shakeTimer  = new Timer();
        private readonly Timer _startShakeTimer  = new Timer();
        
        public override void Awake()
        {
            Controller.EndLevel();

            _startShakeTimer.OnEnd += StartShakeTimer_OnEndHandler;
            _startShakeTimer.Set(GameTimeValues.StartShake);
        }

        public override void Execute()
        {
            _startShakeTimer.Run();
            _shakeTimer.Run();
            _destructionTimer.Run();
        }

        public override void Sleep()
        {
            _destructionTimer.OnEnd -= DestructionTimer_OnEndHandler;
            _shakeTimer.OnEnd -= ShakeTimer_OnEndHandler;
            Controller.RestartLevel();
        }
        
        private void StartShakeTimer_OnEndHandler()
        {
            Controller.StartEarthShake();
            _shakeTimer.Set(GameTimeValues.DeathShake);
            _shakeTimer.OnEnd += ShakeTimer_OnEndHandler;
        }
        
        private void ShakeTimer_OnEndHandler()
        {
            _destructionTimer.Set(GameTimeValues.DestructionOnDeath);
            Controller.StopEarthShake();
            _destructionTimer.OnEnd += DestructionTimer_OnEndHandler;
        }
        
        private void DestructionTimer_OnEndHandler()
        {
            Controller.TriggerEarthDestruction();
        }
    }
}