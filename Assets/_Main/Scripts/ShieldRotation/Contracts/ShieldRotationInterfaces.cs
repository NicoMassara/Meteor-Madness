using System;
using UnityEngine;

namespace _Main.Scripts.ShieldRotation.Contracts
{
    public interface IMovement
    {
        public Vector2 Position { get; }
        public bool IsSnapping { get; }
        public bool IsStopping { get; }
        public event Action<int> OnStartMoving;
        public event Action OnStartStop;
        public event Action OnStopped;
        public event Action<int> OnDirectionChange;
        public event Action OnSnapCorrected;
        public float SpeedRatio { get; }
        public int GetCurrentSlot();
        public void SetDirection(float direction);
        public void Update(float deltaTime);
        public void SetCorrectionData(int targetSlot);
        public void ClearCorrectionData();
    }
    
    public interface IMovementCorrection
    {
        public int GetAngleSlotFromTarget(ITargetable target);
        public int GetSlotDistance(ITargetable target);
        public bool GetTargetIsInRange(ITargetable target);
        public float GetDistanceToTarget(ITargetable target);
        public int GetDirectionToTarget(ITargetable target);
        public bool GetIsInFrontOfTarget(ITargetable target);
    }

    public interface IProjectileDetector
    {
        public ITargetable GetNearestTarget(Vector2 movementPosition, float checkRadius = Mathf.Infinity);
    }

    public interface IRotationSpeeder
    {
        public event Action<float> OnUpdate;
        public event Action OnSpeedIncreased;
        public event Action OnSpeedDecreased;
        public event Action OnReachedMaxSpeed;
        public event Action OnReachedMinSpeed;
        public void Update(float deltaTime);
        public void SetTargetMinSpeed(float minSpeed);
        public void SpeedUp();
        public void SlowDown();
    }

    public interface ITargetSnapper
    {
        
    }

    public interface ITargetable
    {
        public Vector2 Position { get;}
        public bool CanBeTargeted { get;}
        public event Action OnDeath;
        public void DisableTargetable();
    }
}