using _Main.Scripts.FyingObject;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorMotor : FlyingObjectMotor
    {
        private float _value = 1;
     
        public void SetMeteorValues(MeteorValuesData data)
        {
            base.SetValues(data.MovementSpeed,data.Rotation, data.Position, data.Direction);
            _value = data.Value;
        }

        public void HandleShieldDeflection()
        {
            NotifyAll(MeteorObserverMessage.ShieldDeflection, Position, Rotation, Direction, _value);
        }

        public void HandleEarthCollision()
        {
            NotifyAll(MeteorObserverMessage.EarthCollision, Position, Rotation, Direction);
        }
        
    }

    public class MeteorValuesData
    {
        public float MovementSpeed;
        public Quaternion Rotation;
        public Vector2 Position;
        public Vector2 Direction;
        public float Value;
    }
}