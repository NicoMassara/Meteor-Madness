using UnityEngine;

namespace MeteorMadness.GlobalValues._Main.Scripts.GlobalValues.Tools
{
    public static class EarthShakerMultiplier
    {
        public static float Get(float healthRatio)
        {
            healthRatio = 1f - healthRatio;
            var lerpValue = Mathf.Clamp01(healthRatio / 0.9f);
            return Mathf.Lerp(0.1f, 1f, lerpValue);
        }
    }
}