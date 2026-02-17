using System;
using System.Collections.Generic;
using _Main.Scripts.Common.MyRandom;
using _Main.Scripts.Common.SelectorByWeight;
using _Main.Scripts.Gameplay.Projectile.SO;
using _Main.Scripts.Gameplay.Projectile.SO.WeightsData;
using MeteorMadness.Contracts;
using UnityEngine;

namespace _Main.Scripts.Projectile
{
    #region Struct
    

    [System.Serializable]
    internal struct SpawnData
    {
        [SerializeField] private SpeedMultiplierRangeData speedMultiplier;
        [SerializeField] private AmountRangeData batchAmount;
        [SerializeField] private SpawnTypeData[] data;
        public SpawnTypeData[] Data => data;

        public SpeedMultiplierRangeData SpeedMultiplier => speedMultiplier;
        public AmountRangeData BatchAmount => batchAmount;

        public bool GetSpawnDataByType(SpawnType spawnType, out SpawnTypeData spawnData)
        {
            spawnData = default;
            
            foreach (var item in data)
            {
                if (item.SpawnType == spawnType)
                {
                    spawnData = item;
                    return true;
                }
            }
            
            return false;
        }

        public Dictionary<SpawnType, int> GetWeights()
        {
            var temp  = new Dictionary<SpawnType, int>();

            foreach (var item in data)
            {
                if(temp.ContainsKey(item.SpawnType)) continue;
                
                temp[item.SpawnType] = item.Weight;
            }
            return temp;
        }
    }

    [System.Serializable]
    internal struct SpawnTypeData
    {
        [SerializeField] private SpawnType spawnType;
        [Range(0, 100)]
        [SerializeField] private int weight;
        [SerializeField] private AmountRangeData projectileAmount;
        [SerializeField] private DistanceRangeData innerBatchDistance;
        [SerializeField] private DistanceRangeData nextBatchDistance;
        [SerializeField] private SlotRangeData nextBatchSlotRange;
        [SerializeField] private SlotRangeData slotRange;

        public int Weight => weight;
        public SpawnType SpawnType => spawnType;
        public AmountRangeData ProjectileAmount => projectileAmount;
        public DistanceRangeData InnerBatchDistance => innerBatchDistance;
        public DistanceRangeData NextBatchDistance => nextBatchDistance;
        public SlotRangeData NextBatchSlotRange => nextBatchSlotRange;
        public SlotRangeData SlotRange => slotRange;
    }

    #region Range Data
    
    [System.Serializable]
    internal struct AmountRangeData
    {
        [Min(1)] 
        [SerializeField] private int minAmount;
        [Min(1)] 
        [SerializeField] private int maxAmount;
        
        public Vector2Int GetRange() => new Vector2Int(minAmount, maxAmount);
        public int GetRandomRange() => RandomService.Range(minAmount, maxAmount);
    }
    
    
    [System.Serializable]
    internal struct SpeedMultiplierRangeData
    {
        [Range(0.01f, 2)]
        [SerializeField] private float minSpeed;
        [Range(0.01f, 2)] 
        [SerializeField] private float maxSpeed;
        
        public Vector2 GetRange() => new Vector2(minSpeed, maxSpeed);
        public float GetRandomRange() => RandomService.Range(minSpeed, maxSpeed);
    }

    [System.Serializable]
    public struct SlotRangeData
    {
        [Range(1, GameParameters.GameplayValues.AngleSlots/2)]
        [SerializeField] private int minRange;
        [Range(1, GameParameters.GameplayValues.AngleSlots/2)]
        [SerializeField] private int maxRange;
        
        public Vector2Int GetRange() => new Vector2Int(minRange, maxRange);
        public int GetRandomRange() => RandomService.Range(minRange, maxRange);
    }
    
    [System.Serializable]
    internal struct DistanceRangeData
    {
        [Range(0, 1)]
        [Tooltip("0f - Spawn Point / 1f - Shield")]
        [SerializeField] private float minRange;
        [Range(0, 1)] 
        [Tooltip("0f - Spawn Point / 1f - Shield")]
        [SerializeField] private float maxRange;
        
        public Vector2 GetRange() => new Vector2(minRange, maxRange);
        public float GetRandomRange() => RandomService.Range(minRange, maxRange);
    }
    
    #endregion

    public struct ProjectileSpawnValues
    {
        public Vector2 Position;
        public Vector2 Direction;
        public int Slot;
        public float MovementSpeed;
        public float Value;
    }

