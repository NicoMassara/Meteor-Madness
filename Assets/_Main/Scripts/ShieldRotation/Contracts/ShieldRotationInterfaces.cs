using System;
using UnityEngine;

namespace _Main.Scripts.ShieldRotation.Contracts
{
    public interface IShieldMovement
    {
        public float AngularSpeed { get; }
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
        public void EnableCheck();
    }

    public interface IMediator
    {
        // === Events === //
        public event Action OnDirectionChanged;
        public event Action OnStopped;  
        public event Action OnMoved;  
        public event Action OnStartSnapping;  
        public event Action OnStopSnapping;  
        public event Action OnSpeedIncreased;  
        public event Action OnSpeedDecreased;  
        public event Action OnReachedMaxSpeed; 
        public event Action OnReachedMinSpeed; 
        public event Action OnSnapped;  
        public event Action OnSnapping;  
        
        // === Actions  === //
        public void Update(float deltaTime);
        public void Enable();
        public void Disable();
        public void SetInputDirection(float inputAngle);
        public void SpeedUp();
        public void SlowDown();
        public void EnableAutomatic();
        public void DisableAutomatic();
        public void TryToSnapToTarget();
        public void SetInputMagnitude(float magnitude);
    }

    public interface ITargetable
    {
        public Vector2 Position { get;}
        public bool CanBeTargeted { get;}
        public event Action<ITargetable> OnTargetDeath;
        public void DisableTargetable();
        public void EnableTargetable();
    }
}