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
    
    public interface IEnumRangeData<T>
        where T : Enum 
    {
        public Vector2Int Range { get; }
        public T RandomRange { get; }
    }
    
    public interface IBatchSpawnData
    {
        public IIntRangeData AmountRange { get; }
        public ISlotRangeData  SlotRange { get; }
        public IFloatRangeData InnerBatchDistanceRange { get; }
        public IFloatRangeData NextBatchDistanceRange { get; }
        public IFloatRangeData NextBatchDelayRange { get; }
        public ISlotRangeData  NextBatchSlotRange { get; }
        public IEnumRangeData<SpawnType> SpawnTypeRange { get; }
    }

    #endregion
    
    #region Enums

    public enum SpawnType
    {
        Random,
        Ascendent,
        Descendent,
        //SamePosition
            
        DEFAULT_MAX
    }

    #endregion
}