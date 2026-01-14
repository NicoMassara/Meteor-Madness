using MeteorMadness.Core.FlyingObject;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.ProjectileObject
{
    public abstract class ProjectileObjectController<T,TS,TB> : FlyingObjectController<T,TS>,
        IProjectileObjectController<T>
        where T : ProjectileData
        where TS : ProjectileObjectMotor<T,TB>
        where TB : ProjectileCollisionData
    {
        
        private readonly LayerMask _shieldLayerMask;
        private readonly LayerMask _earthLayerMask;
        
        public ProjectileObjectController(TS motor, LayerMask shieldLayerMask, LayerMask earthLayerMask)
            : base(motor)
        {
            _shieldLayerMask = shieldLayerMask;
            _earthLayerMask = earthLayerMask;
        }

        public virtual void HandleOnTriggerEnter(Collider2D other)
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
    
    public abstract class ProjectileObjectMotor<T,TB> : FlyingObjectMotor<T>
        where T : ProjectileData
        where TB : ProjectileCollisionData

    {
        protected abstract TB GetCollisionData();
        
        public void HandleShieldDeflection()
        {
            NotifyAll(ProjectileObserverMessage.ShieldDeflection, GetCollisionData());
        }

        public void HandleEarthCollision()
        {
            NotifyAll(ProjectileObserverMessage.EarthCollision, GetCollisionData());
        }
    }
}