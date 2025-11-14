using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts
{
    public class Timer
    {
        private float _currentTime = -1;
        public bool HasEnded { get; private set; } 
        public float CurrentTime => _currentTime;
        public float CurrentRatio => _currentTime / _targetTime;
        private bool _hasStarted = false;
        private float _aboutToEndRatio;
        private float _targetTime;

        public UnityAction OnEnd;
        public UnityAction OnStart;
        public UnityAction OnAboutToEnd;


        public Timer()
        { }

        public Timer(TimerData timerData)
        {
            Set(timerData.Time);
            OnStart = timerData.OnStartAction;
            OnEnd = timerData.OnEndAction;
        }

        /// <summary>
        /// Set Time in Seconds
        /// </summary>
        /// <param name="time"></param>
        /// <param name="aboutToEndRatio">Time Ratio Left to trigger 'OnAboutToEnd' event</param>
        public void Set(float time, float aboutToEndRatio = -1)
        {
            _currentTime = Mathf.Clamp(time, 0.001f, float.MaxValue);
            _targetTime = _currentTime; 
            _aboutToEndRatio = aboutToEndRatio;
            HasEnded = false;
        }

        public void Run(float deltaTime)
        {
            if (_currentTime > 0)
            {
                if (_hasStarted == false)
                {
                    OnStart?.Invoke();
                    _hasStarted = true;
                }

                _currentTime -= deltaTime;

                //This trigger when the ratio is higher or lower.
                //Needs to be higher than 0 to trigger and equal or lower to 1.
                //The input means the time that's left to trigger the event
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_aboutToEndRatio > 0)
                {
                    var isAboutToFinish = (_currentTime/_targetTime) <= _aboutToEndRatio;

                    if (isAboutToFinish)
                    {
                        OnAboutToEnd?.Invoke();
                    }
                }
                
                if (_currentTime <= 0)
                {
                    HasEnded = true;
                    Reset();
                    OnEnd?.Invoke();
                }
            }
        }
        
        public void Reset()
        {
            _currentTime = -1;
        }
    }
    
    public class TimerData
    {
        public float Time;
        public UnityAction OnEndAction;
        public UnityAction OnStartAction;
    }
}