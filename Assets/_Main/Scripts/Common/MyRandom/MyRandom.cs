using UnityEngine;

namespace _Main.Scripts.Common.MyRandom
{
    public class MyRandom
    {
        private uint _seed;

        public MyRandom(uint seed)
        {
            this._seed = seed;
            Debug.Log("Seed: " + _seed);
        }

        // LCG clásico
        private uint NextUInt()
        {
            _seed = _seed * 1664525u + 1013904223u;
            return _seed;
        }

        public int Range(int min, int max)
        {
            if(min == max || min >= max)
                return min;

            return (int)(NextUInt() % (uint)(max - min)) + min;
        }
        
        public float Range(float min, float max)
        {
            if (min >= max)
                return min;
            
            return min + Value() * (max - min);
        }
        
        public float Value()
        {
            return (NextUInt() >> 8) * (1f / (1 << 24));
        }
    }
}