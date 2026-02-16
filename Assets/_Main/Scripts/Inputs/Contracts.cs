using System;
using _Main.Scripts.Contracts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Inputs
{
    public interface  IDeviceInput
    {
        public event Action<float> OnDirectionChanged;
        public event Action<Vector2> OnPressingToMove;
        public event Action OnReleasedToMove;

        public void Execute();
    }

    public interface IInputData
    {
        public float DeadZoneRadius { get; }
        public float SafeZoneRadius { get; }
        public RectTransform LimitedZone { get; }
    }
}