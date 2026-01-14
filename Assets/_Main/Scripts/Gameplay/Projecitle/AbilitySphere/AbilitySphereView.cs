using MeteorMadness.Gameplay._Main.Scripts.Gameplay.ProjectileObject;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.AbilitySphere
{
    internal class AbilitySphereView : ProjectileObjectView<AbilitySphereData,AbilitySphereCollisionData>,
        AbilitySphereView.IAbilitySphereView,
        IAbilitySphere
    {
        internal interface IAbilitySphereView : IProjectileObjectView<AbilitySphereData> { }
    }
}