using System;
using _Main.Scripts.Gameplay.Projectile.SO;
using MeteorMadness.Contracts;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.GameMode
{
    public class GameLevelController
    {
        private const int LevelAmount = GameParameters.GameplayValues.SpawnLevelAmount;
        private readonly IProjectileMilestoneData _milestoneData;
        private readonly int[] _milestones;
        private int _currentLevel;
        private int _currentStreak;
        private int _currentMilestone;

        public bool HasReachedMinLevel { get; private set; }
        public event Action OnLevelChange;
        public event Action OnMinLevelReached;

        public GameLevelController(IProjectileMilestoneData data)
        {
            _milestoneData = data;
            _milestones = new int[LevelAmount];
        }

        public void InitializeData()
        {
            Debug.Log(_milestoneData);

            for (int i = 0; i < _milestones.Length; i++)
            {
                var item = _milestoneData.GetMilestoneByIndex(i);
                if (item == null)
                {
                    Debug.LogWarning("Milestone " + i + " not found");
                    return;
                }

                var sqrRange = item.Range.sqrMagnitude;
                var milestoneAmount = 0;

                if (sqrRange == 0)
                {
                    milestoneAmount = i;
                }
                else
                {
                    milestoneAmount = _milestoneData.GetMilestoneByIndex(i).RandomRange;
                }
                ;
                _milestones[i] = milestoneAmount;
            }

            _currentMilestone = _milestones[0];
        }

        public void CheckForNextLevel()
        {
            if (_currentStreak >= _currentMilestone)
            {
                _currentLevel++;

                if (_currentLevel == GameParameters.GameplayValues.MinLevel)
                {
                    HasReachedMinLevel = true;
                    OnMinLevelReached?.Invoke();
                }

                _currentLevel = Math.Clamp(_currentLevel, 0, LevelAmount);
                _currentStreak = 0;
                
                SetTargetMilestone();
                
                OnLevelChange?.Invoke();
            }
        }

        private void SetTargetMilestone()
        {
            if (_currentLevel < LevelAmount)
            {
                _currentMilestone = _milestones[_currentLevel];
            }
            else
            {
                _currentMilestone += _milestoneData.GetMilestoneByIndex(GameParameters.GameplayValues.SpawnLevelAmount-1).RandomRange;
            }
        }

        public void IncreaseStreak()
        {
            _currentStreak++;
        }

        public void ResetLevel()
        {
            HasReachedMinLevel = false;
            _currentLevel = 0;
            _currentStreak = 0;
            OnLevelChange?.Invoke();
        }

        public int GetCurrentLevel()
        {
            return _currentLevel;
        }
    }
}