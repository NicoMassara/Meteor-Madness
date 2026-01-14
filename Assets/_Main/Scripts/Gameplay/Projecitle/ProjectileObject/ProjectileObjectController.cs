using MeteorMadness.Core.FlyingObject;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.ProjectileObject
{
    public abstract class ProjectileObjectController<T,TS> : FlyingObjectController<T,TS>,
        IProjectileObjectController<T>
        where T : ProjectileData
        where TS : ProjectileObjectMotor<T>
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
    
    public abstract class ProjectileObjectMotor<T> : FlyingObjectMotor<T>
        where T : ProjectileData
    {
        public void HandleShieldDeflection()
        {
            NotifyAll(ProjectileObserverMessage.ShieldDeflection);
        }

        public void HandleEarthCollision()
        {
            NotifyAll(ProjectileObserverMessage.EarthCollision);
        }
    }
}