using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.FyingObject
{
    public abstract class FlyingObjectMotor : ObservableComponent
    {
        protected float MovementSpeed { get; set; }
        public Vector2 Position { get; protected set; }
        public Quaternion Rotation { get; protected set; }
        public Vector2 Direction { get; protected set; }
        protected bool CanMove { get; set; }
        

        public virtual void SetValues(float movementSpeed, Quaternion rotation, Vector2 position, Vector2 direction)
        {
            MovementSpeed = movementSpeed;
            Rotation = rotation;
            Position = position;
            Direction = direction;
            CanMove = true;
            
            NotifyAll(FlyingObjectObserverMessage.SetValues, MovementSpeed, Rotation, Position, CanMove);
        }

        public virtual void HandleCollision(bool doesShowParticles)
        {
            CanMove = false;
            
            NotifyAll(FlyingObjectObserverMessage.HandleCollision, CanMove, Position, Direction, doesShowParticles);
        }
        
        public void UpdatePosition(Vector2 transformPosition)
        {
            Position = transformPosition;
        }
    }
}