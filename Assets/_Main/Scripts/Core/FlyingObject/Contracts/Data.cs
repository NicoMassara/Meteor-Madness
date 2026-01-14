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
        private float _movementSpeed;

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
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.simulated = true;
            _rigidbody.mass = 0.0001f;
            _rigidbody.drag = 0f;
            _rigidbody.angularDrag = 0.05f;
            _rigidbody.gravityScale = 0f;
            InitializeUpdatable();

        }

        public void SetRigidbodyData(float speed, Quaternion rotation, Vector2 position)
        {
            _movementSpeed = speed;
            _rigidbody.transform.rotation = rotation;
            _rigidbody.transform.position = new Vector3(position.x,position.y);
                
        }

        public void ExecuteFixedUpdate(float fixedDeltaTime)
        {
            if(_rigidbody == null) return;
            
            _rigidbody.transform.Translate(Vector2.right * (_movementSpeed * fixedDeltaTime));
            _onPositionChanged?.Invoke(_rigidbody.position);
        }
    }

}