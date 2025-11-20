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
        private float _lastDisplayedTimer;
        private readonly int _startDelay;
#pragma warning disable CS0414 // Field is assigned but its value is never used
        private bool _isPaused;
#pragma warning restore CS0414 // Field is assigned but its value is never used
        private bool _doesRestartGameMode;
        private bool _hasDoublePoints;
        private bool _canPause;
        private float _highScore;
        private bool _hasGameplayPanelActive;
        private bool _hasPausePanelActive;
        

        public GameModeMotor(int[] levelStreakAmount, int startTimer)
        {
            _levelController = new(levelStreakAmount);
            _levelController.OnLevelChange += OnLevelChangeHandler;
            _startDelay = startTimer + 1;
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
            var hasBeaten = GetHasBeatenHighScore();
            
            if (hasBeaten)
            {
                _highScore = _meteorDeflectCount;
                NotifyAll(GameModeObserverMessage.SaveHighScore, _highScore);
            }
            
            NotifyAll(GameModeObserverMessage.SetHasHighScore, hasBeaten, _highScore);
            NotifyAll(GameModeObserverMessage.EarthEndDestruction, _meteorDeflectCount);
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
            _meteorDeflectCount += finalValue;
            
            if (meteorDeflectValue >= 1)
            {
                _levelController.IncreaseStreak();
                _levelController.CheckForNextLevel();
            }
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD

            if (GetHasBeatenHighScore())
            {
                NotifyAll(GameModeObserverMessage.UpdateHighScore, _meteorDeflectCount);
            }
#endif
            
            if (meteorDeflectValue > 0)
            {
                NotifyAll(GameModeObserverMessage.PointsGained,position,finalValue,_hasDoublePoints);
            }

            NotifyAll(GameModeObserverMessage.MeteorDeflect,_meteorDeflectCount);
        }
        
        public void SetHighScore(float highScore)
        {
            _highScore = highScore;
            NotifyAll(GameModeObserverMessage.UpdateHighScore, _highScore);
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

        public bool GetHasBeatenHighScore()
        {
            return _meteorDeflectCount > _highScore;
        }

        public void RestartValues()
        {
            _meteorCollisionCount = 0;
            _meteorDeflectCount = 0;
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
    }
}