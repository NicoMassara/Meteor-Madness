using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts
{
    public class Timer
    {
        private float _currentTime = -1;
        public bool GetHasEnded => _currentTime is <= 0 and > -1;
        public float CurrentTime => _currentTime;
        private bool _hasStarted = false;
        private float _aboutToEndRatio;
        private float _timerTime;

        public UnityAction OnEnd;
        public UnityAction OnStart;
        public UnityAction OnAboutToEnd;

        /// <summary>
        /// Set Time in Seconds
        /// </summary>
        /// <param name="time"></param>
        /// <param name="aboutToEndRatio">Time Ratio Left to trigger 'OnAboutToEnd' event</param>
        public void Set(float time, float aboutToEndRatio = -1)
        {
            _currentTime = time;
            _timerTime = _currentTime; 
            _aboutToEndRatio = aboutToEndRatio;
        }

        public void Run()
        {
            if (_currentTime > 0)
            {
                if (_hasStarted == false)
                {
                    OnStart?.Invoke();
                    _hasStarted = true;
                }

                _currentTime -= Time.deltaTime;

                //This trigger when the ratio is higher or lower.
                //Needs to be higher than 0 to trigger and equal or lower to 1.
                //The input means the time that's left to trigger the event
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_aboutToEndRatio > 0)
                {
                    var isAboutToFinish = (_currentTime/_timerTime) <= _aboutToEndRatio;

                    if (isAboutToFinish)
                    {
                        OnAboutToEnd?.Invoke();
                    }
                }

                
                if (_currentTime <= 0)
                {
                    OnEnd?.Invoke();
                }
            }
        }

        public bool HasEnded()
        {
            Run();
            return GetHasEnded;
        }

        public void Reset()
        {
            _currentTime = -1;
        }
    }
}