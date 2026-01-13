using UnityEngine;

namespace _Main.Scripts.Environment.Comet
{
    public static class CometHelper
    {
        public static float GetRandomFromRange(float minimum, float maximum) => Random.Range(minimum, maximum);
    }
}