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

        public UnityAction OnEnd;
        public UnityAction OnStart;

        /// <summary>
        /// Set Time in Seconds
        /// </summary>
        /// <param name="time"></param>
        public void Set(float time)
        {
            _currentTime = time;
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