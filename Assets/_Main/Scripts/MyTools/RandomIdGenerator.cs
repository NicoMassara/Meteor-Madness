using System;
using System.Collections.Generic;

namespace _Main.Scripts
{
    public class RandomIdGenerator
    {
        private readonly HashSet<ulong> _inUseId = new HashSet<ulong>();
        private readonly System.Random random = new System.Random();

        public ulong Generate()
        {
            ulong value;
            int attempts = 0;

            do
            {
                value = NextUlong();
                attempts++;

                if (attempts > 100)
                {
                    break;
                }

            } while (_inUseId.Contains(value));

            _inUseId.Add(value);
            return value;
        }
        
        private ulong NextUlong()
        {
            byte[] bytes = new byte[8];
            random.NextBytes(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }
        
        public void Release(ulong value)
        {
            _inUseId.Remove(value);
        }
    }
}