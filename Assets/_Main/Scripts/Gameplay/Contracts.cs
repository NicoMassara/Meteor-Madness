using System;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay
{
    public interface ITargetable
    {
        public Vector2 Position { get;}
        public int Slot { get; }
        public bool CanBeTargeted { get;}
        public event Action<ITargetable> OnTargetDeath;
        public void DisableTargetable();
        public void EnableTargetable();
    }
}