using System;
using _Main.Scripts.Contracts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Inputs
{
    public interface  IDeviceInput
    {
        public event Action<float> OnDirectionChanged;
        public event Action<float> OnMagnitudeChanged;
        public event Action<Vector2> OnPressingToMove;
        public event Action OnReleasedToMove;

        public void Execute();
    }

    public interface IDebugInput : IDebugComponent
    {
        public IDebugDeviceInput DebugDevice { get; }
    }

    public interface IDebugDeviceInput
    {
        public Vector2 PressPosition { get;}
        public Vector2 InputPosition { get; }
        public float CurrentAngle { get; }
        public float InputMagnitude { get; }
        public float LastAngle { get; }
        public bool IsPressingToMove { get; }
        public float DeadZone { get; }
        public float SafeZone { get; }
        public RectTransform LimitedZone { get; }
    }

    public interface IInputData
    {
        public float DeadZoneRadius { get; }
        public float SafeZoneRadius { get; }
        public RectTransform LimitedZone { get; }
    }
    
    public interface ITouchInputData : IInputData
    {
        
    }
    
    public interface IDesktopInputData : IInputData
    {

    }
}