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
        private GeneratedId _collisionId;
        private GeneratedId _abilityUseId;
        private GeneratedId _deflectId;


        public GameModeMotor(int[] levelStreakAmount)
        {
            _levelController = new(levelStreakAmount);
            _levelController.OnLevelChange += UpdateCurrentLevel;

            _currentScoreId = SecureValueManager.RegisterValue<uint>(0);

            SecureValueManager.OnCheatDetected += OnCheatDetectedHandler;
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
            RestartValues();
            NotifyAll(GameModeObserverMessage.StartDisable);
        }
        
        public void ExecuteDisable()
        {
            NotifyAll(GameModeObserverMessage.ExecuteDisable);
        }

        public void TriggerEarthDestruction()
        {
            NotifyAll(GameModeObserverMessage.TriggerEarthDestruction);
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

        private float GetCollisionCount()
        {
            return SecureValueManager.GetDoesContainValue(_collisionId, out float value) ? value : 0;
        }
        private void UpdateCollisionCount(float input)
        {
            SecureValueManager.ModifyValue(_collisionId, input);
        }
        
        private float GetUsedAbilityCount()
        {
            return SecureValueManager.GetDoesContainValue(_abilityUseId, out float value) ? value : 0;
        }
        private void UpdateAbilityCount(float input)
        {
            SecureValueManager.ModifyValue(_abilityUseId, input);
        }
        
        private float GetDeflectCount()
        {
            return SecureValueManager.GetDoesContainValue(_deflectId, out float value) ? value : 0;
        }
        private void UpdateDeflectCount(float input)
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
                UpdateCollisionCount(Mathf.Infinity);
            }
            
            if (_abilityUseId.Id == id)
            {
                Debug.LogWarning("Cheat Detected! Restarting Ability Stats!");
                UpdateAbilityCount(Mathf.NegativeInfinity);
            }
            
            if (_deflectId.Id == id)
            {
                Debug.LogWarning("Cheat Detected! Restarting Ability Stats!");
                UpdateDeflectCount(Mathf.NegativeInfinity);
            }
        }
        
        #endregion

        public void StartFinish()
        {
            NotifyAll(GameModeObserverMessage.StartFinish);
            NotifyAll(GameModeObserverMessage.SaveStats, _collisionId,_abilityUseId,_deflectId);
        }

        public void TriggerPauseMenu()
        {
            NotifyAll(GameModeObserverMessage.OpenPauseMenu);
        }
    }
}