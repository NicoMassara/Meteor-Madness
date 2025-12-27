using System;
using MeteorMadness.Contracts;
using MeteorMadness.Managers.Localization;

namespace MeteorMadness.Gameplay.Abilities
{
    public static class AbilityDataGetter
    {
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