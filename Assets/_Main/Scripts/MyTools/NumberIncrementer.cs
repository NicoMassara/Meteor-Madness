using System;
using UnityEngine;

namespace _Main.Scripts
{
    public class NumberIncrementer
    {
        private readonly float _targetTime;
        private readonly Action<uint> _increaseAction;
        private readonly Action _actionOnFinish;
        private uint _targetValue;
        private uint _currentValue;
        private uint _startValue;

        public uint CurrentValue => _currentValue;
        public bool IsFinished => _currentValue >= _targetValue;

        public NumberIncrementer(float targetTime, Action<uint> increaseAction, Action actionOnFinish)
        {
            _targetTime = targetTime;
            _increaseAction = increaseAction;
            _actionOnFinish = actionOnFinish;
        }

        public void Run(float deltaTime)
        {
            _currentValue = (uint)Mathf.MoveTowards(_currentValue, _targetValue,
                (_targetValue - _startValue) / _targetTime * deltaTime
            );
            
            _increaseAction?.Invoke(_currentValue);

            if (IsFinished)
            {
                _startValue = _currentValue;
                _actionOnFinish?.Invoke();
            }
        }

        public void ResetValues()
        {
            _currentValue = 0;
        }

        public void AddValue(uint value)
        {
            _targetValue += value;
        }
    }
}