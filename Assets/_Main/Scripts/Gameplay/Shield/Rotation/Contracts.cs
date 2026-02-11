using System;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation
{
    public interface IShieldRotator
    {
        public IAngularRotation GetInputRotation();
        public void Execute(float deltaTime);
    }

    public interface IAngularRotationData
    {
        public float Acceleration { get; }
        public float MaxSpeed { get; }
        public float MinInputAngle { get; }
        public AnimationCurve MagnitudeCurve { get; }
    }

    public interface IAngularRotation
    {
        public event Action OnStopped;
        public event Action OnMoved;
        public void Execute(float deltaTime);
        public void SetInputAngle(float inputAngle);
        public void SetInputMagnitude(float inputMagnitude);
        public void StopRotation();
        public void SetActiveInput(bool isActive);
        public void SetEnable(bool isEnabled);
        public void RestartPosition();
    }
}