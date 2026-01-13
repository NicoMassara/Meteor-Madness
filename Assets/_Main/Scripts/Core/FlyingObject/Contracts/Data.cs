using System;
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
    
    public class FlyingObjectMovement
    {
        private readonly Rigidbody2D _rigidbody;
        private event Action<Vector2> _onPositionChanged;
        private float _movementSpeed;

        public FlyingObjectMovement(Rigidbody2D rigidbody, Action<Vector2> onPositionChanged)
        {
            _rigidbody = rigidbody;
            _onPositionChanged += onPositionChanged;
                
            Initialize();
        }

        private void Initialize()
        {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.simulated = true;
            _rigidbody.mass = 0.0001f;
            _rigidbody.drag = 0f;
            _rigidbody.angularDrag = 0.05f;
            _rigidbody.gravityScale = 0f;

        }

        public void SetRigidbodyData(float speed, Quaternion rotation, Vector2 position)
        {
            _movementSpeed = speed;
            _rigidbody.transform.rotation = rotation;
            _rigidbody.transform.position = new Vector3(position.x,position.y);
                
        }

        public void Update(float fixedDeltaTime)
        {
            _rigidbody.transform.Translate(Vector2.right * (_movementSpeed * fixedDeltaTime));
            _onPositionChanged?.Invoke(_rigidbody.position);
        }
    }

}