using MeteorMadness.Gameplay._Main.Scripts.Gameplay.ProjectileObject;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.Meteor
{
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

    internal sealed class MeteorController : ProjectileObjectController<MeteorData, MeteorMotor>,
        MeteorController.IMeteorController
    {
        internal interface IMeteorController : IProjectileObjectController<MeteorData> { }

        public MeteorController(MeteorMotor motor, LayerMask shieldLayerMask, LayerMask earthLayerMask) 
            : base(motor, shieldLayerMask, earthLayerMask) { }
    }

    internal sealed class MeteorMotor : ProjectileObjectMotor<MeteorData> { }
}