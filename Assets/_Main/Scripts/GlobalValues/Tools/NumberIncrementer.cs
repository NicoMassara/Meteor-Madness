using System;
using NicolasMassara.CustomTimerManager;
using UnityEngine;

public class NumberIncrementer
{
    private readonly Action<uint> _onValueChanged;
    private readonly Action _onStart;
    private readonly Action _onFinish;

    private uint _startValue;
    private uint _currentValue;
    private uint _targetValue;

    private float _elapsedTime;
    private bool _isRunning;

    private const float DelayToIncrease = 0.25f;
    private const float IncreaseTime = 0.75f;

    public uint CurrentValue => _currentValue;
    public bool IsFinished => !_isRunning;

    public NumberIncrementer(
        Action<uint> onValueChanged,
        Action onStart,
        Action onFinish)
    {
        _onValueChanged = onValueChanged;
        _onStart = onStart;
        _onFinish = onFinish;
    }

    public void Run(float deltaTime)
    {
        if (!_isRunning)
            return;

        _elapsedTime += deltaTime;
        float t = Mathf.Clamp01(_elapsedTime / IncreaseTime);

        _currentValue = (uint)Mathf.Lerp(_startValue, _targetValue, t);
        _onValueChanged?.Invoke(_currentValue);

        if (t >= 1f)
        {
            _currentValue = _targetValue;
            _isRunning = false;
            _onFinish?.Invoke();
        }
    }

    public void ResetValues()
    {
        _currentValue = 0;
        _targetValue = 0;
        _startValue = 0;
        _elapsedTime = 0;
        _isRunning = false;
    }

    public void AddValue(uint value, bool instant = false)
    {
        if (instant)
        {
            _currentValue += value;
            _targetValue = _currentValue;
            _startValue = _currentValue;
            _onValueChanged?.Invoke(_currentValue);
            _onFinish?.Invoke();
            return;
        }
        
        void StartIncrement()
        {
            _startValue = _currentValue;
            _targetValue += value;
            _elapsedTime = 0f;
            _isRunning = true;
            _onStart?.Invoke();
        }
        
        TimerManager.Add(new TimerData(DelayToIncrease, StartIncrement));
    }
}
