using System;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.ProjectileObject;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.Meteor
{
    internal class MeteorView : ProjectileObjectView<MeteorData,MeteorCollisionData>,
        MeteorView.IMeteorView,
        IDebugMeteor,
        IMeteor
    {
        internal interface IMeteorView : IProjectileObjectView<MeteorData> { }
        
    }
    
}