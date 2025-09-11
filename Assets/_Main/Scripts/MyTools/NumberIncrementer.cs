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
        
        public void Run()
        {
            _currentValue = Mathf.MoveTowards(
                _currentValue,
                _targetValue,
                (_targetValue - _startValue) / _targetTime * Time.deltaTime
            );

            if (IsFinished)
            {
                _actionOnFinish?.Invoke();
            }
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
        public float Target;
        public float Current;
        public float TargetTime;
        public Action ActionOnFinish;
    }
}