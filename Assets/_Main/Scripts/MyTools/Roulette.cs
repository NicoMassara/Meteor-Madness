using System.Collections.Generic;
using UnityEngine;

namespace _Main.Scripts.MyTools
{
    public class Roulette
    {
        public T Run<T>(Dictionary<T, int> items)
        {
            int total = 0;
            foreach (var item in items)
            {
                total += item.Value;
            }
            
            int random = UnityEngine.Random.Range(1, total);

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