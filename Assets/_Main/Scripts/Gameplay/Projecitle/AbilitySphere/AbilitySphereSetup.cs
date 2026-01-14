using MeteorMadness.Gameplay._Main.Scripts.Gameplay.ProjectileObject;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.AbilitySphere
{
    internal class AbilitySphereSetup : ProjectileObjectSetup<AbilitySphereData, AbilitySphereView.IAbilitySphereView, AbilitySphereController.IAbilitySphereController>
    {
        protected override void Awake()
        {
            var view = GetComponent<AbilitySphereView>();
            var motor = new AbilitySphereMotor();

            motor.Subscribe(view);
            Controller = new AbilitySphereController(motor, ShieldLayerMask, EarthLayerMask);
            View = view;
            
            InitializeViewHandlers();
        }
    }
    
    #region Controller / Motor

    public sealed class AbilitySphereController : ProjectileObjectController<AbilitySphereData, AbilitySphereMotor>,
        AbilitySphereController.IAbilitySphereController
    {
        internal interface IAbilitySphereController : IProjectileObjectController<AbilitySphereData> { }

        public AbilitySphereController(AbilitySphereMotor motor, LayerMask shieldLayerMask, LayerMask earthLayerMask) 
            : base(motor, shieldLayerMask, earthLayerMask) { }
    }

    public sealed class AbilitySphereMotor : ProjectileObjectMotor<AbilitySphereData> { }

    #endregion
    

}