using _Main.Scripts.CustomId;
using _Main.Scripts.Observer;
using _Main.Scripts.SecurityData;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeMotor : ObservableComponent
    {
#pragma warning disable CS0414 // Field is assigned but its value is never used
        private int _meteorCollisionCount;
#pragma warning restore CS0414 // Field is assigned but its value is never used

        private readonly GameLevelController _levelController;
        
        private float _startTimer;
        private float _lastDisplayedTimer;
        private readonly int _startDelay;
#pragma warning disable CS0414 // Field is assigned but its value is never used
        private bool _isPaused;
#pragma warning restore CS0414 // Field is assigned but its value is never used
        private bool _doesRestartGameMode;
        private bool _hasDoublePoints;
        private bool _canPause;
        private bool _hasGameplayPanelActive;
        private bool _hasPausePanelActive;

        private GeneratedId _highScoreSecuredId;
        private GeneratedId _currentScoreSecuredId;
        

        public GameModeMotor(int[] levelStreakAmount, int startTimer)
        {
            _levelController = new(levelStreakAmount);
            _levelController.OnLevelChange += OnLevelChangeHandler;
            _startDelay = startTimer + 1;

            _currentScoreSecuredId = SecureValueManager.RegisterValue<float>(0);

            SecureValueManager.OnCheatDetected += OnCheatDetectedHandler;
        }

        private void OnCheatDetectedHandler(ushort id)
        {
            if (_currentScoreSecuredId.Id == id)
            {
                Debug.LogWarning("Cheat Detected! Restarting Points!");
                UpdateCurrentScore(Mathf.NegativeInfinity);
            }
            
            if (_highScoreSecuredId.Id == id)
            {
                Debug.LogWarning("Cheat Detected! Restarting High Score!");
                UpdateHighScore(Mathf.NegativeInfinity);
            }
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
            var currentScore = GetCurrentScore();
            var highScore = GetHighScore();
            var hasBeaten = GetHasBeatenHighScore(currentScore);
            
            if (hasBeaten)
            {
                highScore = currentScore;
                UpdateHighScore(highScore);
                NotifyAll(GameModeObserverMessage.SaveHighScore, highScore);
            }
            
            NotifyAll(GameModeObserverMessage.SetHasHighScore, hasBeaten, highScore);
            NotifyAll(GameModeObserverMessage.EarthEndDestruction, currentScore);
        }
        
        public void EarthRestartFinish()
        {
            NotifyAll(GameModeObserverMessage.EarthRestartFinish, _doesRestartGameMode);
        }

        #endregion
        
        #region Ability

        public void SetDoublePoints(bool isEnable)
        {
            _hasDoublePoints = isEnable;
        }

        #endregion

        #region Spawn

        public void GrantSpawnMeteor(int projectileTypeIndex)
        {
            NotifyAll(GameModeObserverMessage.GrantProjectileSpawn,projectileTypeIndex);
        }
        
        public void SetEnableMeteorSpawn(bool canSpawn)
        {
            NotifyAll(GameModeObserverMessage.SetEnableSpawnMeteor, canSpawn);
        }

        #endregion

        #region GameMode
        
        public void StartGameplay()
        {
            NotifyAll(GameModeObserverMessage.StartGameplay);
        }
        
        public void HandleMeteorDeflect(Vector2 position, float meteorDeflectValue)
        {
            var finalValue = _hasDoublePoints ? meteorDeflectValue*2 : meteorDeflectValue;
            var currentScore = GetCurrentScore();
            
            currentScore += finalValue;
            
            if (meteorDeflectValue >= 1)
            {
                _levelController.IncreaseStreak();
                _levelController.CheckForNextLevel();
            }
            
            UpdateCurrentScore(currentScore);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD

            if (GetHasBeatenHighScore(currentScore))
            {
                NotifyAll(GameModeObserverMessage.UpdateHighScore, currentScore);
            }
#endif
            
            if (meteorDeflectValue > 0)
            {
                NotifyAll(GameModeObserverMessage.PointsGained,position,finalValue,_hasDoublePoints);
            }

            NotifyAll(GameModeObserverMessage.MeteorDeflect,currentScore);
        }
        
        public void LoadHighScore(float highScore)
        {
            _highScoreSecuredId = SecureValueManager.RegisterValue(highScore);
            NotifyAll(GameModeObserverMessage.UpdateHighScore, highScore);
        }

        public void InitializeValues()
        {
            NotifyAll(GameModeObserverMessage.InitializeValues);
        }

        public void SetCanPause(bool canPause)
        {
            _canPause = canPause;
            NotifyAll(GameModeObserverMessage.SetCanPause, _canPause);
        }

        public void SetDoesRestartGameMode(bool doesRestart)
        {
            _doesRestartGameMode = doesRestart;
        }

        public void StartCountdown()
        {
            _startTimer = _startDelay;
            _lastDisplayedTimer = Mathf.Infinity;
            NotifyAll(GameModeObserverMessage.StartCountdown);
        }
        
        public void HandleCountdownTimer(float deltaTime)
        {
            _startTimer -= deltaTime;
            int seconds = Mathf.CeilToInt(_startTimer);

            if (seconds != _lastDisplayedTimer)
            {
                _lastDisplayedTimer = seconds;
                NotifyAll(GameModeObserverMessage.UpdateCountdown, _startTimer);
                
                if (_startTimer <= 0)
                {
                    NotifyAll(GameModeObserverMessage.CountdownFinish);
                }
            }
        }

        private bool GetHasBeatenHighScore(float currentScore)
        {
            return currentScore > GetHighScore();
        }

        public void RestartValues()
        {
            _meteorCollisionCount = 0;
            UpdateCurrentScore(0);
            _levelController.ResetLevel();
        }

        public void DisableGameMode()
        {
            NotifyAll(GameModeObserverMessage.Disable);
        }
        
        public void Enable()
        {
            NotifyAll(GameModeObserverMessage.Enable);
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

        #endregion

        #region Pause
        
        public void PauseGame()
        {
            _isPaused = true;
            NotifyAll(GameModeObserverMessage.GamePaused);
        }
        
        public void UnPauseGame()
        {
            _isPaused = false;
            NotifyAll(GameModeObserverMessage.GameUnPaused);
        }

        #endregion

        #region Camera

        public void HandleCameraZoomOut()
        {
            NotifyAll(GameModeObserverMessage.CameraZoomOut);
        }

        public void HandleCameraZoomIn()
        {
            NotifyAll(GameModeObserverMessage.CameraZoomIn);
        }

        #endregion

        #region Screens

        public void SetGameplayPanel(bool isActive)
        {
            _hasGameplayPanelActive = isActive;
            NotifyAll(GameModeObserverMessage.GameplayPanel, _hasGameplayPanelActive);
        }

        public void SetPausePanel(bool isActive)
        {
            _hasPausePanelActive = isActive;
            NotifyAll(GameModeObserverMessage.PausePanel, _hasPausePanelActive);
        }
        
        public void TriggerOptions()
        {
            NotifyAll(GameModeObserverMessage.Options);
        }

        public void TriggerMainMenu()
        {
            NotifyAll(GameModeObserverMessage.TriggerMainMenu);
        }

        #endregion

        public void Asleep()
        {
            NotifyAll(GameModeObserverMessage.Asleep);
        }

        public void Leaving()
        {
            NotifyAll(GameModeObserverMessage.Leaving);
        }

        #region Secured Data

        private float GetHighScore()
        {
            var highScore = 0f;
            return SecureValueManager.GetDoesContainValue(_highScoreSecuredId, out highScore) ? highScore : 0;
        }

        private float GetCurrentScore()
        {
            var highScore = 0f;
            return SecureValueManager.GetDoesContainValue(_currentScoreSecuredId, out highScore) ? highScore : 0;
        }

        private void UpdateHighScore(float input)
        {
            SecureValueManager.ModifyValue(_highScoreSecuredId, input);
        }
        
        private void UpdateCurrentScore(float input)
        {
            SecureValueManager.ModifyValue(_currentScoreSecuredId, input);
        }
        
        #endregion

        public void InitializeData()
        {
            NotifyAll(GameModeObserverMessage.InitializeData);
        }
    }
}