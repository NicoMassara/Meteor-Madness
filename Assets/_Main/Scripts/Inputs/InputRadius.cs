using System;
using UnityEngine;

namespace _Main.Scripts.Inputs
{
    public class InputRadius
    {
        private readonly float _offset;
        private float _currentAngle;
        private float _currentMagnitude;
        
        public event Action<float> OnDirectionChanged;
        public event Action<float> OnMagnitudeChanged;

        public InputRadius(float offset = 0f)
        {
            _offset = offset;
        }

        public void SetAngleFromDirection(Vector2 direction)
        {
            var inputAngle = InputHelper.GetAngleFromDirection(direction, _offset);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_currentAngle == inputAngle) return;
            
            _currentAngle = inputAngle;
            OnDirectionChanged?.Invoke(_currentAngle);
        }

        public void SetMagnitude(float magnitude)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_currentMagnitude == magnitude) return;
            
            _currentMagnitude = magnitude;
            OnMagnitudeChanged?.Invoke(_currentMagnitude);
        }

        public void ClearAngle()
        {
            _currentAngle = 0;
            _currentMagnitude = -1;
        }
    }
}