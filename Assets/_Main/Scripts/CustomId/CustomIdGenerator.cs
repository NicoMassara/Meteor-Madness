using System;
using System.Collections.Generic;

namespace _Main.Scripts.CustomId
{
    public class GeneratedId
    {
        internal ulong Id { get; private set; }
        public bool IsValid { get; private set; } = true;

        public GeneratedId(ulong value) => Id = value;

        internal void Invalidate()
        {
            IsValid = false;
        }

        public ulong GetId()
        {
            return IsValid ? Id : 0;
        }

        public override string ToString() => IsValid ? Id.ToString() : "[INVALIDATED]";
    }
    
    public class CustomIdGenerator
    {
        private const ulong NullId = 0;
        private readonly HashSet<ulong> _inUseId = new HashSet<ulong>();
        
        private readonly System.Random random = new System.Random();

        public GeneratedId Generate()
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
            
            return new GeneratedId(value);
        }
        
        private ulong NextUlong()
        {
            ulong value;

            do
            {
                byte[] bytes = new byte[8];
                random.NextBytes(bytes);
                value = BitConverter.ToUInt64(bytes, 0);

            } while (value == NullId);

            return value;
        }
        
        public void Release(GeneratedId generatedId)
        {
            if (generatedId == null) return;
    
            generatedId.Invalidate();
            _inUseId.Remove(generatedId.Id);
        }
    }
}