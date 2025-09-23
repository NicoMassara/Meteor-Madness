using System.Collections.Generic;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityStorage
    {
        private readonly Queue<AbilityType> _abilityQueue = new Queue<AbilityType>();
        private readonly int _maxAbilityCount;

        public UnityAction OnStorageFilled;
        public UnityAction<AbilityType> OnAbilityTaken;
        public UnityAction<AbilityType> OnAbilityAdded;

        public AbilityStorage(int maxAbilityCount)
        {
            _maxAbilityCount = maxAbilityCount;
        }

        public void AddAbility(AbilityType ability)
        {
            if(_abilityQueue.Count >= _maxAbilityCount) return;
            
            _abilityQueue.Enqueue(ability);

            if (_abilityQueue.Count == _maxAbilityCount)
            {
                OnStorageFilled?.Invoke();
            }
            
            OnAbilityAdded?.Invoke(ability);
        }

        public void TakeAbility()
        {
            OnAbilityTaken?.Invoke(_abilityQueue.Dequeue());
        }

        public bool IsEmpty()
        {
            return _abilityQueue.Count == 0;
        }

        public bool IsFull()
        {
            return _abilityQueue.Count == _maxAbilityCount;
        }
    }
}