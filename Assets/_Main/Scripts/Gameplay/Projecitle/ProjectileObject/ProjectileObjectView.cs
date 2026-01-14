using System;
using _Main.Scripts.ShieldRotation.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Core.FlyingObject;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile;
using MeteorMadness.GlobalValues.Tools.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;


namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.ProjectileObject
{
    public abstract class ProjectileObjectView<T,TS> : FlyingObjectView<T>,
        IProjectileObjectView<T>,
        IParticleProjectile<TS>,
        IDestructibleProjectile<T,TS>, 
        IProjectile,
        ITargetable
        where T : ProjectileData
        where TS : ProjectileCollisionData
    {

        public override UpdateGroup MovementUpdateGroup { get; } = UpdateGroup.Gameplay;
        public override TickGroup MovementTickGroup { get; } = TickGroup.EveryFrame;
        public event Action<Collider2D> OnTriggerEnter;
        
        public event Action<IProjectile,TS> OnEarthCollision;
        public event Action<IProjectile,TS> OnDeflection;

        private void Start()
        {
            OnRecycle += OnRecycleHandler;
        }

        #region Observer Handlers
        
        public override void OnNotify(ulong message, params object[] args)
        {
            base.OnNotify(message, args);

            switch (message)
            {
                case ProjectileObserverMessage.EarthCollision:
                    HandleCollision((TS)args[0]);
                    break;
                case ProjectileObserverMessage.ShieldDeflection:
                    HandleDeflection((TS)args[0]);
                    break;
            }
        }

        protected virtual void HandleCollision(TS collisionData)
        {
            Recycle();
            OnEarthCollision?.Invoke(this,collisionData);
        }

        protected virtual void HandleDeflection(TS collisionData)
        {
            Recycle();
            OnDeflection?.Invoke(this,collisionData);
        }

        #endregion

        #region ITargetable

        public bool CanBeTargeted { get; private set; }
        public event Action<ITargetable> OnTargetDeath;

        public void DisableTargetable()
        {
            CanBeTargeted = false;
        }

        public void EnableTargetable()
        {
            CanBeTargeted = true;
        }

        #endregion
        
        #region Handlers

        private void OnRecycleHandler(FlyingObjectView<T> input)
        {
            OnTargetDeath?.Invoke(this);
        }
        
        #endregion
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            OnTriggerEnter?.Invoke(other);
        }
    }
}