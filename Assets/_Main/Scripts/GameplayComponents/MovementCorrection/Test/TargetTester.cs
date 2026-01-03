using System;
using UnityEngine;

namespace _Main.Scripts.GameplayComponents.MovementCorrection.Test
{
    public class TargetTester : MonoBehaviour, ITargetable
    {
        public Vector2 Position => transform.position;
        public bool CanBeTargeted { get; } = true;
        public event Action OnDeath;
        public void DisableTargetable()
        {
            gameObject.SetActive(false);
        }
    }
}