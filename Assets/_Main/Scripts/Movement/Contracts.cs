using System;
using _Main.Scripts.Contracts;
using _Main.Scripts.Contracts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Movement
{
    public interface IMovementData
    {
        public float Acceleration { get; }
        public float DeAcceleration { get; }
        public float MaxSpeed { get; }
        public float MinInputAngle { get; }
        public float MaxTravelDistanceRatioToDeAccelerate { get; }
        public float MinDistanceRatioToSnap { get; }
        public AnimationCurve MagnitudeCurve { get; }
    }
    public interface IMovement
    {
        public event Action OnStopped;
        public event Action OnMoved;
        public event Action OnDirectionChanged;
        
        public void Initialize();
        public void SetInputAngle(float inputAngle);
        public void SetInputMagnitude(float inputMagnitude);
        public void Update(float deltaTime);
        public void RemoveInput();
        public void ForceStop();
    }

    public interface IDebugMovementComponent
    {
        public float CurrentSpeed { get; }
        public float SpeedRatio { get; }
        public float CurrentAngle { get; }
        public float CurrentDirection { get; }
        public float TargetAngle { get; }
        public float DistanceToTarget { get; }
        public float DistanceToTargetRatio { get; }
        public float CurveMagnitude { get; }
        public bool HasTargetAngle { get; }
        public bool HasInput { get; }
        public string CurrentState { get; }
        public string LastState { get; }
    }

    public interface IDebugMovement : IDebugComponent
    {
        public Vector3 DebugPosition { get; }
        public IDebugMovementComponent DebugComponent { get; }
    }
}