using System;
using _Main.Scripts.ShieldRotation.Contracts;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Gameplay.FlyingObject;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;
using UnityEngine.Events;

namespace MeteorMadness.Gameplay.Abilities.Sphere
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

        public event Action<ITargetable> OnTargetDeath;
        public event Action OnObjectDisabled;
        public event Action OnStartSound;
        public event Action OnStopSound;
        
        public void DisableTargetable()
        {
            CanBeTargeted = false;
        }
        
        public void EnableTargetable()
        {
            CanBeTargeted = true;
        }

        public override void SetValues(AbilitySphereValues data)
        {
            base.SetValues(data);
            EnableTargetable();
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
            DestroySphere();
            
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
            DestroySphere();
            
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

        private void DestroySphere()
        {
            OnObjectDisabled?.Invoke();
            OnStopSound?.Invoke();
            OnTargetDeath?.Invoke(this);
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