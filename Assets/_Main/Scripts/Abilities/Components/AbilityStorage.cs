using System;
using _Main.Scripts.CustomId;
using _Main.Scripts.SecurityData;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Abilities
{
    public class AbilityStorage
    {
        #region PrivateClasses
        private class BitMasking
        {
            private const int SlotSize = 3;
            private const int MaxSlots = 3;
            
            public int GetSlot(ref ushort abilities, int slotIndex)
            {
                int shift = slotIndex * SlotSize;
                return (abilities >> shift) & 0b111;
            }
            
            public void SetSlot(ref ushort abilities, int slotIndex, int value)
            {
                if (value < 0 || value > 7) throw new ArgumentOutOfRangeException();
                int shift = slotIndex * SlotSize;
                abilities &= (ushort)~(0b111 << shift);
                abilities |= (ushort)((value & 0b111) << shift);
            }
            
            public int Dequeue(ref ushort abilities)
            {
                int first = GetSlot(ref abilities,0);

                // mover slots 1 y 2 hacia la izquierda
                int slot1 = GetSlot(ref abilities,1);
                int slot2 = GetSlot(ref abilities,2);

                SetSlot(ref abilities,0, slot1);
                SetSlot(ref abilities,1, slot2);

                // slot2 queda vacío (o puede llenarse con un valor nuevo)
                SetSlot(ref abilities,2, 0);

                return first;
            }

            public void Enqueue(ref ushort abilities,int value)
            {
                if (value < 1 || value > 7) throw new ArgumentOutOfRangeException();

                for (int i = 0; i < MaxSlots; i++)
                {
                    if (GetSlot(ref abilities,i) == 0)
                    {
                        SetSlot(ref abilities,i, value);
                        return;
                    }
                }

                throw new InvalidOperationException("Queue is full");
            }
            
            public void Clear(ref ushort abilities)
            {
                abilities = 0; // pone todos los bits en 0
            }
            
            public int GetAmount(ref ushort abilities)
            {
                int count = 0;
                for (int i = 0; i < MaxSlots; i++)
                {
                    if (GetSlot(ref abilities,i) != 0)
                        count++;
                }
                return count;
            }
        }

        #endregion
        
        private const int MaxAbilityCount = 3;
        private readonly BitMasking _abilityQueue = new BitMasking();
        private GeneratedId _storedAbilitiesId;
        
        public UnityAction OnStorageFilled;
        public UnityAction<int> OnAbilityTaken;
        public UnityAction<int> OnAbilityAdded;

        public AbilityStorage()
        {
            _storedAbilitiesId = SecureValueManager.RegisterValue<ushort>();

            SecureValueManager.OnCheatDetected += OnCheatDetectedHandler;
        }

        public void Restart()
        {
            var storedValue = GetStoredAbilities();
            //
            _abilityQueue.Clear(ref storedValue);
            //
            UpdateStoredAbilities(storedValue);
        }

        public void AddAbility(int abilityIndex)
        {
            if(GetAbilityCount() >= MaxAbilityCount) return;

            var storedValue = GetStoredAbilities();
            //
            _abilityQueue.Enqueue(ref storedValue, abilityIndex);
            //
            UpdateStoredAbilities(storedValue);
            
            if (GetAbilityCount() == MaxAbilityCount)
            {
                OnStorageFilled?.Invoke();
            }
            
            OnAbilityAdded?.Invoke(abilityIndex);
        }

        public void TakeAbility()
        {
            var storedValue = GetStoredAbilities();
            //
            var index = _abilityQueue.Dequeue(ref storedValue);
            //
            UpdateStoredAbilities(storedValue);
            
            OnAbilityTaken?.Invoke(index);
        }

        public bool IsEmpty()
        {
            return GetAbilityCount() == 0;
        }

        public bool IsFull()
        {
            return GetAbilityCount() == MaxAbilityCount;
        }
        
        private ushort GetStoredAbilities()
        {
            return SecureValueManager.GetDoesContainValue(_storedAbilitiesId, out ushort storedAbilities) ? 
                storedAbilities : default(ushort);
        }

        private void UpdateStoredAbilities(ushort storedAbilities)
        {
            SecureValueManager.ModifyValue(_storedAbilitiesId,storedAbilities);
        }

        public int GetAbilityCount()
        {
            var value = GetStoredAbilities();
            return _abilityQueue.GetAmount(ref value);
        }
        
        private void OnCheatDetectedHandler(ushort id)
        {
            if (_storedAbilitiesId.Id == id)
            {
                Debug.LogWarning("CHEAT DETECTED! Clearing Ability Storage");
                Restart();
            }
        }
    }
}