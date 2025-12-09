using System;

namespace _Main.Scripts.Interfaces
{
    public interface IAbilitySelector
    {
        public int MinUnlockLevel { get;}
        public Tuple<AbilityType[],int[]> GetRarityValues();
        public Tuple<int[],AbilityType[]> GetUnlockLevelValues();
    }
}