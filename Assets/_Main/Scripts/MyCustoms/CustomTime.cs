using UnityEngine;

namespace _Main.Scripts.MyCustoms
{
    public static class CustomTime
    {
        public static float DeltaTime { get; private set; }
        public static float FixedDeltaTime { get; private set; }

        public static float TimeScale { get; private set; } = 1f;
        public static bool IsPaused { get; private set; }

        public static void SetTimeScale(float scale) => TimeScale = Mathf.Max(0f, scale);
        public static void SetPaused(bool value) => IsPaused = value;

        internal static void UpdateFrame(float unscaledDeltaTime)
        {
            DeltaTime = IsPaused ? 0f : unscaledDeltaTime * TimeScale;
        }

        internal static void UpdateFixed(float fixedUnscaledDeltaTime)
        {
            FixedDeltaTime = IsPaused ? 0f : fixedUnscaledDeltaTime * TimeScale;
        }
    }
}