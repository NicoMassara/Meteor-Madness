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
        

        public virtual void SetValues(FlyingObjectValues data)
        {
            MovementSpeed = data.MovementSpeed;
            Rotation = data.Rotation;
            Position = data.Position;
            Direction = data.Direction;
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
    
    public class FlyingObjectValues
    {
        public float MovementSpeed;
        public Quaternion Rotation;
        public Vector2 Position;
        public Vector2 Direction;
    }
}