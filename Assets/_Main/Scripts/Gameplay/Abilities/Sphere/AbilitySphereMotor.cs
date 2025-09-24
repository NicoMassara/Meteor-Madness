using _Main.Scripts.FyingObject;
using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Observer;

namespace _Main.Scripts.Gameplay.Abilities.Sphere
{
    public class AbilitySphereMotor : FlyingObjectMotor
    {
        private AbilityType _abilityStored;
        
        public void SetAbilityValues(AbilitySphereValues data)
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