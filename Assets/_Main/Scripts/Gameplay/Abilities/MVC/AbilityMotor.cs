using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityMotor : ObservableComponent
    {
        private readonly AbilityStorage _storage;
        private int _currentAbilityIndex;
        private bool _canUseAbility;
        private bool _isUIEnable;

        public AbilityMotor(int maxAbilityStorage)
        {
            _storage = new AbilityStorage(maxAbilityStorage);
            Initialize();
        }

        private void Initialize()
        {
            _storage.OnAbilityAdded += Storage_OnAbilityAddedHandler;
            _storage.OnAbilityTaken += Storage_OnAbilityTakenHandler;
            _storage.OnStorageFilled += Storage_OnStorageFilledHandler;
        }
        
        public void SelectAbility()
        {
            if (_canUseAbility == false)
            {
                return;
            }

            if (_storage.IsEmpty())
            {
                return;
            }

            _storage.TakeAbility();
        }
        
        public void TryAddAbility(int abilityIndex)
        {
            if (_storage.IsFull() || abilityIndex == 0)
            {
                return;
            }

            _storage.AddAbility(abilityIndex);
        }

        public void TriggerAbility()
        {
            NotifyAll(AbilityObserverMessage.TriggerAbility, _currentAbilityIndex);
        }

        public void FinishAbility()
        {
            NotifyAll(AbilityObserverMessage.FinishAbility, _currentAbilityIndex);
            _currentAbilityIndex = 0;
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
        
        public void RestartAbilities()
        {
            _currentAbilityIndex = 0;
            _canUseAbility = false;
            _storage.Restart();
            NotifyAll(AbilityObserverMessage.RestartAbilities);
        }
        
        #region Handlers

        private void Storage_OnAbilityAddedHandler(int abilityTypeIndex)
        {
            NotifyAll(AbilityObserverMessage.AddAbility, abilityTypeIndex);
        }

        private void Storage_OnAbilityTakenHandler(int abilityTypeIndex)
        {
            _currentAbilityIndex = abilityTypeIndex;
            
            NotifyAll(AbilityObserverMessage.SelectAbility, _currentAbilityIndex);
            NotifyAll(AbilityObserverMessage.SetStorageFull, false);
        }

        private void Storage_OnStorageFilledHandler()
        {
            NotifyAll(AbilityObserverMessage.SetStorageFull, true);
        }
        

        #endregion


        public void RunActiveTimer()
        {
            NotifyAll(AbilityObserverMessage.RunActiveTimer,(int)_currentAbilityIndex);
        }
    }
    
    public enum AbilityType
    {
        None,
        SuperShield,
        Health,
        SlowMotion
    }
}