using System;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameLevelController
    {
        private int _currentLevel;
        private int _currentStreak;
        private readonly int[] levelStreakAmount;
        private int LevelLength => levelStreakAmount.Length-1;

        public event Action OnLevelChange;

        public GameLevelController(int[] levelStreakAmount)
        {
            this.levelStreakAmount = levelStreakAmount;
        }


        public void CheckForNextLevel()
        {
            if(_currentLevel == LevelLength) return;

            if (_currentStreak >= levelStreakAmount[_currentLevel])
            {
                _currentLevel++;
                _currentStreak = 0;
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