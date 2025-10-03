using System;
using _Main.Scripts.Gameplay.Abilies;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilities
{
    public static class AbilityDataGetter
    {
        public static Color GetColor(AbilityType ability)
        {
            var newColor = ability switch
            {
                AbilityType.SuperShield => Color.cyan,
                AbilityType.SlowMotion => Color.green,
                AbilityType.Health => Color.blue,
                AbilityType.DoublePoints => Color.yellow,
                AbilityType.Automatic => Color.red,
                _ => Color.clear
            };
            
            return newColor;
        }

        public static string GetDisplayName(AbilityType ability)
        {
            var newColor = ability switch
            {
                AbilityType.SuperShield => "Super Shield",
                AbilityType.SlowMotion => "Slow Motion",
                AbilityType.Health => "Reconstruction",
                AbilityType.DoublePoints => "Double Points",
                AbilityType.Automatic => "Auto-Shield",
                AbilityType.None => "None",
                _ => throw new ArgumentOutOfRangeException(nameof(ability), ability, null)
            };

            return newColor;
        }
    }
}