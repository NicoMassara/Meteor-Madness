using System;
using _Main.Scripts.FyingObject;
using _Main.Scripts.Gameplay.AutoTarget;
using _Main.Scripts.Gameplay.FlyingObject.Projectile;
using _Main.Scripts.Observer;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorView : FlyingObjectView<MeteorMotor, MeteorView, MeteorValuesData>, IMeteor, ITargetable, IProjectile
    {
        public UnityAction<MeteorCollisionData> OnEarthCollision { get; set; }
        public UnityAction<MeteorCollisionData> OnDeflection { get; set; }
        public Vector2 Position => (Vector2)transform.position;

        public bool EnableMovement { get; set; }
        public event Action OnDeath;

        public override void ManagedFixedUpdate()
        {
            if (EnableMovement)
            {
                base.ManagedFixedUpdate();
            }
        }


        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case MeteorObserverMessage.EarthCollision:
                    HandleEarthCollision(
                        (Vector2)args[0], 
                        (Quaternion)args[1],
                        (Vector2)args[2]);
                    break;
                case MeteorObserverMessage.ShieldDeflection:
                    HandleShieldDeflection(
                        (Vector2)args[0], 
                        (Quaternion)args[1],
                        (Vector2)args[2],
                        (float)args[3]);
                    break;
            }
            
            base.OnNotify(message, args);
        }

        private void HandleEarthCollision(Vector2 position, Quaternion rotation, Vector2 direction)
        {
            OnDeath?.Invoke();
            
            OnEarthCollision?.Invoke(new MeteorCollisionData
            {
                Meteor = this,
                Position = position,
                Rotation = rotation,
                Direction = direction,
            });
        }
        
        private void HandleShieldDeflection(Vector2 position,Quaternion rotation, Vector2 direction, float value)
        {
            OnDeath?.Invoke();
            
            OnDeflection?.Invoke(new MeteorCollisionData
            {
                Meteor = this,
                Position = position,
                Rotation = rotation,
                Direction = direction,
                Value = value
            });
            HandleCollision(false, position, direction,true);
        }
    }

    public class MeteorCollisionData
    {
        public MeteorView Meteor;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector2 Direction;
        public float Value;
    }
}