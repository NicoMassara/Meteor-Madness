using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MyComponents;
using _Main.Scripts.MyTools;
using _Main.Scripts.CustomId;
using _Main.Scripts.Save;
using _Main.Scripts.SecurityData;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        public bool CanPlay { get; set; }
        public bool IsPaused { get; private set; }

        public event Action OnPaused;
        public event Action OnResumed;
        public bool HadCorruptedSaveData { get; set; }

        public EventBusManager EventManager { get; private set; }
        public IInputReader InputReader { get; private set; }
        
        // Game Stored Data
        
        private GeneratedId _highScoreSecuredId;
        public uint VisualPoints { get;  set; }

        public bool AntiEpileptic { get; set; } = true;
        public GeneratedId CurrentScoreSecuredId { get; set; }
        public GeneratedId CollisionCountId { get; set; }
        public GeneratedId AbilityUseCountId { get; set; }
        public GeneratedId DeflectCountId { get; set; }

        private void Awake()
        {
            EventManager = new EventBusManager();
        }
        
        public void SetInputReader(IInputReader inputReader)
        {
            if(inputReader == null) return;
            
            InputReader = inputReader;
        }

        #region Screen Loading

        public void LoadTutorial()
        {
            LoadGameScreen(ScreenType.Tutorial);
        }

        public void LoadGameMode()
        {
            LoadGameScreen(ScreenType.GameMode);
        }

        public void LoadMainMenu()
        {
            LoadGameScreen(ScreenType.MainMenu);
        }
        
        public void LoadCosmeticMenu()
        {
            LoadGameScreen(ScreenType.Cosmetic);
        }
        
        public void LoadOptionsMenu()
        {
            LoadGameScreen(ScreenType.OptionsMenu);
        }
        
        public void LoadDefeatScreen()
        {
            LoadGameScreen(ScreenType.Defeat);
        }
        
        public void LoadPauseScreen()
        {
            LoadGameScreen(ScreenType.Pause);
        }

        public void LoadLastScreen()
        {
            GameScreenEventCaller.LoadLastScreen();
        }

        private void LoadGameScreen(ScreenType type)
        {
            GameScreenEventCaller.EnableScreen(type, EventRequestType.Requested);
        }

        #endregion

        public void PauseGame()
        {
            SetPauseChannels(true);
            IsPaused = true;
            OnPaused?.Invoke();
        }

        public void ResumeGame()
        {
            SetPauseChannels(false);
            IsPaused = false;
            OnResumed?.Invoke();
        }

        private void SetPauseChannels(bool isPaused)
        {
            CustomTime.SetChannelPaused(new []
            {
                UpdateGroup.Gameplay,
                UpdateGroup.Ability, 
                UpdateGroup.Shield,
                UpdateGroup.Effects,
                
            }, isPaused);
        }
        

        public void QuitGame()
        {
            QuitUtility.Quit();
        }

        #region Score

        public void ClearScoreData()
        {
            CurrentScoreSecuredId = null;
            CollisionCountId = null;
            AbilityUseCountId = null;
            DeflectCountId = null;
        }

        public bool GetHasNewHighScore()
        {
            if(SecureValueManager.GetDoesContainValue<uint>(CurrentScoreSecuredId, out var currentScore) == false) 
                return false;
            
            if(SecureValueManager.GetDoesContainValue<uint>(GetHighScoreSecuredId(), out var highScore) == false) 
                return true;
            
            return currentScore > highScore;
        }

        public bool GetHasPlayed()
        {
            var dataManager = DataManager.Instance;
            var saveData = dataManager.GetData<DataManager.StatsSaveData>(DataManager.SaveDataType.Stats);
            var value = saveData.HasPlayed;
            
            if(value)
                return true;
            
            saveData.HasPlayed = true;
            
#pragma warning disable CS0162 // Unreachable code detected
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (GameParameters.GameplayValues.DoesSaveProgress)
            {
                dataManager.SaveGameData(saveData, DataManager.SaveDataType.Stats);
            }
#else
            dataManager.SaveGameData(saveData, DataManager.SaveDataType.Stats);
#endif
#pragma warning restore CS0162 // Unreachable code detected
            
            return false;
        }

        public GeneratedId GetHighScoreSecuredId()
        {
            if (_highScoreSecuredId == null)
            {
                var temp = DataManager.Instance.GetData<DataManager.ScoreSaveData>(DataManager.SaveDataType.Score);
                _highScoreSecuredId = SecureValueManager.RegisterValue(temp.HighScore);
            }

            return _highScoreSecuredId;
        }

        public void SaveHighScore(GeneratedId currentScoreId)
        {
            if (currentScoreId == null)
            {
                Debug.LogWarning("Failed To Save High Score Data");
                return;
            }

            // Gets current Score
            if (SecureValueManager.GetDoesContainValue<uint>(CurrentScoreSecuredId, out var currentScore) == false)
            {
                Debug.LogWarning("Failed To Save High Score Data");
                return;
            }

            // Gets Saved High Score
            var dataManager = DataManager.Instance;
            var saveData = dataManager.GetData<DataManager.ScoreSaveData>(DataManager.SaveDataType.Score);
            
            // Overwrites the data
            saveData.HighScore = currentScore;
            dataManager.SaveGameData(saveData, DataManager.SaveDataType.Score);
        }

        public void SaveRuntimeHighScore(GeneratedId highScoreId, GeneratedId currentScoreId)
        {
            if (SecureValueManager.GetDoesContainValue<uint>(currentScoreId, out var currentScore) == false)
            {
                Debug.LogWarning("Failed To Save High Score Data");
                return;
            }
            
            SecureValueManager.ModifyValue(highScoreId,currentScore);
        }

        public void SaveStats()
        {
            var dataManager = DataManager.Instance;
            var saveData = dataManager.GetData<DataManager.StatsSaveData>(DataManager.SaveDataType.Stats);

            if (SecureValueManager.GetDoesContainValue<uint>(CollisionCountId, out var collisionCount))
            {
                saveData.CollisionAmount = collisionCount;
            }
            
            if (SecureValueManager.GetDoesContainValue<uint>(AbilityUseCountId, out var abilityCount))
            {
                saveData.AbilityUseAmount = abilityCount;
            }
            
            if (SecureValueManager.GetDoesContainValue<uint>(DeflectCountId, out var deflectCount))
            {
                saveData.DeflectAmount = deflectCount;
            }
            
            dataManager.SaveGameData(saveData, DataManager.SaveDataType.Stats);
        }

        #endregion
    }
}