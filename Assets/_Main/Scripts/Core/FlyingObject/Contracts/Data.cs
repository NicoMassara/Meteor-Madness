using System;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.Core.FlyingObject.Contracts
{
    public abstract class FlyingObjectValues
    {
        public float MovementSpeed;
        public Vector2 Position;
        public Vector2 Direction;
        public Quaternion Rotation;
    }
    
    public class FlyingObjectMovement : ManagedComponent, IFixedUpdatable
    {
        private readonly Rigidbody2D _rigidbody;
        private event Action<Vector2> _onPositionChanged;
        private Vector2 _movementDirection;
        private float _movementSpeed;
        private bool _isDeactivated;
        private bool _hasPendingActivation;
        private bool _justActivated;

        public bool CanMove { get; set; }

        public UpdateGroup SelfUpdateGroup { get; private set;}
        public TickGroup SelfTickGroup { get; private set; }

        public FlyingObjectMovement(Rigidbody2D rigidbody, 
            UpdateGroup selfUpdateGroup, TickGroup selfTickGroup, 
            Action<Vector2> onPositionChanged) : base()
        {
            _rigidbody = rigidbody;
            SelfUpdateGroup = selfUpdateGroup;
            SelfTickGroup = selfTickGroup;
            _onPositionChanged += onPositionChanged;
            Initialize();
        }

        public void Initialize()
        {
            _rigidbody.mass = 0.001f;
            _rigidbody.drag = 0f;
            _rigidbody.angularDrag = 0.05f;
            _rigidbody.gravityScale = 0f;
            _isDeactivated = true;
        }

        public void DeactivateBody()
        {
            Dispose();
            _isDeactivated = true;
            
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
            
            _rigidbody.simulated = false;
        }

        private void ActivateBody()
        {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.simulated = true;
            
            _isDeactivated = false;
            _justActivated = true;
        }

        public void SetRigidbodyData(float speed, 
            Quaternion rotation, 
            Vector2 position, Vector2 direction)
        {
            Register();
            ActivateBody();
            _movementDirection = direction;
            _movementSpeed = speed;
            _rigidbody.SetRotation(rotation.eulerAngles.z);
            _rigidbody.position = position;
            
            _onPositionChanged?.Invoke(_rigidbody.position);
        }

        public void ExecuteFixedUpdate(float fixedDeltaTime)
        {
            if(_rigidbody == null) return;
            
            if (_justActivated)
            {
                _justActivated = false;
                return;
            }

            if (_isDeactivated)
            {
                return;
            }

            var finalDirection = _movementDirection * (_movementSpeed * fixedDeltaTime);
            
            _rigidbody.MovePosition(_rigidbody.position + finalDirection);

            _onPositionChanged?.Invoke(_rigidbody.position);
        }
    }

}