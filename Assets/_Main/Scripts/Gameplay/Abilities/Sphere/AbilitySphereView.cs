using System;
using _Main.Scripts.FyingObject;
using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Observer;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Abilities.Sphere
{
    public class AbilitySphereView : FlyingObjectView<AbilitySphereMotor, AbilitySphereView, AbilitySphereController>
    {
        public UnityAction<AbilitySphereCollisionData> OnEarthCollision { get; set; }
        public UnityAction<AbilitySphereCollisionData> OnDeflection { get; set; }
        public event Action OnDeath;
        
        
        public void SetSphereValues(AbilitySphereValues data)
        {
            Controller.SetSphereValues(data);
        }
        
        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case AbilitySphereObserverMessage.EarthCollision:
                    HandleEarthCollision(
                        (Vector2)args[0], 
                        (Quaternion)args[1],
                        (Vector2)args[2]);
                    break;
                case AbilitySphereObserverMessage.ShieldDeflection:
                    HandleShieldDeflection(
                        (Vector2)args[0], 
                        (Quaternion)args[1],
                        (Vector2)args[2],
                        (AbilityType)args[3]);
                    break;
            }
            
            base.OnNotify(message, args);
        }
        
        private void HandleEarthCollision(Vector2 position, Quaternion rotation, Vector2 direction)
        {
            OnDeath?.Invoke();
            
            OnEarthCollision?.Invoke(new AbilitySphereCollisionData
            {
                Sphere = this,
                Position = position,
                Rotation = rotation,
                Direction = direction,
            });
        }
        
        private void HandleShieldDeflection(Vector2 position,Quaternion rotation, Vector2 direction, AbilityType ability)
        {
            OnDeath?.Invoke();
            
            OnDeflection?.Invoke(new AbilitySphereCollisionData
            {
                Sphere = this,
                Position = position,
                Rotation = rotation,
                Direction = direction,
                Ability = ability
            });
            HandleCollision(false, position, direction,true);
        }
    }
    
    public class AbilitySphereCollisionData
    {
        public AbilitySphereView Sphere;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector2 Direction;
        public AbilityType Ability;
    }
}