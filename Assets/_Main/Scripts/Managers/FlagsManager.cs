using System;
using MeteorMadness.Contracts.Events;
using MeteorMadness.GlobalValues.BaseSingleton;
using MeteorMadness.GlobalValues.Tools;
using MeteorMadness.Managers.Save;
using UnityEngine;

namespace MeteorMadness.Managers
{
    public class FlagsManager : SingletonBehaviour<FlagsManager>
    {
        /// <summary>
        /// 0 - HasPlayed | |
        /// 1 - HasCompletedTutorial | |
        /// 2 - HasOpenedCosmetics | |
        /// 3 - HasOpenedStats | |
        /// 4 - Has OpenedLore | |
        /// 5 - Empty | |
        /// 6 - Empty | |
        /// 7 - Empty 
        /// </summary>
        private byte _firstTimeFlags;
        
        private void Awake()
        {
            BootEvents.OnMainSystemRequestInitialize += Initialize;
        }

        private void Initialize()
        {
            BootEvents.OnMainSystemRequestInitialize -= Initialize;
            //
            
            var flagsData = DataManager.Instance.GetData<DataManager.FlagsSaveData>(DataManager.SaveDataType.Flags);
            
            if (flagsData != null)
            {
                _firstTimeFlags = flagsData.FirstTimeFlags;
            }
            else
            {
                Debug.Log("Flags saved data was not found!");
            }
            
            
            //
            BootEvents.MainSystemInitialized();
        }
        
        #region Tools

        private bool GetIsTrue(int index) 
            => BitwiseTool.Read(_firstTimeFlags, index);
        private void SetTrue(int index) 
            => _firstTimeFlags = BitwiseTool.Set(_firstTimeFlags, index);

        #endregion
        
        #region Public

        #region Getters
        
        public static bool GetHasPlayed() => Instance.Internal_GetHasPlayed();
        public static bool GetHasCompletedTutorial() => Instance.Internal_GetHasCompletedTutorial();
        public static bool GetHasOpenedCosmetics() => Instance.Internal_GetHasOpenedCosmetics();
        public static bool GetHasOpenedStats() => Instance.Internal_GeHasOpenedStats();
        public static bool GetHasOpenedLore() => Instance.Internal_GetHasOpenedLore();
        
        #endregion
        
        #region Setters
        
        public static void SetHasPlayed() => Instance.Internal_SetHasPlayed();
        public static void SetHasCompletedTutorial() => Instance.Internal_SetHasCompletedTutorial();
        public static void SetHasOpenedCosmetics() => Instance.Internal_SetHasOpenedCosmetics();
        public static void SetHasOpenedStats() => Instance.Internal_SetHasOpenedStats();
        public static void SetHasOpenedLore() => Instance.Internal_SetHasOpenedLore();
        
        #endregion
        
        public static void SaveFlags() => Instance.Internal_SaveFlags();

        #endregion

        #region Internal

        #region Getters
        private bool Internal_GetHasPlayed() => GetIsTrue( 0);
        private bool Internal_GetHasCompletedTutorial() => GetIsTrue( 1);
        private bool Internal_GetHasOpenedCosmetics() => GetIsTrue( 2);
        private bool Internal_GeHasOpenedStats() => GetIsTrue( 3);
        private bool Internal_GetHasOpenedLore() => GetIsTrue( 4);
        
        #endregion

        #region Setters

        private void Internal_SetHasPlayed() => SetTrue( 0);
        private void Internal_SetHasCompletedTutorial() => SetTrue( 1);
        private void Internal_SetHasOpenedCosmetics() => SetTrue( 2);
        private void Internal_SetHasOpenedStats() => SetTrue( 3);
        private void Internal_SetHasOpenedLore() => SetTrue( 4);

        #endregion

        private void Internal_SaveFlags()
        {
            var flagsData = DataManager.Instance.GetData<DataManager.FlagsSaveData>(DataManager.SaveDataType.Flags);
            
            flagsData.FirstTimeFlags = _firstTimeFlags;
            
            DataManager.Instance.SaveGameData(flagsData, DataManager.SaveDataType.Flags);
        }

        #endregion
    }
}