    public struct BatchSpawnData
    {
        public float MovementSpeed;
        public int Amount;
        public int SlotRange;
        public float InnerDistance;
        public float NextDistance;
        public float Delay;
        public int NextSlotRange;
        public SpawnType SpawnType;
        public SlotRangeData SlotRangeData;
        public bool HasAbility;
    }
    
    public struct SlotData
    {
        public int Slot;
        public float DistanceRatio;
        public float MovementSpeed;
        public bool IsAbility;
        public float FinalValue;
    }

    public struct DefaultBatchDebugData
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
    
    public struct BatchDebugData
    {
        public int Amount;
        public int StartSlot;
        public int Offset;
        public float InnerDist;
        public float NextDist;
        public int LastSlot;
        public SpawnType SpawnType;
    }
    

    public struct HistoryDebugData
    {
        public int Amount;
        public SpawnType[] History;
        public HistoryWeightData[] Weights;
    }

    public struct HistoryWeightData
    {
        public int[] Values;
    }

    #endregion

    #region Interfaces

    public interface IBatchTypeCreator
    {
        public BatchSpawnData GetBatchSpawnData(SelectRandomSpawnDelegate selectRandomSpawn);
        public float GetProjectileValue(int index, int batchAmount);

        public void RestartValues();
    }

    public interface ILeveledBatchTypeCreator
    {
        public void SetLevel(int currentLevel);
    }

    internal interface IBatchDataBase
    {
       public BatchType BatchType { get; }
       public float ProjectileSpeed { get; }
    }

    internal interface IDefaultBatchData : IBatchDataBase
    {
        public SpawnData[] FixedBatchValues { get;  }
        public SpawnData RandomBatchValues { get;  }
    }

    internal interface ISingleBatchData : IBatchDataBase
    {
        public SpawnData BatchValues { get;  }
    }
    
    [System.Serializable]
    internal struct BatchTypeData
    {
        public DefaultBatchData defaultData;
        public RingBatchData ringData;
        public SlowMotionBatchData slowMotionData;
        public AutomaticBatchData automaticData;
    }
    

    [System.Serializable]
    internal struct DefaultBatchData
    {
        public DefaultBatchDataSo data;
        public SpawnWeightsDataSo weights;
    }
    
    [System.Serializable]
    internal struct RingBatchData
    {
        public ISingleBatchData data;
        public SpawnWeightsDataSo weights;
    }
    
    [System.Serializable]
    internal struct SlowMotionBatchData
    {
        public ISingleBatchData data;
        public SpawnWeightsDataSo weights;
    }
    
    [System.Serializable]
    internal struct AutomaticBatchData
    {
        public ISingleBatchData data;
        public SpawnWeightsDataSo weights;
    }
    

    #endregion
    
    #region Enums

    public enum SpawnType
    {
        None,
        Random,
        Ascendent,
        Descendent,
        SamePosition,
        UpAndDown,
        
        DEFAULT_MAX
    }

    #endregion

    public delegate SpawnType SelectRandomSpawnDelegate(Dictionary<SpawnType, int> currWeights,
        ISpawnBaseWeights baseWeights);
    

    [System.Serializable]
    public class WeightsPair : WeightsPairBase<SpawnType> { }
    [System.Serializable]
    public class WeightsData : WeightsBaseData<SpawnType, WeightsPair>, IWeightsData { }
    public interface IWeightsData : IWeightsBaseData<SpawnType> { }

    [System.Serializable]
    public class SpawnBaseWeights : ISpawnBaseWeights
    {
        public WeightsData[] weights;
        
        private Dictionary<SpawnType, IWeightsData> _cache;

        public void BuildCache()
        {
            _cache = new Dictionary<SpawnType, IWeightsData>();
            foreach (var w in weights)
            {
                _cache[w.itemType] = w;
            }
        }

        public Dictionary<SpawnType, IWeightsData> GetWeights()
        {
            if(_cache == null)
                BuildCache();
            
            return _cache;
        }

        public void TryCreateDefaultArray()
        {
            if (weights == null || weights.Length != (int)SpawnType.DEFAULT_MAX - 1)
            {
                var count = (int)SpawnType.DEFAULT_MAX - 1;
                weights = new WeightsData[count];
            
                for (int i = 0; i < count; i++)
                {
                    var item = new WeightsData
                    {
                        itemType = (SpawnType)i + 1
                    };
                    
                    item.CreateDefaultData();

                    weights[i] = item;
                }
            }
        }
    }
    public interface ISpawnBaseWeights : IBaseWeights<SpawnType, IWeightsData> { }
    

    public interface ISpawnWeightsData
    {
        public ISpawnBaseWeights GetWeights();
    }
}