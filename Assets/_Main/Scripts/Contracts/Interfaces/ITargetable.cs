using System;
using UnityEngine;

namespace MeteorMadness.Contracts.Interfaces
{
    public interface ITargetable
    {
        public Vector2 Position { get;}
        public bool CanBeTargeted { get;}
        public event Action OnDeath;
        public void DisableTargetable();
    }
}