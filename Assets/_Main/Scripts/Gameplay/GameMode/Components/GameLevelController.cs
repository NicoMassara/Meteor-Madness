using System;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameLevelController
    {
        private int _currentLevel;
        private int _currentStreak;
        private readonly int[] levelStreakAmount = new int[]
        {
            1,
            1,
            1,
            2,
            2,
            3,
            3,
            3,
            4,
            5
        };

        public event Action OnLevelChange;

        public void CheckForNextLevel()
        {
            if(_currentLevel >= levelStreakAmount.Length) return;

            if (_currentStreak >= levelStreakAmount[_currentLevel])
            {
                _currentLevel++;
                OnLevelChange?.Invoke();
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