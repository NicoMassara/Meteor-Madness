using System;
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

        private readonly StatController<uint> _uintDic = new StatController<uint>();
        private DataManagerTools.GameplayStatsIdData _runtimeData;

        private void Awake()
        {
            _uintDic.Initialize(new StatType[]
            {
                StatType.HighScore,
                StatType.Collision,
                StatType.Deflect,
                StatType.Ability,
                StatType.Streak,
                StatType.TimesPlayed,
                StatType.TotalScored,
                StatType.LongestTime,
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
                Internal_ModifyValueByStat(StatType.LongestTime, statsData.LongestTime);
            }
            else
            {
                Debug.Log("Stats saved data was not found!");
            }
            
            //
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
            if(_uintDic.DoesContain(statType))
                return _uintDic.GetValue(statType);
            
            throw new KeyNotFoundException($"{statType} stat was not found!");
        }
        private void Internal_AddValueByStat(StatType statType, object newValue)
        {
            if (_uintDic.DoesContain(statType))
            {
                var finalValue = _uintDic.GetValue(statType) + (uint)newValue;
                _uintDic.ModifyValue(statType, finalValue);
                return;
            }
            
            throw new KeyNotFoundException($"{statType} stat was not found!");
        }
        private void Internal_ModifyValueByStat(StatType statType, object newValue)
        {
            if (_uintDic.DoesContain(statType))
            {
                _uintDic.ModifyValue(statType, (uint)newValue);
                
                Debug.Log($"{statType} stat was set to: {newValue}");
                return;
            }
            
            throw new KeyNotFoundException($"{statType} stat was not found!");
        }
        private void Internal_ClearValueByStat(StatType statType)
        {
            if (_uintDic.DoesContain(statType))
            {
                _uintDic.ClearValue(statType);
                return;
            }
            
            throw new KeyNotFoundException($"{statType} stat was not found!");
        }
        private void Internal_SaveValues()
        {
            var statsData = DataManager.Instance.GetData<DataManager.StatsSaveData>(DataManager.SaveDataType.Stats);
            
            // === Longest Time === //
            // Compares the Time and if the recorded is greater, its overwrites it
            if (SecureValueManager.GetDoesContainValue<uint>(_runtimeData.TimeId, out var recordedTime))
            {
                if (recordedTime > (uint)Internal_GetValueByStat(StatType.LongestTime))
                {
                    Internal_ModifyValueByStat(StatType.LongestTime, recordedTime);
                    statsData.LongestTime = (uint)Internal_GetValueByStat(StatType.LongestTime);
                }
            }
            
            // === High Score ===//
            // Compares the HighScore with the RuntimeScore and if the runtime score is greater, overwrites it
            if (SecureValueManager.GetDoesContainValue<uint>(_runtimeData.RuntimeScoreId, out var recordedScore))
            {
                if (recordedScore > (uint)Internal_GetValueByStat(StatType.HighScore))
                {
                    Internal_ModifyValueByStat(StatType.HighScore, recordedScore);
                    statsData.HighScore = (uint)Internal_GetValueByStat(StatType.HighScore);
                }
            }
            
            // === Streak === //
            // Compares the Streak and if the recorded score is greater, its overwrites it
            if (SecureValueManager.GetDoesContainValue<uint>(_runtimeData.StreakId, out var recordedStreak))
            {
                if (recordedStreak > (uint)Internal_GetValueByStat(StatType.Streak))
                {
                    Internal_ModifyValueByStat(StatType.Streak, recordedStreak);
                    statsData.LongestStreak = (uint)Internal_GetValueByStat(StatType.Streak);
                }
            }
            
            // === Total Score === //
            // Adds to the total score the runtime one
            Internal_AddValueByStat(StatType.TotalScored, GetRuntimeScore());
            statsData.TotalScore = (uint)Internal_GetValueByStat(StatType.TotalScored);
            
            // === Times Played === //
            // Adds a game played
            Internal_AddValueByStat(StatType.TimesPlayed, (uint)1);
            statsData.GamesPlayed = (uint)Internal_GetValueByStat(StatType.TimesPlayed);
            
            // === Collision Count === //
            // Adds to the total collision count
            Internal_AddValueByStat(StatType.Collision, GetRuntimeValue(StatType.Collision));
            statsData.CollisionAmount = (uint)Internal_GetValueByStat(StatType.Collision);
            
            // === Deflect Count === //
            // Adds to the total deflect count
            Internal_AddValueByStat(StatType.Deflect, GetRuntimeValue(StatType.Deflect));
            statsData.DeflectAmount = (uint)Internal_GetValueByStat(StatType.Deflect);
            
            // === Ability Count === //
            // Adds to the total ability use count
            Internal_AddValueByStat(StatType.Ability, GetRuntimeValue(StatType.Ability));
            statsData.AbilityUseAmount = (uint)Internal_GetValueByStat(StatType.Ability);
            
            Debug.Log($"Collisions: {statsData.CollisionAmount}\n");
            
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

        private uint GetRuntimeValue(StatType statType)
        {
            if(_runtimeData == null)
                return 0;

            GeneratedId selectedId;
            
            switch (statType)
            {
                case StatType.HighScore: selectedId = _runtimeData.RuntimeScoreId; break;
                case StatType.Collision: selectedId = _runtimeData.CollisionId; break;
                case StatType.Deflect: selectedId = _runtimeData.DeflectId; break;
                case StatType.Ability: selectedId = _runtimeData.AbilityUseId; break;
                case StatType.Streak: selectedId = _runtimeData.StreakId; break;
                default: return 0;
            }
            
            if(SecureValueManager.GetDoesContainValue<uint>(selectedId, out var runtimeScore))
                return runtimeScore;
            
            return 0;
        }

        #endregion
        
    }
}