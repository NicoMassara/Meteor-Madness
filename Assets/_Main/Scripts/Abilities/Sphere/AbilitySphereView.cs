using System;
using _Main.Scripts.FlyingObject;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Observer;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Abilities.Sphere
{
    public class AbilitySphereView : 
        FlyingObjectView<AbilitySphereMotor, AbilitySphereView, AbilitySphereValues>, 
        IProjectile, ITargetable, IAbilitySphereSounds
    {
        public UnityAction<AbilitySphereCollisionData> OnEarthCollision { get; set; }
        public UnityAction<AbilitySphereCollisionData> OnDeflection { get; set; }
        public Vector2 Position => (Vector2)transform.position;
        public bool CanBeTargeted { get; private set; }
        public bool EnableMovement { get; set; }

        public event Action OnDeath;
        public event Action OnStartSound;
        public event Action OnStopSound;
        
        public void DisableTargetable()
        {
            CanBeTargeted = false;
        }

        public override void SetValues(AbilitySphereValues data)
        {
            base.SetValues(data);
            CanBeTargeted = true;
            OnStartSound?.Invoke();
        }

        public void SetEnableMovement(bool enable)
        {
            Movement.CanMove = enable;
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
            OnStopSound?.Invoke();
            
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
    
    public struct AbilitySphereCollisionData
    {
        public AbilitySphereView Sphere;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector2 Direction;
        public AbilityType Ability;
    }
}