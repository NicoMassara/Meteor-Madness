using UnityEngine;

namespace MeteorMadness.GlobalValues.Utilities
{
    public class AbilityColorHelper
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
    }
}