using System;
using MeteorMadness.Contracts;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.ProjectileObject;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.AbilitySphere
{
    public class AbilitySphereView : ProjectileObjectView<AbilitySphereData,AbilitySphereCollisionData>,
        AbilitySphereView.IAbilitySphereView,
        IAbilitySphere,
        IAbilitySphereColor,
        IDebugAbilitySphere
    {
        internal interface IAbilitySphereView : IProjectileObjectView<AbilitySphereData> { }

        public event Action<AbilityType> OnAbilitySet;
        public AbilityType DebugAbility { get; set; }

        protected override void HandleSetValues(AbilitySphereData data)
        {
            base.HandleSetValues(data);
            OnAbilitySet?.Invoke(data.Ability);
            DebugAbility = data.Ability;
        }

    }
}