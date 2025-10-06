using System.Collections.Generic;

namespace _Main.Scripts.Interfaces
{
    public interface IGameAbilitySelector
    {
        public (AbilityType[] abilityTypes, int[] rarityValues) GetRarityValues();
        public (int[] unlockLevels, AbilityType[] abilityTypes) GetUnlockLevelValues();
    }
}