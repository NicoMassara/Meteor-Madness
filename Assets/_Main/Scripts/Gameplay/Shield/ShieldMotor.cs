using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldMotor : ObservableComponent
    {
        private float _lastDirection;

        public ShieldMotor()
        {
            
        }

        #region Movement
        public void Rotate(float direction = 1)
        {
            if (direction != _lastDirection)
            {
                _lastDirection = direction;
                NotifyAll(ShieldObserverMessage.PlayMoveSound);
            }
            
            NotifyAll(ShieldObserverMessage.Rotate, direction);
        }

        public void StopRotate()
        {
            _lastDirection = 0;
            NotifyAll(ShieldObserverMessage.StopRotate);
        }

        public void RestartPosition()
        {
            NotifyAll(ShieldObserverMessage.RestartPosition);
        }

        #endregion

        #region Sprites

        public void SetSpriteByEnum(SpriteType spriteType)
        {
            NotifyAll(ShieldObserverMessage.SetSpriteType, spriteType);
        }

        public void SetActiveShield(bool isActive)
        {
            NotifyAll(ShieldObserverMessage.SetActiveShield, isActive);
        }

        #endregion

        public void HandleHit(Vector3 position, Quaternion rotation, Vector2 direction)
        {
            NotifyAll(ShieldObserverMessage.Deflect,position, rotation, direction);
        }
    }

    public enum SpriteType
    {
        Normal,
        Super
    }
}