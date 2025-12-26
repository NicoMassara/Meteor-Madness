using System;
using NicolasMassara.CustomTimerManager;
using UnityEngine;

namespace MeteorMadness.GlobalValues.Tools
{
    public class NumberIncrementer
    {
        private readonly Action<uint> _increaseAction;
        private readonly Action _actionOnStart;
        private readonly Action _actionOnFinish;
        private uint _targetValue;
        private uint _currentValue;
        private uint _startValue;
        private float _elapsedTime;
        private const float DelayToIncrease = 0.25f;
        private const float IncreaseTime = 0.75f;

        public uint CurrentValue => _currentValue;
        public bool IsFinished => _currentValue >= _targetValue;

        public NumberIncrementer(Action<uint> increaseAction, Action actionOnStart ,Action actionOnFinish)
        {
            _increaseAction = increaseAction;
            _actionOnStart = actionOnStart;
            _actionOnFinish = actionOnFinish;
        }

        public void Run(float deltaTime)
        {
            _elapsedTime += deltaTime;
            
            float ratio = Mathf.Clamp01(_elapsedTime /IncreaseTime);

            _currentValue = (uint)Mathf.Lerp(_startValue, _targetValue, ratio);

            if (ratio >= 1f)
            {
                _startValue = _currentValue;
                _increaseAction?.Invoke(_currentValue);
                _actionOnFinish?.Invoke();
            }
            else
            {
                _increaseAction?.Invoke(_currentValue);
            }
        }
        

        public void ResetValues()
        {
            _currentValue = 0;
            _targetValue = 0;
            _elapsedTime = 0;
        }

        public void AddValue(uint value, bool doInstant = false)
        {
            if (doInstant)
            {
                _targetValue += value;
                _elapsedTime = 0;
                _actionOnStart?.Invoke();
            }
            else
            {
                TimerManager.Add(new TimerData(DelayToIncrease, () =>
                {
                    _targetValue += value;
                    _elapsedTime = 0;
                    _actionOnStart?.Invoke();
                }));
            }
        }
    }
}