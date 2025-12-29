using System;

namespace MeteorMadness.GlobalValues.Tools
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public class FPSCounter
    {
        private readonly float _updateInterval = 0.5f;
        private int _frameCount = 0;
        private float _elapsedTime = 0;
        private float _current = 0;
        private float _sum = 0;
        private int _samples = 0;
        private float _min = float.MaxValue;
        private float _max = 0;
        
        public float Current => _current;
        public float AVG => _samples > 0 ? _sum/_samples : 0;
        public float Max => _samples > 0 ? _max : 0;
        public float Min => _samples > 0 ? _min : 0;

        public FPSCounter(float elapsedTime)
        {
            _elapsedTime = elapsedTime;
        }

        public void Update(float deltaTime)
        {
            _frameCount++;
            _elapsedTime += deltaTime;

            if (_elapsedTime >= _updateInterval)
            {
                _current = _frameCount / _elapsedTime;
                
                _sum += _current;
                _samples++;
                
                if(_current < _min) _min = _current;
                if(_current > _max) _max = _current;
                
                _frameCount = 0;
                _elapsedTime = 0;
            }
        }
    }
#endif

}