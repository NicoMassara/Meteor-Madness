using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldColliderExtender
    {
        private readonly CapsuleCollider2D _collider;
        private readonly float _defaultYSize;

        public ShieldColliderExtender(CapsuleCollider2D collider)
        {
            _collider = collider;
            
            _defaultYSize = _collider.size.y;
        }

        public void Extend()
        {
            _collider.size = new Vector2(_collider.size.x, _defaultYSize + 0.5f);
        }
        
        public void Retract()
        {
            _collider.size = new Vector2(_collider.size.x, _defaultYSize);
        }
    }
}