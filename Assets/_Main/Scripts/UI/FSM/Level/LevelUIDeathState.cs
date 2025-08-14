using UnityEngine;

namespace _Main.Scripts.UI.FSM.Level
{
    public class LevelUIDeathState<T> : LevelUIBaseState<T>
    {
        private GameObject canvasPanel;
        private float _elapsedTime;
        private int _targetPoints;
        private float _displayedPoints;
        private const float Increase_Time = GameValues.PointsTextTimeToIncreaseOnDeath;
        private bool _isCountingPoints = false;

        public LevelUIDeathState(GameObject canvasPanel)
        {
            this.canvasPanel = canvasPanel;
        }
        
        public override void Awake()
        {
            Controller.ChangeCurrentPanel(canvasPanel);
            Controller.UpdateDeathPointsText(0);
            _targetPoints = Controller.GetFinalPoints();
            _isCountingPoints = true;
        }

        public override void Execute()
        {
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