using MeteorMadness.Core.FlyingObject;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.ProjectileObject
{
    public abstract class ProjectileObjectSetup<T, TA, TB> : FlyingObjectSetup<T, TA, TB>
    where T : ProjectileData
    where TA : IProjectileObjectView<T>
    where TB : IProjectileObjectController<T>
    {
        [Header("Collision Layers")]
        [SerializeField] private LayerMask shieldLayerMask;
        [SerializeField] private LayerMask earthLayerMask;

        protected LayerMask ShieldLayerMask => shieldLayerMask;
        protected LayerMask EarthLayerMask => earthLayerMask;

        protected override void InitializeViewHandlers()
        {
            base.InitializeViewHandlers();
            View.OnTriggerEnter += HandleOnTriggerEnter;
        }
        
        private void HandleOnTriggerEnter(Collider2D collision) => Controller?.HandleOnTriggerEnter(collision);
    }
}