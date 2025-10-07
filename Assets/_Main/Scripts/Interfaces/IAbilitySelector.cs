using System.Collections.Generic;

namespace _Main.Scripts.Interfaces
{
    public interface IAbilitySelector
    {
        public (AbilityType[] abilityTypes, int[] rarityValues) GetRarityValues();
        public (int[] unlockLevels, AbilityType[] abilityTypes) GetUnlockLevelValues();
    }
}