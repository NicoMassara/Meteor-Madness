using System.Collections.Generic;
using _Main.Scripts.DebugGUI;

namespace _Main.Scripts.Gameplay.Abilities
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD

    public class AbilityDebugData
    {
        private int _availableIndex;
        
        private readonly List<AbilityType> _storedAbility = new List<AbilityType>
        {
            AbilityType.None,
            AbilityType.None,
            AbilityType.None,
        };
        
        public AbilityDebugData()
        {
            DebugGUIManager.Instance.CreateGroup(DebugGUIKeys.Group.Gameplay, DebugGUISortingOrder.Group.Gameplay)
                ?.CreateSubGroup(DebugGUIKeys.SubGroup.Ability, DebugGUISortingOrder.SubGroup.Ability)
                ?.AddEntry(
                    () => $"Slot 1: {GetStoredAbilityName(0)}",
                    () => $"Slot 2: {GetStoredAbilityName(1)}",
                    () => $"Slot 3: {GetStoredAbilityName(2)}"
                );
        }
        
        private string GetStoredAbilityName(int index)
        {
            return _storedAbility[index].ToString();
        }

        public void AddAbility(int abilityIndex)
        {
            var ability = (AbilityType)abilityIndex;
            _storedAbility[_availableIndex] = ability;
            _availableIndex++;
        }

        public void RemoveAbility()
        {
            _availableIndex--;
            _storedAbility[0] = _storedAbility[1]; 
            _storedAbility[1] = _storedAbility[2];
            _storedAbility[2] = AbilityType.None;
        }
    }    

#endif
    
}