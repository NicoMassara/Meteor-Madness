using System;
using UnityEngine;

namespace MeteorMadness.Contracts.Interfaces
{
    public interface IProjectile
    {
        public Vector2 Position { get; }
        public void SetEnableMovement(bool enable);
        public event Action OnDeath;
    }
}