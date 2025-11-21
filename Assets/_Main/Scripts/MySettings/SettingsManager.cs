using System;
using _Main.Scripts.MyComponents;
using _Main.Scripts.Save;
using UnityEngine;

namespace _Main.Scripts.MySettings
{
    public class SettingsManager : SingletonBehaviour<SettingsManager>
    {
        private SettingsSaveData _settingsData;
        
        public event Action<int> OnLanguageChanged;
        public event Action<float> OnMasterVolumeChanged;
        public event Action<bool> OnVibrationChanged;

        private void Start()
        {
            _settingsData = DataManager.Instance.GetData<SettingsSaveData>(SaveDataType.Settings);
        }

        #region Settings Actions

        
        public void SetLanguageIndex(int index)
        {
            _settingsData.LanguageIndex = index;
            OnLanguageChanged?.Invoke(_settingsData.LanguageIndex);
        }

        public void SetMasterVolume(float volume)
        {
            _settingsData.MasterVolume = volume;
            OnMasterVolumeChanged?.Invoke(volume);
        }

        public void SetVibration(bool enable)
        {
            _settingsData.VibrationEnable = enable;
            OnVibrationChanged?.Invoke(enable);
        }

        public void SaveSettings()
        {
            DataManager.Instance.SaveGameData(_settingsData, SaveDataType.Settings);
        }

        #endregion

        #region Data Getters
        
        public bool HasLoadedData()
        {
            return _settingsData != null;
        }

        public int GetLanguageIndex()
        {
            return _settingsData?.LanguageIndex ?? -1; 
        }

        public float GetMasterVolume()
        {
            return _settingsData?.MasterVolume ?? 1; 
        }

        public bool GetVibration()
        {
            return _settingsData?.VibrationEnable ?? true; 
        }

        #endregion

    }
}