using System;
using UnityEngine;

namespace _Main.Scripts.ShieldRotation.Contracts
{
    public interface IShieldMovement
    {
        public float AngularSpeed { get; }
    }

    public interface IMovement : IShieldMovement
    {
        public Vector2 Position { get; }
        public bool IsSnapping { get; }
        public bool IsStopping { get; }
        public event Action<int> OnStartMoving;
        public event Action OnStartStop;
        public event Action OnStopped;
        public event Action<int> OnDirectionChange;
        public event Action OnSnapCorrected;
        public event Action OnCheckForCorrection;
        public float SpeedRatio { get; }
        public int GetCurrentSlot();
        public void SetDirection(float direction);
        public void Update(float deltaTime);
        public void SetCorrectionData(int targetSlot);
        public void ClearCorrectionData();
        public void ForceStop();
    }
    
    public interface IMovementCorrection
    {
        public int GetAngleSlotFromTarget(ITargetable target);
        public int GetSlotDistance(ITargetable target);
        public bool GetTargetIsInRange(ITargetable target);
        public float GetDistanceToTarget(ITargetable target);
        public int GetDirectionToTarget(ITargetable target);
    }

    public interface IProjectileDetector
    {
        public ITargetable GetNearestTarget(Vector2 movementPosition, float checkRadius = Mathf.Infinity);
    }

    public interface IRotationSpeeder : IShieldMovement
    {
        public event Action OnSpeedIncreased;
        public event Action OnSpeedDecreased;
        public event Action OnReachedMaxSpeed;
        public event Action OnReachedMinSpeed;
        public void Update(float deltaTime);
        public void SetTargetMinSpeed(float minSpeed);
        public void SpeedUp();
        public void SlowDown(float targetMinSpeed);
    }

    public interface ITargetSnapper : IShieldMovement
    {
        public float StartVelocity { get; }
        public event Action OnSnapping;
        public event Action OnSnapped;
        public void SetTargetSlot(int targetSlot);
        public void Update(float deltaTime);
    }

    public interface IAutomaticMovement : IShieldMovement
    {
        public event Action OnStartSnapping;
        public event Action OnStopSnapping;
        public event Action OnCheckForTarget;
        public event Action OnClearTarget;
        public void Update(float deltaTime);
        public void SetTargetAngle(int targetSlot);
    }

    public interface IMediator
    {
        // === Events === //
        public event Action<int> OnStartMoving;
        public event Action OnStartStop;
        public event Action<int> OnDirectionChange;
        public event Action OnStopped;  
        public event Action OnStartSnapping;  
        public event Action OnStopSnapping;  
        public event Action OnSpeedIncreased;  
        public event Action OnSpeedDecreased;  
        public event Action OnReachedMaxSpeed; 
        public event Action OnSnapped;  
        public event Action OnSnapping;  
        
        // === Actions  === //
        public void Update(float deltaTime);
        public void Enable();
        public void Disable();
        public void SetInputDirection(float direction);
        public void SpeedUp();
        public void SlowDown();
        public void EnableAutomatic();
        public void DisableAutomatic();
        public void TryToSnapToTarget();
    }

    public interface ITargetable
    {
        public Vector2 Position { get;}
        public bool CanBeTargeted { get;}
        public event Action<ITargetable> OnDeath;
        public void DisableTargetableComponent();
    }
}