using System;
using _Main.Scripts.Localization;
using _Main.Scripts.MyComponents;
using _Main.Scripts.Save;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;

namespace _Main.Scripts.MySettings
{
    public class SettingsManager : SingletonBehaviour<SettingsManager>
    {
        private DataManager.SettingsSaveData _settingsData;

        private bool _hasChanged;
        
        public event Action<int> OnLanguageChanged;
        public event Action<float> OnMasterVolumeChanged;
        public event Action<bool> OnVibrationChanged;

        private void Awake()
        {
            SaveDataEvents.OnSaveInitialized += Initialize;
        }

        private void Initialize()
        {
            SaveDataEvents.OnSaveInitialized -= Initialize;
            //
            _settingsData = DataManager.Instance.GetData<DataManager.SettingsSaveData>(DataManager.SaveDataType.Settings);
        }

        #region Settings Actions

        
        public void SetLanguageIndex(int index)
        {
            _settingsData.LanguageIndex = Math.Clamp(index, 0, LocalizationTools.LanguageCount);
            OnLanguageChanged?.Invoke(_settingsData.LanguageIndex);
            _hasChanged = true;
        }

        public void SetMasterVolume(float volume)
        {
            _settingsData.MasterVolume = volume;
            OnMasterVolumeChanged?.Invoke(volume);
            AudioMixerTools.SetMixerChannelVolume(MixerChannels.Master, volume);
            _hasChanged = true;
        }

        public void SetVibration(bool enable)
        {
            _settingsData.VibrationEnable = enable;
            OnVibrationChanged?.Invoke(enable);
            _hasChanged = true;
        }

        public void SaveSettings()
        {
            if (_hasChanged)
            {
                DataManager.Instance.SaveGameData(_settingsData, DataManager.SaveDataType.Settings);
            }
            
            _hasChanged = false;
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