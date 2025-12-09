using System;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Shield
{
    public class ShieldMotor : ObservableComponent
    {
        private float _lastDirection = Mathf.Infinity;
        private ShieldType _currentShieldType;

        #region Movement
        public void Rotate(float direction = 1)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (direction != _lastDirection)
            {
                _lastDirection = direction;
                NotifyAll(ShieldObserverMessage.ChangedDirection);
            }
            
            NotifyAll(ShieldObserverMessage.Rotate, direction);
        }

        public void StopRotate()
        {
            _lastDirection = 0;
            NotifyAll(ShieldObserverMessage.StopRotate);
        }
        

        #endregion

        #region Enable/Disable
        
        public void EnableShield()
        {
            NotifyAll(ShieldObserverMessage.SetActiveShield, true);
        }

        public void DisableShield()
        {
            NotifyAll(ShieldObserverMessage.SetActiveShield, false);
            NotifyAll(ShieldObserverMessage.RestartPosition);
        }
        
        #endregion

        public void HandleHit(Vector3 position, Quaternion rotation, Vector2 direction)
        {
            NotifyAll(ShieldObserverMessage.Deflect,position, rotation, direction);
        }

        #region Ability

        public void SetAbility(ShieldType ability)
        {
            if(ability == ShieldType.None 
               && _currentShieldType == ShieldType.None) return;
            
            if (ability == ShieldType.None)
            {
                NotifyAll(GetAbilityMessage(_currentShieldType), false);
                _currentShieldType = ShieldType.None;
            }
            else
            {
                NotifyAll(GetAbilityMessage(ability), true);
                _currentShieldType = ability;
            }
        }

        private ulong GetAbilityMessage(ShieldType ability)
        {
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            return ability switch
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            {
                ShieldType.Super => ShieldObserverMessage.SetActiveSuperShield,
                ShieldType.Gold => ShieldObserverMessage.SetGold,
                ShieldType.Automatic => ShieldObserverMessage.SetAutomatic,
                ShieldType.Slow => ShieldObserverMessage.SetSlow,
            };
        }
        
        #endregion
    }
}