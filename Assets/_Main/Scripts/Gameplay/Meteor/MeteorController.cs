using _Main.Scripts.FyingObject;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorController : FlyingObjectController<MeteorMotor>
    {
        private readonly LayerMask _shieldLayerMask;
        private readonly LayerMask _earthLayerMask;

        public MeteorController(MeteorMotor motor, LayerMask shieldLayerMask, LayerMask earthLayerMask) : 
            base(motor)
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
}