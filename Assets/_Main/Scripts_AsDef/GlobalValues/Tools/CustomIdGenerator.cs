using System;
using System.Collections.Generic;

namespace MeteorMadness.GlobalValues.Tools
{
    public class GeneratedId
    {
        internal ushort Id { get; private set; }
        public bool IsValid => Id > 0;

        private event Action<GeneratedId> _onRelease;

        public GeneratedId(ushort id, Action<GeneratedId> onRelease)
        {
            Id = id;
            _onRelease = onRelease;
        }

        public void Reset()
        {
            Id = 0;
        }

        public ushort GetId()
        {
            return (ushort)(IsValid ? Id : 0);
        }

        public override string ToString() => IsValid ? Id.ToString() : "[INVALIDATED]";
    }
    
    public class CustomIdGenerator
    {
        private const ushort NullId = 0; // Default ID used as null

        private readonly HashSet<ushort> _inUseId;// IDs currently in use
        private ushort _nextId = 1; // Start from 1 (0 = NullId)
        
        public CustomIdGenerator(int initialSize = 10)
        {
            initialSize = Math.Clamp(initialSize, 0, ushort.MaxValue);
            _inUseId = new HashSet<ushort>(initialSize);
        }

        /// <summary>
        /// Generates an incremental GeneratedId
        /// </summary>
        public GeneratedId Generate()
        {
            if (_inUseId.Count >= ushort.MaxValue - 1)
                throw new InvalidOperationException("All available IDs are in use.");

            // Find the next free ID
            while (_inUseId.Contains(_nextId) || _nextId == NullId)
            {
                _nextId++;

                if (_nextId == ushort.MaxValue)
                    _nextId = 1; // Wrap around if overflow
            }

            ushort value = _nextId;
            _inUseId.Add(value);
            _nextId++;

            return new GeneratedId(value, Release);
        }

        private void Release(GeneratedId idData)
        {
            _inUseId.Remove(idData.Id);
            idData.Reset();
        }
        
    }
    
}