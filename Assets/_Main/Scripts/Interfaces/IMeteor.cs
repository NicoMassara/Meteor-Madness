using _Main.Scripts.Gameplay.Meteor;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Interfaces
{
    public interface IMeteor
    {
        public UnityAction<MeteorCollisionData> OnEarthCollision { get; set; }
        public UnityAction<MeteorCollisionData> OnDeflection { get; set; }
        public Vector2 Position { get;}
    }
}