using System;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation
{
    #region Rotator
    
    public interface IShieldRotator
    {
        public event Action OnSpeederReachedMaxSpeed;
        public event Action OnSpeederReachedMinSpeed;
        public event Action OnFinderFinish;
        public event Action OnRotationStopped;
        public event Action OnRotationStarted;
        public void TransitionToDisable();
        public void TransitionToManualInput();
        public void TransitionToAutomaticInput();
        public void TransitionToSpeeder();
        public void TransitionToFinder();
        public void CheckForTarget();
        public void SlowSpeederDown();
        public void SetInputAngle(float angle);
        public void SetInputMagnitude(float magnitude);
        public void RestartAngularRotation();
        
        public void Execute(float deltaTime);
    }
    

    public interface IShieldRotatorData
    {
        public IAutomaticSpeederData SpeederData { get; }
        public IAutomaticInputData AutomaticInputData { get; }
        public IInputRotationData InputRotationData { get; }
        public ITargetFinderData TargetFinderData { get; }
        public IDetectorData DetectorData { get; }
    }
    
    #endregion

    #region Angular Rotation

    public interface IAngularRotation
    {
        public event Action OnTargetReached;
        public event Action OnStartRotation;

        public void Execute(float deltaTime);
        public void SetEnable(bool isEnabled);
        public void SetRotationData(IRotationData rotationData);
        public void SetTargetAngle(float targetAngle);
        public void SetSpeedMultiplier(float speedMultiplier);
        public void StopRotation();
        public void RestartRotation();
    }

    public interface IRotationData
    {
        public float Acceleration { get; }
        public float MaxSpeed { get; }
    }

    [System.Serializable]
    public class RotationData : IRotationData
    {
        [SerializeField]
        [Min(0)]
        private float acceleration;
        [SerializeField]
        [Min(0)]
        private float maxSpeed;

        public float Acceleration => acceleration;
        public float MaxSpeed => maxSpeed;
    }

    #endregion

    #region Speeder

    public interface IAutomaticSpeederData
    {
        public float MaxSpeed { get; }
        public float DegreesStep { get; }
        public int AccelerateTurnsAmount { get; }
        public int DeAccelerateTurnsAmount { get; }
    }
    
    public interface IAutomaticSpeeder
    {
        public event Action OnReachedMaxSpeed;
        public event Action OnReachedMinSpeed;
        public event Action OnSpeedIncreased;
        public event Action OnSpeedDecreased;
        public void Execute(float deltaTime);
        public void SpeedUp();
        public void SpeedDown(float targetSpeed);
    }
    
    #endregion

    #region Automatic

    public interface IAutomaticInput
    {
        public event Action<float> OnTargetFound;
        public IRotationData GetRotationData();
        public void SetActive(bool isActive);
        public void Execute(float deltaTime);
    }
    
    public interface IAutomaticInputData
    {
        public float CheckRate { get; }
        public IRotationData RotationData { get; }
    }

    #endregion

    #region Input Rotation

    public interface IInputRotationData
    {
        public float MinInputAngle { get; }
        public IRotationData RotationData { get; }
        public AnimationCurve MagnitudeCurve { get; }
    }
    
    public interface IInputRotation
    {
        public event Action<float> OnInputChanged;  
        public event Action<float> OnMagnitudeChanged;  
        public void DisableInput();
        public void SetInputAngle(float inputAngle);
        public void SetInputMagnitude(float inputMagnitude);
        public IRotationData GetRotationData();
        public float GetInputAngle();
        public float GetInputMagnitude();
    }

    #endregion

    #region Target Finder

    public interface ITargetFinderData
    {
        public IRotationData RotationData { get; }
    }

    public interface ITargetFinder
    {
        public event Action<float> OnTargetFound;
        public event Action OnTargetNotFound;
        
        public void TryToFindTarget();
        public IRotationData GetRotationData();
    }

    #endregion

    #region Detector

    public interface ITargetDetector
    {
        public bool GetNearestTarget(out int targetSlot);
    }

    public interface IDetectorData
    {
        public LayerMask TargetLayerMask { get; }
        public float CheckRadius { get; }
    }

    #endregion
}