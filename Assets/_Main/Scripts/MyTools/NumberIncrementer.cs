using System;
using UnityEngine;

namespace _Main.Scripts
{
    public class NumberIncrementer
    {
        private float _targetValue;
        private float _currentValue;
        private float _startValue;
        private float _targetTime;
        private Action _actionOnFinish;

        public float CurrentValue => _currentValue;
        public bool IsFinished => _currentValue >= _targetValue;
        
        public void Run(float deltaTime)
        {
            _currentValue = Mathf.MoveTowards(
                _currentValue,
                _targetValue,
                (_targetValue - _startValue) / _targetTime * deltaTime
            );

            if (IsFinished)
            {
                _actionOnFinish?.Invoke();
            }
        }

        public void ResetValues()
        {
            _currentValue = 0;
        }

        public void SetData(NumberIncrementerData data)
        {
            _currentValue = data.Current;
            _startValue = _currentValue;
            _targetValue = data.Target;
            _targetTime = data.TargetTime;
            _actionOnFinish = data.ActionOnFinish;
        }
    }

    public class NumberIncrementerData
    {
        public float Target = 0;
        public float Current = 0;
        public float TargetTime = 0;
        public Action ActionOnFinish;
    }
}