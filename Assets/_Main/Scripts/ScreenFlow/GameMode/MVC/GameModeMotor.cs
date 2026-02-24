using System;
using _Main.Scripts.Gameplay.Projectile.SO;
using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.GameMode
{
    public class GameModeMotor : ObservableComponent
    {
        // Gameplay Values
        private readonly GameLevelController _levelController;
        private bool _hasDoublePoints;
        private readonly int _startDelay;
        private bool _canPause;
#pragma warning disable CS0414 // Field is assigned but its value is never used
        private bool _isPaused;
#pragma warning restore CS0414 // Field is assigned but its value is never used

        private readonly GameplayStats _stats;

        private class StreakMilestoneNotifier
        {
            private int _nextMilestone;
            private readonly int _step;
            
            public event Action<int> OnMilestoneReached;


            public StreakMilestoneNotifier(int step = 10)
            {
                _step = step;
                ResetMilestone();
            }

            public void UpdateStreak(int current)
            {
                if (current == 0)
                {
                    ResetMilestone();
                    return;
                }
                
                if(current == 1) return;

                if (current >= _nextMilestone)
                {
                    _nextMilestone += _step;
                    OnMilestoneReached?.Invoke(current);
                }
            }

            public void ResetMilestone()
            {
                _nextMilestone = _step;
            }
        }
        
        private readonly StreakMilestoneNotifier _streakNotifier;

        public GameModeMotor(IProjectileMilestoneData milestoneData)
        {
            _levelController = new(milestoneData);
            _levelController.OnLevelChange += UpdateCurrentLevel;
            _levelController.OnMinLevelReached += OnMinLevelReachedHandler;
            
            _stats = new GameplayStats();
            _stats.OnCheatDetected += OnCheatDetectedHandler;
            _stats.OnStreakUpdated += OnStreakUpdated;
            
            _streakNotifier = new StreakMilestoneNotifier();
            _streakNotifier.OnMilestoneReached += OnStreakMilestoneReached;
        }

        private void RestartValues()
        {
            _hasDoublePoints = false;
            _canPause = true;
            _isPaused = false;
            _levelController.ResetLevel();
        }
        
        #region Enable / Disable

        public void StartDisable()
        {
            NotifyAll(GameModeObserverMessage.StartDisable);
        }
        
        public void ExecuteDisable()
        {
            if (_isPaused)
            {
                NotifyAll(GameModeObserverMessage.GameInterrupted);
            }

            NotifyAll(GameModeObserverMessage.ExecuteDisable);
            RestartValues();
        }

        public void TriggerEarthDestruction()
        {
            NotifyAll(GameModeObserverMessage.TriggerEarthDestruction);
        }
        
        public void StartFinish()
        {
            NotifyAll(GameModeObserverMessage.StartFinish);
        }

        #endregion

        #region Pause
        
        public void PauseGame()
        {
            _isPaused = true;
            NotifyAll(GameModeObserverMessage.GamePaused);
        }
        
        public void ResumeGame()
        {
            _isPaused = false;
            NotifyAll(GameModeObserverMessage.GameResume);
        }
        
        public void EnablePause()
        {
            _canPause = true;
            NotifyAll(GameModeObserverMessage.SetCanPause, _canPause);
        }

        public void DisablePause()
        {
            _canPause = false;
            NotifyAll(GameModeObserverMessage.SetCanPause, _canPause);
        }
        
        public void TriggerPause()
        {
            NotifyAll(GameModeObserverMessage.PauseGameModeScreen);
        }
        
        public void TriggerPauseMenu()
        {
            NotifyAll(GameModeObserverMessage.OpenPauseMenu);
        }

        #endregion

        #region Data

        public void InitializeData()
        {
            _stats.InitializeValues();
            _levelController.InitializeData();
            NotifyAll(GameModeObserverMessage.InitializeData);
        }

        public void SaveScore()
        {
            // The View receives this data and stores it in the StatsManager
            // So the DefeatScreen can use it 
            NotifyAll(GameModeObserverMessage.SaveScore, _stats.CreateGameplayStatsData());
        }

        #endregion    
        
        #region Meteor 
        
        public void NotifyBatchDeflected()
        {
            if (_levelController.HasReachedMinLevel == false)
            {
                _levelController.IncreaseStreak();
                _levelController.CheckForNextLevel();
            }
        }
        
        public void NotifyBatchFinished()
        {
            if (_levelController.HasReachedMinLevel)
            {
                _levelController.IncreaseStreak();
                _levelController.CheckForNextLevel();
            }
        }
        
        public void HandleMeteorDeflect(Vector2 position, float projectileValue)
        {
            var multiplier = _hasDoublePoints ? 2 : 1;
            var finalValue = (uint)(projectileValue * multiplier);
            var currentScore = _stats.GetCurrentScore();
            bool isFullValue = GameParameters.GameplayValues.BaseMeteorValue == projectileValue;

            currentScore += finalValue;
            
            _stats.UpdateCurrentScore(currentScore);
            
            if (isFullValue)
            {
                _stats.IncreaseDeflectStreak();
            }
            
            if (projectileValue > 0)
            {
                NotifyAll(GameModeObserverMessage.PointsGained,position,finalValue,_hasDoublePoints);
            }
        }
        
        #endregion
        
        #region Projectile Spawn
        
        public void EnableProjectileSpawn()
        {
            NotifyAll(GameModeObserverMessage.SetEnableSpawnMeteor, true);
        }

        public void DisableProjectileSpawn()
        {
            NotifyAll(GameModeObserverMessage.SetEnableSpawnMeteor, false);
        }

        #endregion

        #region Countdown

        public void StartCountdown() 
            => NotifyAll(GameModeObserverMessage.StartCountdown,_startDelay);

        public void UpdateCountdown(float remainingTime) 
            => NotifyAll(GameModeObserverMessage.UpdateCountdown, remainingTime);

        public void FinishCountdown() 
            => NotifyAll(GameModeObserverMessage.FinishCountdown);

        #endregion
        
        #region Gameplay
        
        public void StartGameplay()
        {
            if (_isPaused)
                ResumeGame();
            else
                NotifyAll(GameModeObserverMessage.StartGameplay);

            EnableGameplayUI();
        }
        
        public void StopGameplay()
        {
            NotifyAll(GameModeObserverMessage.StopGameplay);
            DisableGameplayUI();
        }
        
        public void FinishGame() 
            => NotifyAll(GameModeObserverMessage.GameFinish);

        public void SetDoublePoints(bool isActive) 
            => _hasDoublePoints = isActive;

        #region UI

        public void DisableGameplayUI() 
            => NotifyAll(GameModeObserverMessage.DisableGameplayUI);

        public void EnableGameplayUI() 
            => NotifyAll(GameModeObserverMessage.EnableGameplayUI);

        public void TriggerFinishAddingPoints() 
            => NotifyAll(GameModeObserverMessage.FinishAddingPoints);

        #endregion

        #endregion

        #region Internal Level

        private void UpdateCurrentLevel()
        {
            NotifyAll(GameModeObserverMessage.UpdateGameLevel, _levelController.GetCurrentLevel());
        }

        #endregion
        
        public void NotifyAbilityActive(AbilityType abilityType) 
            => NotifyAll(GameModeObserverMessage.AbilityActive, abilityType);

        private void OnCheatDetectedHandler() 
            => NotifyAll(GameModeObserverMessage.CheatDetected);

        #region Stats

        public void IncreaseCollisionCount() => _stats.IncreaseCollisionCount();
        public void IncreaseAbilityUseCount() => _stats.IncreaseAbilityUseCount();
        public void IncreaseDeflectCount() => _stats.IncreaseDeflectCount();

        public void UpdateTimer(float deltaTime) => _stats.IncreaseTimer(deltaTime);
        
        private void OnStreakUpdated(uint streak)
        {
            _streakNotifier.UpdateStreak((int)streak);
            
            NotifyAll(GameModeObserverMessage.UpdateStreak, streak);
        }
        
        private void OnStreakMilestoneReached(int streak) 
            => NotifyAll(GameModeObserverMessage.NotifyStreak, streak);

        #endregion
        
        private void OnMinLevelReachedHandler()
        {
            NotifyAll(GameModeObserverMessage.MinLevelReached, true);
        }
    }
}