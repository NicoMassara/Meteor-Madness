using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityMotor : ObservableComponent
    {
        private readonly AbilityStorage _storage;
        private AbilityType _currentAbility;
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
                Debug.Log("Can't use ability");
                return;
            }

            if (_storage.IsEmpty())
            {
                Debug.Log("No Ability in Storage"); 
                return;
            }

            _storage.TakeAbility();
        }
        
        public void TryAddAbility(AbilityType ability)
        {
            if (_storage.IsFull())
            {
                Debug.Log("Ability storage is full");
                return;
            }

            _storage.AddAbility(ability);
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
        
        public void RestartAbilities()
        {
            _currentAbility = AbilityType.None;
            _canUseAbility = false;
            _storage.Restart();
            NotifyAll(AbilityObserverMessage.RestartAbilities);
        }
        
        #region Handlers

        private void Storage_OnAbilityAddedHandler(AbilityType abilityType)
        {
            NotifyAll(AbilityObserverMessage.AddAbility, abilityType);
        }

        private void Storage_OnAbilityTakenHandler(AbilityType abilityType)
        {
            _currentAbility = abilityType;
            
            NotifyAll(AbilityObserverMessage.SelectAbility, _currentAbility);
        }

        private void Storage_OnStorageFilledHandler()
        {
            NotifyAll(AbilityObserverMessage.StorageFilled);
        }

        #endregion


    }
    
    public enum AbilityType
    {
        None,
        SuperShield,
        Health,
        SlowMotion
    }
}