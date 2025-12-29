using System.Collections.Generic;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.GlobalValues.BaseSingleton;
using MeteorMadness.GlobalValues.Tools;
using MeteorMadness.Managers.Save;
using UnityEngine;

namespace MeteorMadness.Managers
{
    public class StatsManager : SingletonBehaviour<StatsManager>
    {
        #region Private Classes

        private interface IStat<T> where T : struct
        {
            public T GetValue();

            public void ModifyValue(T value);

            public void ClearValue();
        }
        private class StatData<T> : IStat<T> where T : struct
        {
            private readonly GeneratedId _secureId;

            public StatData(GeneratedId secureId)
            {
                _secureId = secureId;
            }

            public T GetValue()
            {
                SecureValueManager.GetDoesContainValue<T>(_secureId, out var storedValue);
                return storedValue;
            }

            public void ModifyValue(T value)
            {
                SecureValueManager.ModifyValue<T>(_secureId, value);
            }

            public void ClearValue()
            {
                SecureValueManager.ModifyValue<T>(_secureId, default);
            }
        }

        private class StatController<T> where T : struct
        {
            private readonly Dictionary<StatType, IStat<T>> _statsDic = new Dictionary<StatType, IStat<T>>();

            public void Initialize(StatType[] initStats)
            {
                foreach (var stat in initStats)
                {
                    var id = SecureValueManager.RegisterValue<T>();
                    _statsDic.Add(stat, new StatData<T>(id));
                }
                
            }

            public bool DoesContain(StatType statType)
            {
                return _statsDic.ContainsKey(statType);
            }

            public T GetValue(StatType statType)
            {
                if (!_statsDic.TryGetValue(statType, out var value))
                {
                    Debug.Log("Stat was not found!");
                    return default;
                }
                
                return value.GetValue();
            }

            public void ModifyValue(StatType statType, T newValue)
            {
                if (!_statsDic.TryGetValue(statType, out var storedStat))
                {
                    Debug.Log("Stat was not found!");
                    return;
                }
                
                storedStat.ModifyValue(newValue);
            }

            public void ClearValue(StatType statType)
            {
                if (!_statsDic.TryGetValue(statType, out var storedStat))
                {
                    Debug.Log("Stat was not found!");
                    return;
                }
                
                storedStat.ClearValue();
            }
        }
        
        #endregion

        private readonly StatController<uint> _uintStats = new StatController<uint>();
        private readonly StatController<float> _floatStats = new StatController<float>();
        private DataManagerTools.GameplayStatsIdData _runtimeData;

        private void Awake()
        {
            _uintStats.Initialize(new StatType[]
            {
                StatType.HighScore,
                StatType.Collision,
                StatType.Deflect,
                StatType.Ability,
                StatType.Streak,
                StatType.TimesPlayed,
                StatType.TotalScored,
            });
            
            _floatStats.Initialize(new StatType[]
            {
                StatType.Time,
            });

            BootEvents.OnMainSystemRequestInitialize += Initialize;
        }

        private void Initialize()
        {
            BootEvents.OnMainSystemRequestInitialize -= Initialize;
            //

            var statsData = DataManager.Instance.GetData<DataManager.StatsSaveData>(DataManager.SaveDataType.Stats);


            if (statsData != null)
            {
                Internal_ModifyValueByStat(StatType.HighScore, statsData.HighScore);
                Internal_ModifyValueByStat(StatType.Collision, statsData.CollisionAmount);
                Internal_ModifyValueByStat(StatType.Deflect, statsData.DeflectAmount);
                Internal_ModifyValueByStat(StatType.Ability, statsData.AbilityUseAmount);
                Internal_ModifyValueByStat(StatType.Streak, statsData.LongestStreak);
                Internal_ModifyValueByStat(StatType.TimesPlayed, statsData.GamesPlayed);
                Internal_ModifyValueByStat(StatType.TotalScored, statsData.TotalScore);
                Internal_ModifyValueByStat(StatType.Time, statsData.LongestTime);
            }
            else
            {
                Debug.Log("Stats saved data was not found!");
            }
            


            BootEvents.MainSystemInitialized();
        }

        #region Public

        public static object GetValueByStat(StatType statType) => Instance.Internal_GetValueByStat(statType);
        public static void AddValueByStat(StatType statType, object newValue) => Instance.Internal_AddValueByStat(statType, newValue);
        public static void ModifyValueByStat(StatType statType, object newValue) => Instance.Internal_ModifyValueByStat(statType, newValue);
        public static void ClearValueByStat(StatType statType) => Instance.Internal_ClearValueByStat(statType);
        public static void SaveValues() => Instance.Internal_SaveValues();

