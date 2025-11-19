using UnityEngine;

namespace _Main.Scripts.MyTools
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