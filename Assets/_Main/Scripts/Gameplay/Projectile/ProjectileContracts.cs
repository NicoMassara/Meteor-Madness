using System;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Core.FlyingObject.Contracts;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile
{
    #region Projectile

    public abstract class ProjectileData : FlyingObjectValues
    {
        public float Value { get; set; }
        public int Slot { get; set; }
    }
    
    public abstract class ProjectileCollisionData
    {
        public Vector2 Position;
        public Quaternion Rotation;
        public Vector2 Direction;
    }
    
    public interface IDestructibleProjectile<T,TS> : IFlyingObject<T>
        where T : ProjectileData
        where TS : ProjectileCollisionData
    {
        public event Action<IProjectile, TS> OnEarthCollision;
        public event Action<IProjectile, TS> OnDeflection;
    }
    

    public interface IParticleProjectile<T>
        where T : ProjectileCollisionData
    {
        public event Action<IProjectile, T> OnEarthCollision;
        public event Action<IProjectile, T> OnDeflection;
    }


    public interface IProjectileObjectView<T> : IFlyingObjectView<T>
        where T : ProjectileData
    {
        public event Action<Collider2D> OnTriggerEnter;
    }
    
    public interface IProjectileObjectController<T> : IFlyingObjectController<T>
        where T : ProjectileData
    {
        public void HandleOnTriggerEnter(Collider2D other);
    }

    internal interface IDebugProjectile : IDebugFlyingObject
    {
        public bool CanBeTargeted { get; }
        public float TargetRatio { get; set; }
    }

    #endregion

    #region Meteor
    
    public sealed class MeteorData : ProjectileData { }

    public sealed class MeteorCollisionData : ProjectileCollisionData
    {
        public float Value { get; set; } 
    }

    public interface IMeteor : IDestructibleProjectile<MeteorData,MeteorCollisionData> { }

    internal interface IDebugMeteor : IDebugProjectile
    {
        
    }

    #endregion

    #region Ability Sphere
    
    public sealed class AbilitySphereData : ProjectileData
    {
        public AbilityType Ability { get; set; }
    }

    public sealed class AbilitySphereCollisionData : ProjectileCollisionData
    {
        public AbilityType Ability { get; set; }
    }

    public interface IAbilitySphere : IDestructibleProjectile<AbilitySphereData,AbilitySphereCollisionData>{ }

    public interface IAbilitySphereColor
    {
        public event Action<AbilityType> OnAbilitySet;
    }
    
    internal interface IDebugAbilitySphere : IDebugProjectile
    {
        public AbilityType DebugAbility { get; set; }
    }
    

    #endregion
    
}