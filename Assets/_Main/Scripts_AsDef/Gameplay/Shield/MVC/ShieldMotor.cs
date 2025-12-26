using System;
using MeteorMadness.GlobalValues;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;

namespace MeteorMadness.Gameplay.Shield
{
    public class ShieldMotor : ObservableComponent
    {
        private ShieldType _currentShieldType;

        #region Movement
        public void Rotate(float direction = 1) 
            => NotifyAll(ShieldObserverMessage.Rotate, direction);

        public void StopRotate() 
            => NotifyAll(ShieldObserverMessage.StopRotate);

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
            => NotifyAll(ShieldObserverMessage.Deflect,position, rotation, direction);

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