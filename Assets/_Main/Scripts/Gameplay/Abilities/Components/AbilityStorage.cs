using System.Collections.Generic;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityStorage
    {
        private readonly Queue<int> _abilityQueue = new Queue<int>();
        private readonly int _maxAbilityCount;

        public UnityAction OnStorageFilled;
        public UnityAction<int> OnAbilityTaken;
        public UnityAction<int> OnAbilityAdded;

        public AbilityStorage(int maxAbilityCount)
        {
            _maxAbilityCount = maxAbilityCount;
        }

        public void Restart()
        {
            _abilityQueue.Clear();
        }

        public void AddAbility(int abilityIndex)
        {
            if(_abilityQueue.Count >= _maxAbilityCount) return;
            
            _abilityQueue.Enqueue(abilityIndex);

            if (_abilityQueue.Count == _maxAbilityCount)
            {
                OnStorageFilled?.Invoke();
            }
            
            OnAbilityAdded?.Invoke(abilityIndex);
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