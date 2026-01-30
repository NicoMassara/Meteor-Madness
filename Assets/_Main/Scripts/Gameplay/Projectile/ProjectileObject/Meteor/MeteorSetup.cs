using MeteorMadness.Gameplay._Main.Scripts.Gameplay.ProjectileObject;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.Meteor
{
    [RequireComponent(typeof(MeteorView))]
    [RequireComponent(typeof(MeteorParticles))]
    internal sealed class MeteorSetup : ProjectileObjectSetup<MeteorData, MeteorView.IMeteorView, MeteorController.IMeteorController>
    {
        protected override void Awake()
        {
            var view = GetComponent<MeteorView>();
            var motor = new MeteorMotor();

            motor.Subscribe(view);
            
            Controller = new MeteorController(motor, ShieldLayerMask, EarthLayerMask);
            View = view;
            
            InitializeViewHandlers();
        }
    }

    internal sealed class MeteorController : ProjectileObjectController<MeteorData, MeteorMotor,MeteorCollisionData>,
        MeteorController.IMeteorController
    {
        internal interface IMeteorController : IProjectileObjectController<MeteorData> { }

        public MeteorController(MeteorMotor motor, LayerMask shieldLayerMask, LayerMask earthLayerMask) 
            : base(motor, shieldLayerMask, earthLayerMask) { }
    }

    internal sealed class MeteorMotor : ProjectileObjectMotor<MeteorData, MeteorCollisionData>
    {
        private MeteorCollisionData _collisionData;
        protected override MeteorCollisionData GetCollisionData()
        {
            _collisionData ??= new MeteorCollisionData();
            
            _collisionData.Position = Data.Position;
            _collisionData.Rotation = Data.Rotation;
            _collisionData.Direction = Data.Direction;
            
            return _collisionData;
        }

        public override void SetValues(MeteorData data)
        {
            base.SetValues(data);
            
            _collisionData ??= new MeteorCollisionData();
            
            _collisionData.Value = Data.Value;
        }
    }
}