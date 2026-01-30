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

    public sealed class AbilitySphereController : ProjectileObjectController<AbilitySphereData, AbilitySphereMotor, AbilitySphereCollisionData>,
        AbilitySphereController.IAbilitySphereController
    {
        internal interface IAbilitySphereController : IProjectileObjectController<AbilitySphereData> { }

        public AbilitySphereController(AbilitySphereMotor motor, LayerMask shieldLayerMask, LayerMask earthLayerMask) 
            : base(motor, shieldLayerMask, earthLayerMask) { }
    }

    public sealed class AbilitySphereMotor : ProjectileObjectMotor<AbilitySphereData,AbilitySphereCollisionData>
    {
        private AbilitySphereCollisionData _collisionData;
        protected override AbilitySphereCollisionData GetCollisionData()
        {
            if (_collisionData == null)
            {
                _collisionData = new AbilitySphereCollisionData();
            }
            
            _collisionData.Position = Data.Position;
            _collisionData.Rotation = Data.Rotation;
            _collisionData.Direction = Data.Direction;
            
            return _collisionData;
        }

        public override void SetValues(AbilitySphereData data)
        {
            base.SetValues(data);
            
            if (_collisionData == null)
            {
                _collisionData = new AbilitySphereCollisionData();
            }

            _collisionData.Ability = data.Ability;
        }
    }

    #endregion
    

}