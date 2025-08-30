using UnityEngine;

namespace _Main.Scripts.UI.FSM.Level
{
    public class LevelUIPlayState<T> : LevelUIBaseState<T>
    {
        private float _elapsedTime;
        private int _targetPoints;
        private float _displayedPoints;
        private const float Increase_Time = GameTimeValues.PointsTextTimeToIncrease;
        private bool _isCountingPoints = false;

        public override void Awake()
        {
            Controller.SetActivePlayPanel();
            Controller.OnPointsChanged += OnPointsChangedHandler;
            _displayedPoints = 0;
            _targetPoints = 0;
        }

        public override void Execute()
        {
            if(_isCountingPoints == false) return;
            
            _elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsedTime / Increase_Time);
            _displayedPoints = Mathf.Lerp(0, _targetPoints, t);
            _displayedPoints = Mathf.Clamp(_displayedPoints, 0, _targetPoints);
            
            Controller.UpdatePointsText(Mathf.RoundToInt(_displayedPoints));
            
            if (_displayedPoints >= _targetPoints)
            {
                _isCountingPoints = false;
            }
        }

        public override void Sleep()
        {
            Controller.OnPointsChanged -= OnPointsChangedHandler;
        }
        
        private void OnPointsChangedHandler(int amount)
        {
            _displayedPoints = Mathf.RoundToInt(_elapsedTime >= Increase_Time ? _targetPoints : Controller.GetDisplayedPointsFromText());
            _targetPoints = amount;
            _isCountingPoints = true;
            _elapsedTime = 0f;
        }
    }
}