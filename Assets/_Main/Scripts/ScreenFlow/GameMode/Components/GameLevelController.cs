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

        public event Action OnLevelChange;

        public GameLevelController(IProjectileMilestoneData data)
        {
            _milestoneData = data;
            _milestones = new[] { LevelAmount };
        }

        public void InitializeData()
        {
            for (int i = 0; i < _milestones.Length; i++)
            {
                var sqrRange = _milestoneData.GetMilestoneByIndex(i).Range.sqrMagnitude;
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
                _currentMilestone += _milestoneData.GetMilestoneByIndex(_currentLevel).RandomRange;
            }
        }

        public void IncreaseStreak()
        {
            _currentStreak++;
        }

        public void ResetLevel()
        {
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