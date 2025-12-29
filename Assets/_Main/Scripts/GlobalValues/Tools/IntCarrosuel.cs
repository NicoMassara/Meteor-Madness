using UnityEngine;

namespace MeteorMadness.GlobalValues.Tools
{
    public class ValueCarousel
    {
        public static int GetValue(int current, int max)
        {
            current++;
            return ((current % max) + max) % max;
        }
    }
}