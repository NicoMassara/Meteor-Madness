using System.Collections.Generic;
using _Main.Scripts.Common.MyRandom;

namespace _Main.Scripts.Common
{
    public class Roulette
    {
        public static T Run<T>(Dictionary<T, int> items)
        {
            int total = 0;
            foreach (var item in items)
            {
                total += item.Value;
            }
            
            int random = RandomService.Range(1, total);

            foreach (var item in items)
            {
                random -= item.Value;
                if (random <= 0)
                {
                    return item.Key;
                }
            }

            return default(T);
        }
    }
}