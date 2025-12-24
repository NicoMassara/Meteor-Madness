using _Main.Scripts.CustomId;
using _Main.Scripts.Save;
using _Main.Scripts.SecurityData;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class StatsController
    {
        public uint VisualPoints { get;  set; }

        private GeneratedId _highScoreSecuredId;
        private DataManagerTools.GameplayStatsIdData _gameplayStatsIdData;
        
        #region Score

        public bool GetHasNewHighScore()
        {
            if(GetIsValueNull(_gameplayStatsIdData.CurrentScoreId, out var currentScore)) return false;
            if(GetIsValueNull(GetHighScoreSecuredId(), out var highScore)) return false;
            
            return currentScore > highScore;
        }

        public GeneratedId GetHighScoreSecuredId()
        {
            if (_highScoreSecuredId == null)
            {
                var temp = DataManager.Instance.GetData<DataManager.StatsSaveData>(DataManager.SaveDataType.Stats);
                _highScoreSecuredId = SecureValueManager.RegisterValue(temp.HighScore);
            }

            return _highScoreSecuredId;
        }
        
        public GeneratedId GetCurrentScoreSecuredId() => _gameplayStatsIdData.CurrentScoreId;

        public void SaveHighScore(GeneratedId currentScoreId)
        {
            if (currentScoreId == null)
            {
                Debug.LogWarning("Failed To Save High Score Data");
                return;
            }
            
            if (GetIsValueNull(currentScoreId, out var currentScore))
            {
                Debug.LogWarning("Failed To Save High Score Data");
                return;
            }

            if (DataManagerTools.GetIsSaveEnabled())
            {
                // Gets Saved High Score
                var dataManager = DataManager.Instance;
                var saveData = dataManager.GetData<DataManager.StatsSaveData>(DataManager.SaveDataType.Stats);
                
                // Overwrites the data
                saveData.HighScore = currentScore;
                dataManager.SaveGameData(saveData, DataManager.SaveDataType.Stats);
                
                
                Debug.Log($"Saved High Score Data: {currentScore}");
            }
        }

        public void SaveRuntimeHighScore(GeneratedId highScoreId, GeneratedId currentScoreId)
        {
            if (GetIsValueNull(currentScoreId, out var currentScore))
            {
                Debug.LogWarning("Failed To Save High Score Data");
                return;
            }
            
            SecureValueManager.ModifyValue(highScoreId,currentScore);
        }

        #endregion
        
        public void SaveStats()
        {
            var dataManager = DataManager.Instance;
            var saveData = dataManager.GetData<DataManager.StatsSaveData>(DataManager.SaveDataType.Stats);
            
            saveData.GamesPlayed++;
            AddSingleStat(_gameplayStatsIdData.CurrentScoreId, ref saveData.TotalScore);
            AddSingleStat(_gameplayStatsIdData.CollisionId, ref saveData.CollisionAmount);
            AddSingleStat(_gameplayStatsIdData.AbilityUseId, ref saveData.AbilityUseAmount);
            AddSingleStat(_gameplayStatsIdData.DeflectId, ref saveData.DeflectAmount);
            SaveSingleStat(_gameplayStatsIdData.StreakId, ref saveData.LongestStreak, true);
            SaveTimeStats(_gameplayStatsIdData.TimeId, ref saveData.LongestTime);
            
            if (DataManagerTools.GetIsSaveEnabled())
            {
                dataManager.SaveGameData(saveData, DataManager.SaveDataType.Stats);
            }
        }

        private void SaveTimeStats(GeneratedId valueId, ref float storedValue)
        {
            if (SecureValueManager.GetDoesContainValue<float>(valueId, out float securedValue))
            {
                if (securedValue > storedValue)
                {
                    storedValue = securedValue;
                }
            }
        }

        private void SaveSingleStat(GeneratedId valueId, ref uint storedValue, bool isRecord = false)
        {
            if (SecureValueManager.GetDoesContainValue<uint>(valueId, out var securedValue))
            {
                if (isRecord && securedValue > storedValue)
                {
                    storedValue = securedValue;
                }
                else
                {
                    storedValue = securedValue;
                }
            }
        }

        private void AddSingleStat(GeneratedId valueId, ref uint storedValue)
        {
            if (SecureValueManager.GetDoesContainValue<uint>(valueId, out var securedValue))
            {
                storedValue += securedValue;
            }
        }

        private bool GetIsValueNull(GeneratedId valueId, out uint storedValue)
        {
            return !SecureValueManager.GetDoesContainValue<uint>(valueId, out storedValue);
        }

        public void ClearScoreData()
        {
            _gameplayStatsIdData = null;
        }

        public void SetStatsIdData(DataManagerTools.GameplayStatsIdData statsId)
        {
            _gameplayStatsIdData = statsId;
        }
    }
}