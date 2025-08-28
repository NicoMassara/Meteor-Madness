using System;
using UnityEngine;

namespace _Main.Scripts
{
    public class Oscillator
    {
        private readonly float _speed;
        private readonly float _amplitude;
        private readonly float _offset;

        public Oscillator(float speed, float amplitude, float offset)
        {
            _speed = speed;
            _amplitude = amplitude;
            _offset = offset;
        }

        public float OscillateCos()
        {
            return _offset + MathF.Cos(Time.time * _speed) * _amplitude;
        }
        
        public float OscillateSin()
        {
            return _offset + MathF.Sin(Time.time * _speed) * _amplitude;
        }
    }
}