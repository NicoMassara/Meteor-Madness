using System;
using _Main.Scripts.Gameplay.GameMode;
using _Main.Scripts.Interfaces.UI;
using NicolasMassara.CustomTimerManager;
using UnityEngine;

namespace _Main.Scripts
{
    public class NumberIncrementer
    {
        private readonly Action<uint> _increaseAction;
        private readonly Action _actionOnFinish;
        private uint _targetValue;
        private uint _currentValue;
        private uint _startValue;
        private float _elapsedTime;
        private const float DelayToIncrease = 0.25f;
        private const float IncreaseTime = 0.75f;

        public uint CurrentValue => _currentValue;
        public bool IsFinished => _currentValue >= _targetValue;

        public NumberIncrementer(Action<uint> increaseAction, Action actionOnFinish)
        {

            _increaseAction = increaseAction;
            _actionOnFinish = actionOnFinish;
        }

        public void Run(float deltaTime)
        {
            _elapsedTime += deltaTime;
            
            float ratio = Mathf.Clamp01(_elapsedTime /IncreaseTime);

            _currentValue = (uint)Mathf.Lerp(_startValue, _targetValue, ratio);
            _increaseAction?.Invoke(_currentValue);
            
            _increaseAction?.Invoke(_currentValue);

            if (ratio >= 1f)
            {
                _startValue = _currentValue;
                _actionOnFinish?.Invoke();
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
            }
            else
            {
                TimerManager.Add(new TimerData(DelayToIncrease, () =>
                {
                    _targetValue += value;
                    _elapsedTime = 0;
                }));
            }
        }
        
        
    }
}