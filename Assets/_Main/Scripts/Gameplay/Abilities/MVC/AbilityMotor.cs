using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities
{
    public class AbilityMotor : ObservableComponent
    {
        private readonly AbilityStorage _storage;
        private Vector2 _abilityAddedPosition;
        private bool _isEnable;
        private bool _canUse;
        private bool _hasAbilityRunning;
        private int _currentAbilityIndex;

        public AbilityMotor()
        {
            _storage = new AbilityStorage();
            Initialize();
        }

        private void Initialize()
        {
            _storage.OnStorageFilled += Storage_OnStorageFilledHandler;
        }

        public void SetEnable(bool isEnable)
        {
            _isEnable = isEnable;
        }
        
        public void SetCanUse(bool canUse)
        {
            _canUse = canUse;
        }
        

        public void TryTriggerAbility()
        {
            if (_canUse == false ||
                _isEnable == false || 
                _storage.IsEmpty())
            {
                return;
            }
            
            _currentAbilityIndex = _storage.TakeAbility();
            
            _hasAbilityRunning = true;
            NotifyAll(AbilityObserverMessage.TriggerAbility, _currentAbilityIndex);
            NotifyAll(AbilityObserverMessage.SetStorageFull, false);
        }
        
        public void TryAddAbility(int abilityIndex, Vector2 abilityPosition)
        {
            if (_storage.IsFull() ||
                abilityIndex == 0 ||
                _canUse == false ||
                _isEnable == false)
            {
                return;
            }
            
            _storage.AddAbility(abilityIndex);
            _abilityAddedPosition = abilityPosition;
            NotifyAll(AbilityObserverMessage.AddAbility, abilityIndex, _abilityAddedPosition);
        }

        public void FinishAbility()
        {
            _hasAbilityRunning = false;
            _currentAbilityIndex = 0;
        }
        
        public void ForceFinishAbility()
        {
            if(_hasAbilityRunning == false) return;
            
            NotifyAll(AbilityObserverMessage.ForceFinish);
        }
        
        public void RestartValues()
        {
            _currentAbilityIndex = 0;
            _hasAbilityRunning = false;
            _canUse = false;
            _storage.Restart();
            NotifyAll(AbilityObserverMessage.RestartValues);
        }

        #region Handlers
        

        private void Storage_OnStorageFilledHandler()
        {
            NotifyAll(AbilityObserverMessage.SetStorageFull, true);
        }
        

        #endregion
    }
}