using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldCollider : MonoBehaviour
    {
        public UnityAction OnMeteorHit;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            OnMeteorHit?.Invoke();
        }
    }
}