using UnityEngine;

namespace _Main.Scripts.UI.FSM.Level
{
    public class LevelUIDeathState<T> : LevelUIBaseState<T>
    {
        private float _elapsedTime;
        private int _targetPoints;
        private float _displayedPoints;
        private bool _isCountingPoints = false;

        private readonly Timer _displayPanelTimer = new Timer();
        private readonly Timer _countPointsTimer = new Timer();
        private readonly Timer _enableRestartTimer = new Timer();
        
        public override void Awake()
        {
            Controller.DisableCurrentPanel();
            Controller.SetActiveRestartSubPanel(false);
            _elapsedTime = 0;
            _displayedPoints = 0;
            Controller.OnDestruction += OnDestructionHandler;
            _displayPanelTimer.OnEnd += DisplayPanelTimer_OnEndHandler;
            _enableRestartTimer.OnEnd += EnableRestartTimer_OnEndHandler;
        }

        public override void Execute()
        {
            if (_displayPanelTimer.HasEnded())
            {
                if (_countPointsTimer.HasEnded())
                {
                    if (_isCountingPoints)
                    {
                        HandlePoints();
                    }
                    else
                    {
                        _enableRestartTimer.Run();
                    }
                }
            }
        }

        public override void Sleep()
        {
            Controller.OnDestruction -= OnDestructionHandler;
            _displayPanelTimer.OnEnd -= DisplayPanelTimer_OnEndHandler;
            _enableRestartTimer.OnEnd -= EnableRestartTimer_OnEndHandler;
        }

        private void HandlePoints()
        {
            _elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsedTime / GameTimeValues.PointsTextTimeToIncreaseOnDeath);
            _displayedPoints = Mathf.Lerp(_displayedPoints, _targetPoints, t);
            
            if (_displayedPoints >= _targetPoints)
            {
                _displayedPoints = _targetPoints;
            }
            
            Controller.UpdateDeathPointsText(Mathf.RoundToInt(_displayedPoints));
            
            if (t >= 0.85f)
            {
                _isCountingPoints = false;
            } 
        }

        private void OnDestructionHandler()
        {
            _displayPanelTimer.Set(UITimeValues.EnableDeathPanel);
        }
        
        private void DisplayPanelTimer_OnEndHandler()
        {
            _targetPoints = Controller.GetDisplayedPoints();
            _countPointsTimer.Set(UITimeValues.StartCountingPointsOnDeath);
            _enableRestartTimer.Set(UITimeValues.EnableRestartButtonOnDeath);
            if (_targetPoints > 0)
            {
                _isCountingPoints = true;
            }
            Controller.UpdateDeathPointsText(0);
            Controller.EndLevel();
        }
        
        private void EnableRestartTimer_OnEndHandler()
        {
            Controller.SetActiveRestartSubPanel(true);
        }
    }
}