using _Main.Scripts.CustomId;
using _Main.Scripts.Observer;
using _Main.Scripts.SecurityData;
using UnityEngine;

namespace _Main.Scripts.GameMode
{
    public class GameModeMotor : ObservableComponent
    {
        // Scores Values
        private GeneratedId _currentScoreId;
        // Gameplay Values
        private readonly GameLevelController _levelController;
        private bool _hasDoublePoints;
        private readonly int _startDelay;
        private bool _canPause;
#pragma warning disable CS0414 // Field is assigned but its value is never used
        private bool _isPaused;
#pragma warning restore CS0414 // Field is assigned but its value is never used
        // Stats Values
        private readonly GeneratedId _collisionId;
        private readonly GeneratedId _abilityUseId;
        private readonly GeneratedId _deflectId;


        public GameModeMotor(int[] levelStreakAmount)
        {
            _levelController = new(levelStreakAmount);
            _levelController.OnLevelChange += UpdateCurrentLevel;

            _currentScoreId = SecureValueManager.RegisterValue<uint>(0);

            SecureValueManager.OnCheatDetected += OnCheatDetectedHandler;
            
            _collisionId = SecureValueManager.RegisterValue<uint>(0);
            _abilityUseId = SecureValueManager.RegisterValue<uint>(0);
            _deflectId = SecureValueManager.RegisterValue<uint>(0);
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
        
        public void UnPauseGame()
        {
            _isPaused = false;
            NotifyAll(GameModeObserverMessage.GameUnPaused);
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
            UpdateCurrentScore(0);
            UpdateCollisionCount(0);
            UpdateAbilityCount(0);
            UpdateDeflectCount(0);
            NotifyAll(GameModeObserverMessage.InitializeData);
        }

        public void SaveScore()
        {
            // The View receives this data and stores it in the GameManager
            // So the DefeatScreen can use it 
            NotifyAll(GameModeObserverMessage.SaveScore, _currentScoreId);
            NotifyAll(GameModeObserverMessage.SaveStats, _collisionId,_abilityUseId,_deflectId);
        }

        #endregion    
        
        #region Meteor 
        
        public void HandleMeteorDeflect(Vector2 position, byte projectileValue)
        {
            var finalValue = (uint)(_hasDoublePoints ? projectileValue * 2 : projectileValue);
            var currentScore = GetCurrentScore();

            currentScore += finalValue;
            
            UpdateCurrentScore(currentScore);
            
            if (projectileValue >= 1)
            {
                _levelController.IncreaseStreak();
                _levelController.CheckForNextLevel();
            }
            
            if (projectileValue > 0)
            {
                NotifyAll(GameModeObserverMessage.PointsGained,position,finalValue,_hasDoublePoints);
            }
        }
        
        #endregion
        
        #region Projectile Spawn

        public void GrantProjectileSpawn(int projectileTypeIndex)
        {
            NotifyAll(GameModeObserverMessage.GrantProjectileSpawn,projectileTypeIndex);
        }
        
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
        {
            NotifyAll(GameModeObserverMessage.StartCountdown,_startDelay);
        }
        
        public void UpdateCountdown(float remainingTime)
        {
            NotifyAll(GameModeObserverMessage.UpdateCountdown, remainingTime);
        }
        
        public void FinishCountdown()
        {
            NotifyAll(GameModeObserverMessage.FinishCountdown);
        }

        #endregion
        
        #region Gameplay
        
        public void StartGameplay()
        {
            NotifyAll(GameModeObserverMessage.StartGameplay);
            EnableGameplayUI();
        }
        
        public void StopGameplay()
        {
            NotifyAll(GameModeObserverMessage.StopGameplay);
            DisableGameplayUI();
        }
        
        public void FinishGame()
        {
            NotifyAll(GameModeObserverMessage.GameFinish);
        }

        public void SetDoublePoints(bool isActive)
        {
            _hasDoublePoints = isActive;
        }
        
        #region UI

        public void DisableGameplayUI()
        {
            NotifyAll(GameModeObserverMessage.DisableGameplayUI);
        }

        public void EnableGameplayUI()
        {
            NotifyAll(GameModeObserverMessage.EnableGameplayUI);
        }
        
        public void TriggerFinishAddingPoints()
        {
            NotifyAll(GameModeObserverMessage.FinishAddingPoints);
        }

        #endregion

        #endregion

        #region Stats

        public void IncreaseCollisionCount()
        {
            var current = GetCollisionCount();
            current++;
            UpdateCollisionCount(current);
        }

        public void IncreaseAbilityUseCount()
        {
            var current = GetUsedAbilityCount();
            current++;
            UpdateAbilityCount(current);
        }
        
        public void IncreaseDeflectCount()
        {
            var current = GetDeflectCount();
            current++;
            UpdateDeflectCount(current);
        }

        #endregion

        #region Internal Level

        private void UpdateCurrentLevel()
        {
            NotifyAll(GameModeObserverMessage.UpdateGameLevel, _levelController.GetCurrentLevel());
        }

        #endregion
        
        #region Secured Data

        #region Score

        private uint GetCurrentScore()
        {
            return SecureValueManager.GetDoesContainValue(_currentScoreId, out uint value) ? value : 0;
        }
        private void UpdateCurrentScore(uint input)
        {
            SecureValueManager.ModifyValue(_currentScoreId, input);
        }
        
        #endregion

        #region Stats

        private uint GetCollisionCount()
        {
            return SecureValueManager.GetDoesContainValue(_collisionId, out uint value) ? value : 0;
        }
        private void UpdateCollisionCount(uint input)
        {
            SecureValueManager.ModifyValue(_collisionId, input);
        }
        
        private uint GetUsedAbilityCount()
        {
            return SecureValueManager.GetDoesContainValue(_abilityUseId, out uint value) ? value : 0;
        }
        private void UpdateAbilityCount(uint input)
        {
            SecureValueManager.ModifyValue(_abilityUseId, input);
        }
        
        private uint GetDeflectCount()
        {
            return SecureValueManager.GetDoesContainValue(_deflectId, out uint value) ? value : 0;
        }
        private void UpdateDeflectCount(uint input)
        {
            SecureValueManager.ModifyValue(_deflectId, input);
        }


        #endregion
        
        private void OnCheatDetectedHandler(ushort id)
        {
            if (_currentScoreId.Id == id)
            {
                Debug.LogWarning("Cheat Detected! Restarting Points!");
                UpdateCurrentScore(0);
            }
            if (_collisionId.Id == id)
            {
                Debug.LogWarning("Cheat Detected! Restarting Collision Stats!");
                UpdateCollisionCount(0);
            }
            
            if (_abilityUseId.Id == id)
            {
                Debug.LogWarning("Cheat Detected! Restarting Ability Stats!");
                UpdateAbilityCount(0);
            }
            
            if (_deflectId.Id == id)
            {
                Debug.LogWarning("Cheat Detected! Restarting Ability Stats!");
                UpdateDeflectCount(0);
            }
        }
        
        #endregion

        
        public void NotifyAbilityActive(AbilityType abilityType)
        {
            NotifyAll(GameModeObserverMessage.AbilityActive, abilityType);
        }
    }
}