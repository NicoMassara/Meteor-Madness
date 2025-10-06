using _Main.Scripts.Gameplay.Meteor;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeMotor : ObservableComponent
    {
        private float _meteorDeflectCount;
#pragma warning disable CS0414 // Field is assigned but its value is never used
        private int _meteorCollisionCount;
#pragma warning restore CS0414 // Field is assigned but its value is never used

        private readonly GameLevelController _levelController;
        
        private float _startTimer;
        private bool _isPaused;
        private bool _doesRestartGameMode;
        private bool _hasDoublePoints;
        

        public GameModeMotor(int[] levelStreakAmount)
        {
            _levelController = new(levelStreakAmount);
            _levelController.OnLevelChange += OnLevelChangeHandler;
        }

        public void InitializeValues()
        {
            NotifyAll(GameModeObserverMessage.Initialize);
        }
        
        public void SetDoesRestartGameMode(bool doesRestart)
        {
            _doesRestartGameMode = doesRestart;
        }

        public void StartCountdown(float time)
        {
            _startTimer = time + 1;
            NotifyAll(GameModeObserverMessage.StartCountdown);
        }
        
        public void HandleCountdownTimer(float deltaTime)
        {
            _startTimer -= deltaTime;

            NotifyAll(GameModeObserverMessage.UpdateCountdown, _startTimer);
            
            if (_startTimer <= 0)
            {
                NotifyAll(GameModeObserverMessage.CountdownFinish);
            }
        }

        public void StartGameplay()
        {
            NotifyAll(GameModeObserverMessage.StartGameplay);
        }

        public void HandleMeteorDeflect(Vector2 position, float meteorDeflectValue)
        {
            var finalValue = _hasDoublePoints ? meteorDeflectValue*2 : meteorDeflectValue;
            _meteorDeflectCount += finalValue;
            
            if (meteorDeflectValue >= 1)
            {
                _levelController.IncreaseStreak();
                _levelController.CheckForNextLevel();
            }

            if (meteorDeflectValue > 0)
            {
                NotifyAll(GameModeObserverMessage.PointsGained,position,finalValue,_hasDoublePoints);
            }

            NotifyAll(GameModeObserverMessage.MeteorDeflect,_meteorDeflectCount);
        }

        public void RestartValues()
        {
            _meteorCollisionCount = 0;
            _meteorDeflectCount = 0;
            _levelController.ResetLevel();
        }

        #region Earth

        public void HandleEarthShake()
        {
            NotifyAll(GameModeObserverMessage.EarthShaking);
        }

        public void HandleEarthStartDestruction()
        {
            NotifyAll(GameModeObserverMessage.EarthStartDestruction);
        }

        public void HandleEarthEndDestruction()
        {
            NotifyAll(GameModeObserverMessage.EarthEndDestruction, _meteorDeflectCount);
        }
        
        public void EarthRestartFinish()
        {
            NotifyAll(GameModeObserverMessage.EarthRestartFinish, _doesRestartGameMode);
        }

        #endregion
        
        public void SetEnableMeteorSpawn(bool canSpawn)
        {
            NotifyAll(GameModeObserverMessage.SetEnableSpawnMeteor, canSpawn);
        }

        public void UpdateCurrentLevel()
        {
            NotifyAll(GameModeObserverMessage.UpdateGameLevel, _levelController.GetCurrentLevel());
        }

        public void HandleGameFinish()
        {
            NotifyAll(GameModeObserverMessage.GameFinish);
        }
        
        private void OnLevelChangeHandler()
        {
            UpdateCurrentLevel();
        }

        public void GameRestart()
        {
            NotifyAll(GameModeObserverMessage.GameRestart);
        }

        public void SetGamePaused(bool isPaused)
        {
            _isPaused = isPaused;
            NotifyAll(GameModeObserverMessage.GamePaused, _isPaused);
        }
        
        public void DisableGameMode()
        {
            NotifyAll(GameModeObserverMessage.Disable);
        }

        public void SetDoublePoints(bool isEnable)
        {
            _hasDoublePoints = isEnable;
        }
        
        public void GrantSpawnMeteor(int projectileTypeIndex)
        {
            NotifyAll(GameModeObserverMessage.GrantProjectileSpawn,projectileTypeIndex);
        }
    }
}