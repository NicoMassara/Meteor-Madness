using System;
using UnityEngine;

namespace _Main.Scripts.Projectile
{
    #region Struct

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
    
    public struct SlotData
    {
        public int Slot;
        public float DistanceRatio;
        public bool IsAbility;
    }

    public struct BatchDebugData
    {
        public int Level;
        public int Amount;
        public int StartSlot;
        public int Offset;
        public float InnerDist;
        public float NextDist;
        public int LastSlot;
        public SpawnType SpawnType;
        
    }

    #endregion

    #region Interfaces

    public interface IProjectileSpawnData
    {
        public IBatchSpawnData GetDataByIndex(int index);
    }
    
    public interface ISlotRangeData
    {
        public Vector2Int Range { get; }
        public int RandomRange { get; }
    }

    public interface IFloatRangeData
    {
        public Vector2 Range { get; }
        public float RandomRange { get; }
    }
    
    public interface IIntRangeData
    {
        public Vector2Int Range { get; }
        public int RandomRange { get; }
    }
    
    public interface IEnumRangeData
    {
        public IEnumWeight[] WeightData { get; }
    }

    public interface IEnumWeight
    {
        public float Weight { get; }
        public SpawnType SpawnType { get; }
    }

    public interface IBatchSpawnData
    {
        public IIntRangeData ProjectileAmountRange { get; }
        public ISlotRangeData  SlotRange { get; }
        public IFloatRangeData InnerBatchDistanceRange { get; }
        public IFloatRangeData NextBatchDistanceRange { get; }
        public IFloatRangeData NextBatchDelayRange { get; }
        public ISlotRangeData  NextBatchSlotRange { get; }
        public IEnumRangeData SpawnTypeRange { get; }
    }

    #endregion
    
    #region Enums

    public enum SpawnType
    {
        None,
        Random,
        Ascendent,
        Descendent,
        SamePosition
    }

    #endregion
}