using _Main.Scripts.FlyingObject;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Meteor
{
    public class MeteorController : FlyingObjectController<MeteorMotor, MeteorValuesData>
    {
        private readonly LayerMask _shieldLayerMask;
        private readonly LayerMask _earthLayerMask;


        public MeteorController(MeteorMotor motor, LayerMask shieldLayerMask, LayerMask earthLayerMask) 
            : base(motor)
        {
            _shieldLayerMask = shieldLayerMask;
            _earthLayerMask = earthLayerMask;
        }

        public override void HandleTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & _shieldLayerMask) != 0)
            {
                Motor.HandleShieldDeflection();
            }
            else if (((1 << other.gameObject.layer) & _earthLayerMask) != 0)
            {
                Motor.HandleEarthCollision(); 
            }
        }
    }
    
    public class MeteorMotor : FlyingObjectMotor<MeteorValuesData>
    {
        private byte _value = 100;
        

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
        public byte Value;
    }
}