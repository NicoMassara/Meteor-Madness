using UnityEngine;

namespace _Main.Scripts.UI.FSM.Level
{
    public class LevelUIDeathState<T> : LevelUIBaseState<T>
    {
        private float _elapsedTime;
        private int _targetPoints;
        private float _displayedPoints;
        private const float Increase_Time = GameValues.PointsTextTimeToIncreaseOnDeath;
        private bool _isCountingPoints = false;

        private float _startCountingDelay = 0.5f;
        
        public override void Awake()
        {
            Controller.SetActiveDeathPanel();
            Controller.UpdateDeathPointsText(0);
            _targetPoints = Controller.GetDisplayedPoints();
            _isCountingPoints = true;
        }

        public override void Execute()
        {
            if (_startCountingDelay > 0) 
            {
                _startCountingDelay -= Time.deltaTime;
                return;
            }

            if(_isCountingPoints == false) return;
            //
            
            _elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsedTime / Increase_Time);
            _displayedPoints = Mathf.Lerp(_displayedPoints, _targetPoints, t);
            
            if (_displayedPoints >= _targetPoints)
            {
                _displayedPoints = _targetPoints;
            }
            
            Controller.UpdateDeathPointsText(Mathf.RoundToInt(_displayedPoints));
            
            if (t >= 1f)
            {
                _isCountingPoints = false;
            }
        }

        public override void Sleep()
        {

        }
    }
}