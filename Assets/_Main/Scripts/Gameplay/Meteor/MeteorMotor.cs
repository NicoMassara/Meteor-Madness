using System;
using _Main.Scripts.Comet;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorMotor : FlyingObjectMotor
    {
        [Header("Layers Mask")]
        [SerializeField] private LayerMask shieldMask;
        [SerializeField] private LayerMask earthMask;
        
        //Reference to itself, hasHitShield
        public UnityAction<MeteorMotor, bool> OnHit;
        public UnityAction<MeteorMotor> OnRecycle;

        protected override void HandleCollision(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & shieldMask) != 0)
            {
                OnHit?.Invoke(this, true);
                CanMove = false;
                moveSound.StopSound();
                OnRecycle?.Invoke(this);
            }
            else if (((1 << other.gameObject.layer) & earthMask) != 0)
            {
                OnHit?.Invoke(this, false);
                CanMove = false;
                moveSound.StopSound();
                OnRecycle?.Invoke(this);
            }
        }

        public void ForceRecycle()
        {
            CanMove = false;
            moveSound.StopSound();
            OnRecycle?.Invoke(this);
        }
    }
}