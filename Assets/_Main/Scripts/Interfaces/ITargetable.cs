using System;
using UnityEngine;

namespace _Main.Scripts.Interfaces
{
    public interface ITargetable
    {
        public Vector2 Position { get;}
        public event Action OnDeath;
    }
}