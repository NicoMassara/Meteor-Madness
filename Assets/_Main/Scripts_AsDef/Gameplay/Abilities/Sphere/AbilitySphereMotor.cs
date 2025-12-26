using MeteorMadness.Gameplay.FlyingObject;
using MeteorMadness.GlobalValues;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities.Sphere
{
    public class AbilitySphereMotor : FlyingObjectMotor<AbilitySphereValues>
    {
        private AbilityType _abilityStored;

        public override void SetValues(AbilitySphereValues data)
        {
            base.SetValues(data);
            _abilityStored = data.AbilityType;
        }

        public void HandleShieldDeflection()
        {
            NotifyAll(AbilitySphereObserverMessage.ShieldDeflection, Position, Rotation, Direction, _abilityStored);
        }

        public void HandleEarthCollision()
        {
            NotifyAll(AbilitySphereObserverMessage.EarthCollision, Position, Rotation, Direction);
        }
    }
    
    public class AbilitySphereValues : FlyingObjectValues
    {
        public AbilityType AbilityType;
    }
}