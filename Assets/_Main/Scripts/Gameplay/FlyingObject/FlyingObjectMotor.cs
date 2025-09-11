using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.FyingObject
{
    public abstract class FlyingObjectMotor : ObservableComponent
    {
        protected float MovementSpeed { get; set; }
        public Vector2 Position { get; protected set; }
        public Quaternion Rotation { get; protected set; }
        protected bool CanMove { get; set; }
        

        public virtual void SetValues(float movementSpeed, Quaternion rotation, Vector2 position)
        {
            MovementSpeed = movementSpeed;
            Rotation = rotation;
            Position = position;
            CanMove = true;
            
            NotifyAll(FlyingObjectObserverMessage.SetValues, MovementSpeed, Rotation, Position, CanMove);
        }

        public virtual void HandleCollision(bool doesShowParticles)
        {
            CanMove = false;
            
            NotifyAll(FlyingObjectObserverMessage.HandleCollision, CanMove, Position, doesShowParticles);
        }
        
        public void UpdatePosition(Vector2 transformPosition)
        {
            Position = transformPosition;
        }
    }
}