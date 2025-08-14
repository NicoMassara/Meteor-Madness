using UnityEngine;

namespace _Main.Scripts.UI.FSM.Level
{
    public class LevelUIPlayState<T> : LevelUIBaseState<T>
    {
        private float _elapsedTime;
        private int _targetPoints;
        private float _displayedPoints;
        private const float Increase_Time = GameValues.PointsTextTimeToIncrease;
        private bool _isCountingPoints = false;

        public override void Awake()
        {
            Controller.SetActivePlayPanel();
            Controller.OnPointsChanged += OnPointsChangedHandler;
        }

        public override void Execute()
        {
            if(_isCountingPoints == false) return;
            
            _elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsedTime / Increase_Time);
            _displayedPoints = Mathf.Lerp(_displayedPoints, _targetPoints, t);
            
            if (_displayedPoints >= _targetPoints)
            {
                _displayedPoints = _targetPoints;
            }
            
            Controller.UpdatePointsText(Mathf.RoundToInt(_displayedPoints));
            
            if (t >= 1f)
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
            _targetPoints = amount * GameValues.VisualMultiplier;
            _isCountingPoints = true;
            _elapsedTime = 0f;
        }
    }
}