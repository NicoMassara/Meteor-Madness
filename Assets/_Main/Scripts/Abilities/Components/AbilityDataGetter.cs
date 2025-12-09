using System;
using _Main.Scripts.Localization;
using UnityEngine;

namespace _Main.Scripts.Abilities
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
                AbilityType.SuperShield => GetLocalizedText("SuperShield"),
                AbilityType.SlowMotion => GetLocalizedText("SlowMotion"),
                AbilityType.Health => GetLocalizedText("Health"),
                AbilityType.DoublePoints => GetLocalizedText("DoublePoints"),
                AbilityType.Automatic => GetLocalizedText("Automatic"),
                AbilityType.None => "NULL",
                _ => throw new ArgumentOutOfRangeException(nameof(ability), ability, null)
            };

            return newColor;
        }

        private static string GetLocalizedText(string key)
        {
            return LocalizationManager.Instance.GetText($"Abilities.{key}");
        }
    }
}