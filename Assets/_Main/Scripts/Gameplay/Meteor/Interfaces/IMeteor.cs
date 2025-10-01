using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Meteor
{
    public interface IMeteor
    {
        public UnityAction<MeteorCollisionData> OnEarthCollision { get; set; }
        public UnityAction<MeteorCollisionData> OnDeflection { get; set; }
        public Vector2 Position { get;}
    }
}