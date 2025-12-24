using _Main.Scripts.Save;

namespace _Main.Scripts.Managers
{
    public class FlagsController
    {
        public bool GetHasPlayed()
        {
            var dataManager = DataManager.Instance;
            var saveData = dataManager.GetData<DataManager.FlagsSaveData>(DataManager.SaveDataType.Flags);
            return saveData.HasPlayed;
        }

        public void FlipHasPlayed()
        {
            var dataManager = DataManager.Instance;
            var saveData = dataManager.GetData<DataManager.FlagsSaveData>(DataManager.SaveDataType.Flags);
            
            if (saveData.HasPlayed) return;
            
            if (DataManagerTools.GetIsSaveEnabled())
            {
                saveData.HasPlayed = true;
                dataManager.SaveGameData(saveData, DataManager.SaveDataType.Flags);
            }
        }

        public bool GetHasCompletedTutorial()
        {
            var dataManager = DataManager.Instance;
            var saveData = dataManager.GetData<DataManager.FlagsSaveData>(DataManager.SaveDataType.Flags);
            
            return saveData.HasCompletedTutorial;
        }
        public void SetHasCompletedTutorial()
        {
            var dataManager = DataManager.Instance;
            var saveData = dataManager.GetData<DataManager.FlagsSaveData>(DataManager.SaveDataType.Flags);
            saveData.HasCompletedTutorial = true;
            
            if (DataManagerTools.GetIsSaveEnabled()) dataManager.SaveGameData(saveData, DataManager.SaveDataType.Flags);
            
        }
        public bool GetHasOpenedCosmetics()
        {
            var dataManager = DataManager.Instance;
            var saveData = dataManager.GetData<DataManager.FlagsSaveData>(DataManager.SaveDataType.Flags);
            var value = saveData.HasOpenedCosmetics;
            
            if(value)
                return true;
            
            saveData.HasOpenedCosmetics = true;
            
            if (DataManagerTools.GetIsSaveEnabled()) dataManager.SaveGameData(saveData, DataManager.SaveDataType.Flags);
            
            return false;
        }
        public bool GetHasOpenedLore()
        {
            var dataManager = DataManager.Instance;
            var saveData = dataManager.GetData<DataManager.FlagsSaveData>(DataManager.SaveDataType.Flags);
            var value = saveData.HasOpenedLore;
            
            if(value)
                return true;
            
            saveData.HasOpenedLore = true;
            
            if (DataManagerTools.GetIsSaveEnabled()) dataManager.SaveGameData(saveData, DataManager.SaveDataType.Flags);
            
            return false;
        }
        public bool GetHasOpenedStats()
        {
            var dataManager = DataManager.Instance;
            var saveData = dataManager.GetData<DataManager.FlagsSaveData>(DataManager.SaveDataType.Flags);
            var value = saveData.HasOpenedStats;
            
            if(value)
                return true;
            
            saveData.HasOpenedStats = true;
            
            if (DataManagerTools.GetIsSaveEnabled()) dataManager.SaveGameData(saveData, DataManager.SaveDataType.Flags);
            
            return false;
        }
    }
}