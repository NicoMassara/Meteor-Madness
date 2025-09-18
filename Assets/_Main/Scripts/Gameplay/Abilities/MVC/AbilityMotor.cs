using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityMotor : ObservableComponent
    {
        private AbilityType _currentAbility;
        private bool _canUseAbility;
        private bool _isUIEnable;

        public void SelectAbility(AbilityType ability)
        {
            if (_canUseAbility == false)
            {
                Debug.Log("Can't use ability");
                return;
            }
            
            _currentAbility = ability;
            
            NotifyAll(AbilityObserverMessage.SelectAbility, _currentAbility);
        }

        public void TriggerAbility()
        {
            NotifyAll(AbilityObserverMessage.TriggerAbility, _currentAbility);
        }

        public void FinishAbility()
        {
            NotifyAll(AbilityObserverMessage.FinishAbility, _currentAbility);
            _currentAbility = AbilityType.None;
        }

        public void SetCanUseAbility(bool canUse)
        {
            _canUseAbility = canUse;
            NotifyAll(AbilityObserverMessage.SetCanUse, _canUseAbility);
        }

        public void SetEnableUI(bool isEnable)
        {
            _isUIEnable = isEnable;
            NotifyAll(AbilityObserverMessage.SetEnableUI, _isUIEnable);
        }
    }
    
    public enum AbilityType
    {
        None,
        SuperShield,
        SlowMotion,
        Health
    }
}