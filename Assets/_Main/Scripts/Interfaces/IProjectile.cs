using UnityEngine;

namespace _Main.Scripts.Interfaces
{
    public interface IProjectile
    {
        public Vector2 Position { get; }
        public bool EnableMovement { get; set; }
    }
}