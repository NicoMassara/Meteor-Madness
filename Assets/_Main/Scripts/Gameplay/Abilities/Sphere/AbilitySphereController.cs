using _Main.Scripts.FyingObject;
using _Main.Scripts.Gameplay.Abilies;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilities.Sphere
{
    public class AbilitySphereController : FlyingObjectController<AbilitySphereMotor>
    {
        private readonly LayerMask _shieldLayerMask;
        private readonly LayerMask _earthLayerMask;
        
        public AbilitySphereController(AbilitySphereMotor motor, LayerMask shieldLayerMask, LayerMask earthLayerMask) : base(motor)
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

        public void SetSphereValues(AbilitySphereValues data)
        {
            Motor.SetAbilityValues(data);
        }
    }
}