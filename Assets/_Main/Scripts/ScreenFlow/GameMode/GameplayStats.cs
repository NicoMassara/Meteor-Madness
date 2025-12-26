using System;
using MeteorMadness.GlobalValues.Tools;
using MeteorMadness.Managers;
using MeteorMadness.Managers.Save;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.GameMode
{
    public class GameplayStats
    {
        private readonly GeneratedId _currentScoreId;
        private readonly GeneratedId _collisionId;
        private readonly GeneratedId _abilityUseId;
        private readonly GeneratedId _deflectId;
        private readonly GeneratedId _currentStreakId;
        private readonly GeneratedId _timeId;
        private readonly GeneratedId _maxStreakId;
        
        public event Action OnCheatDetected;
        public event Action<uint> OnStreakUpdated;

        public GameplayStats()
        {
            _currentScoreId = SecureValueManager.RegisterValue<uint>(0);
            _collisionId = SecureValueManager.RegisterValue<uint>(0);
            _abilityUseId = SecureValueManager.RegisterValue<uint>(0);
            _deflectId = SecureValueManager.RegisterValue<uint>(0);
            _currentStreakId = SecureValueManager.RegisterValue<uint>(0);
            _maxStreakId = SecureValueManager.RegisterValue<uint>(0);
            _timeId = SecureValueManager.RegisterValue<float>(0);

            SecureValueManager.OnCheatDetected += OnCheatDetectedHandler;
        }

        private void OnCheatDetectedHandler(ushort id)
        {
            InitializeValues();
            Debug.LogWarning("Cheat Detected! Restarting Ability Stats!");
            
            OnCheatDetected?.Invoke();
        }

        public void InitializeValues()
        {
            UpdateCurrentScore(0);
            UpdateValueById<uint>(_collisionId,0);
            UpdateValueById<uint>(_abilityUseId,0);
            UpdateValueById<uint>(_deflectId,0);
            UpdateValueById<uint>(_currentStreakId,0);
            UpdateValueById<float>(_timeId,0f);
        }

        public DataManagerTools.GameplayStatsIdData CreateGameplayStatsData()
        {
            return new DataManagerTools.GameplayStatsIdData
            {
                CurrentScoreId = _currentScoreId,
                CollisionId = _collisionId,
                AbilityUseId = _abilityUseId,
                DeflectId = _deflectId,
                StreakId = _maxStreakId,
                TimeId = _timeId,
            };
        }

        #region Score

        public uint GetCurrentScore()
        {
            return GetValueById<uint>(_currentScoreId);
        }

        public void UpdateCurrentScore(uint currentScore)
        {
            UpdateValueById(_currentScoreId, currentScore);
        }

        #endregion
        
        #region Stats

        public void IncreaseCollisionCount()
        {
            var current = GetValueById<uint>(_collisionId);
            current++;
            UpdateValueById(_collisionId,current);
            ClearStreak();
        }

        public void IncreaseAbilityUseCount()
        {
            var current = GetValueById<uint>(_abilityUseId);
            current++;
            UpdateValueById(_abilityUseId,current);
        }

        public void IncreaseDeflectCount()
        {
            var current = GetValueById<uint>(_deflectId);
            current++;
            UpdateValueById(_deflectId,current);
            IncreaseDeflectStreak();
        }

        private void IncreaseDeflectStreak()
        {
            var current = GetValueById<uint>(_currentStreakId);
            current++;
            UpdateValueById(_currentStreakId,current);
            
            OnStreakUpdated?.Invoke(current);
            
            var max = GetValueById<uint>(_maxStreakId);
            if (current > max) UpdateValueById(_maxStreakId,current);
        }
        
        private void ClearStreak()
        {
            OnStreakUpdated?.Invoke(0);
            
            UpdateValueById<uint>(_currentStreakId,0);
        }

        public void IncreaseTimer(float deltaTime)
        {
            var current = GetValueById<float>(_timeId);
            current += deltaTime;
            UpdateValueById(_timeId,current);
        }

        #endregion

        #region Base Methods

        private T GetValueById<T>(GeneratedId id) where T : struct 
        {
            return SecureValueManager.GetDoesContainValue(id, out T value) ? value : default;
        }

        private void UpdateValueById<T>(GeneratedId id, T input) where T : struct 
        {
            SecureValueManager.ModifyValue(id, input);
        }

        #endregion



    }
}