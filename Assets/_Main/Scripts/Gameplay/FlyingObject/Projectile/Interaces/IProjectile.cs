using UnityEngine;

namespace _Main.Scripts.Gameplay.FlyingObject.Projectile
{
    public interface IProjectile
    {
        public Vector2 Position { get; }
        public bool EnableMovement { get; set; }
    }
}