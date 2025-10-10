using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldColliderExtender
    {
        private readonly CapsuleCollider2D _collider;
        private readonly float _defaultYSize;
        private bool _isExtended;

        public ShieldColliderExtender(CapsuleCollider2D collider)
        {
            _collider = collider;
            
            _defaultYSize = _collider.size.y;
        }

        public void Extend()
        {
            if (_isExtended == true) return;
            
            _collider.size = new Vector2(_collider.size.x, _defaultYSize + 0.5f);
            _isExtended = true;
        }
        
        public void Retract()
        {
            if (_isExtended == false) return;
            
            _collider.size = new Vector2(_collider.size.x, _defaultYSize);
            
            _isExtended = false;
        }
    }
}