using UnityEngine;

namespace _Main.Scripts.Projectile
{
    public struct ProjectileSpawnValues
    {
        public Vector2 Position;
        public Vector2 Direction;
        public float MovementSpeed;
        public float Value;
    }

    public struct BatchSpawnData
    {
        public Vector2 AmountRange;
        public Vector2 SlotRange;
        public float DistanceBetweenMeteors;
        public float DistanceBetweenBatch;
    }

    public interface ISpawnSlotData
    {
        
    }
    
    public struct SlotData
    {
        public int Slot;
        public float DistanceRatio;
    }
}