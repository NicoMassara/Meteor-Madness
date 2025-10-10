using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldMotor : ObservableComponent
    {
        private float _lastDirection;
        private bool _isTotalActive;
        private bool _isGolden;
        private bool _isAutomatic;
        private bool _isSlow;

        #region Movement
        public void Rotate(float direction = 1)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
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
        
        public void SetActiveSuperShield(bool isActive)
        {
            _isTotalActive = isActive;
            NotifyAll(ShieldObserverMessage.SetActiveSuperShield,_isTotalActive);
        }

        public void ForceRotate(float direction = 1)
        {
            NotifyAll(ShieldObserverMessage.Rotate, direction);
        }

        public void RestartPosition()
        {
            NotifyAll(ShieldObserverMessage.RestartPosition);
        }

        #endregion

        #region Sprites

        public void SetActiveShield(bool isActive)
        {
            NotifyAll(ShieldObserverMessage.SetActiveShield, isActive);
        }

        #endregion

        public void HandleHit(Vector3 position, Quaternion rotation, Vector2 direction)
        {
            NotifyAll(ShieldObserverMessage.Deflect,position, rotation, direction);
        }

        public void SetActiveGold(bool isActive)
        {
            _isGolden = isActive;
            NotifyAll(ShieldObserverMessage.SetGold,_isGolden);
        }

        public void SetActiveAutomatic(bool isActive)
        {
            _isAutomatic = isActive;
            NotifyAll(ShieldObserverMessage.SetAutomatic,_isAutomatic);
        }

        public void SetActiveSlow(bool isActive)
        {
            _isSlow = isActive;
            NotifyAll(ShieldObserverMessage.SetSlow,_isSlow);
        }
    }
}