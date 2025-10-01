using _Main.Scripts.FyingObject;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorMotor : FlyingObjectMotor<MeteorValuesData>
    {
        private float _value = 1;
        

        public override void SetValues(MeteorValuesData data)
        {
            base.SetValues(data);
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

    public class MeteorValuesData : FlyingObjectValues
    {
        public float Value;
    }
}