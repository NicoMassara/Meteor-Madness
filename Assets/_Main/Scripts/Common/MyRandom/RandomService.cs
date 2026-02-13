using System;

namespace _Main.Scripts.Common.MyRandom
{
    public class RandomService
    {
        private static MyRandom _global;
        private bool _isInitialized;

        public static void Initialize() => _global = new MyRandom((uint)DateTime.UtcNow.Ticks);
        public static int Range(int min, int max) => _global.Range(min, max);
        public static float Range(float min, float max) => _global.Range(min, max);
        public static float Value() => _global.Value();
    }
}