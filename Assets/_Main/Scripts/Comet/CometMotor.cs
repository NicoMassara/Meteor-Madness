using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _Main.Scripts.Comet
{
    public class CometMotor : FlyingObjectMotor
    {
        [SerializeField] private LayerMask cometWallMask;
        
        public UnityAction<CometMotor> OnRecycle;
        protected override void HandleCollision(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & cometWallMask) != 0)
            {
                Debug.Log("Recycle");
                CanMove = false;
                moveSound?.StopSound();
                OnRecycle?.Invoke(this);
            }
        }
        
        public void ForceRecycle()
        {
            CanMove = false;
            moveSound?.StopSound();
            OnRecycle?.Invoke(this);
        }
    }
}