        public static void UpdateRuntimeData(DataManagerTools.GameplayStatsIdData runtimeData)
            => Instance.Internal_UpdateRuntimeData(runtimeData);

        public static void ClearRuntimeData() => Instance.Internal_ClearRuntimeData();
        
        public static uint GetRuntimeScore() => Instance.Internal_GetRuntimeScore();

        #endregion
        
        #region Internal

        private object Internal_GetValueByStat(StatType statType)
        {
            if(_uintStats.DoesContain(statType))
                return _uintStats.GetValue(statType);
            if(_floatStats.DoesContain(statType))
                return _floatStats.GetValue(statType);
            
            throw new KeyNotFoundException($"{statType} stat was not found!");
        }
        private void Internal_AddValueByStat(StatType statType, object newValue)
        {
            if (_uintStats.DoesContain(statType))
            {
                var finalValue = _uintStats.GetValue(statType) + (uint)newValue;
                _uintStats.ModifyValue(statType, finalValue);
                return;
            }
            
            if (_floatStats.DoesContain(statType))
            {
                var finalValue = _floatStats.GetValue(statType) + (float)newValue;
                _floatStats.ModifyValue(statType, finalValue);
                return;
            }
            
            throw new KeyNotFoundException($"{statType} stat was not found!");
        }
        private void Internal_ModifyValueByStat(StatType statType, object newValue)
        {
            if (_uintStats.DoesContain(statType))
            {
                _uintStats.ModifyValue(statType, (uint)newValue);
                return;
            }

            if (_floatStats.DoesContain(statType))
            {
                _floatStats.ModifyValue(statType, (float)newValue);
                return;
            }
            
            throw new KeyNotFoundException($"{statType} stat was not found!");
        }
        private void Internal_ClearValueByStat(StatType statType)
        {
            if (_uintStats.DoesContain(statType))
            {
                _uintStats.ClearValue(statType);
                return;
            }

            if (_floatStats.DoesContain(statType))
            {
                _floatStats.ClearValue(statType);
                return;
            }
            
            throw new KeyNotFoundException($"{statType} stat was not found!");
        }
        private void Internal_SaveValues()
        {
            var statsData = DataManager.Instance.GetData<DataManager.StatsSaveData>(DataManager.SaveDataType.Stats);

            statsData.HighScore = (uint)Internal_GetValueByStat(StatType.HighScore);
            statsData.CollisionAmount = (uint)Internal_GetValueByStat(StatType.Collision);
            statsData.DeflectAmount = (uint)Internal_GetValueByStat(StatType.Deflect);
            statsData.AbilityUseAmount = (uint)Internal_GetValueByStat(StatType.Ability);
            statsData.LongestStreak = (uint)Internal_GetValueByStat(StatType.Streak);
            statsData.GamesPlayed = (uint)Internal_GetValueByStat(StatType.TimesPlayed);
            statsData.TotalScore = (uint)Internal_GetValueByStat(StatType.TotalScored);
            statsData.LongestTime = (float)Internal_GetValueByStat(StatType.Time);
            
            DataManager.Instance.SaveGameData(statsData, DataManager.SaveDataType.Stats);
        }

        private void Internal_UpdateRuntimeData(DataManagerTools.GameplayStatsIdData runtimeData)
        {
            _runtimeData = runtimeData;
        }

        private void Internal_ClearRuntimeData()
        {
            _runtimeData = null;
        }
        
        private uint Internal_GetRuntimeScore()
        {
            if(_runtimeData == null)
                return 0;
            
            if(SecureValueManager.GetDoesContainValue<uint>(_runtimeData.RuntimeScoreId, out var runtimeScore))
                return runtimeScore;
            
            return 0;
        }

        #endregion

        #region Tools

        private void ModifyValueBySecureId<T>(StatType statType, GeneratedId secureId) where T : struct
        {
            if (SecureValueManager.GetDoesContainValue<T>(secureId, out var securedValue))
            {
                Internal_ModifyValueByStat(statType, securedValue);
            }
        }
        
        private void AddValueBySecureId<T>(StatType statType, GeneratedId secureId) where T : struct
        {
            if (SecureValueManager.GetDoesContainValue<T>(secureId, out var securedValue))
            {
                Internal_AddValueByStat(statType, securedValue);
            }
        }

        #endregion
    }
}