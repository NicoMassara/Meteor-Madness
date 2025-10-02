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
                AbilityType.Health => Color.red,
                AbilityType.DoublePoints => Color.yellow,
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
                AbilityType.Health => "Health",
                AbilityType.DoublePoints => "Double Points",
                AbilityType.None => "None",
                _ => throw new ArgumentOutOfRangeException(nameof(ability), ability, null)
            };

            return newColor;
        }
    }
}