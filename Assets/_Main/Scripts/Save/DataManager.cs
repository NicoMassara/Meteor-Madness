using System;
using System.Collections.Generic;
using _Main.Scripts.MyComponents;
using UnityEngine;

namespace _Main.Scripts.Save
{
    public class DataManager : SingletonBehaviour<DataManager>
    {
        public static DataManager Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        private static DataManager _instance;
        private Dictionary<SaveDataType, SaveDataBase> _saveDataDic;

        public event Action OnSaveInitialized;
        
        private static DataManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(DataManager))
            {
                hideFlags = HideFlags.DontSave,
            };
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<DataManager>();
        }
        
        private void Start()
        {
            InitializeSaves();
        }

        private void InitializeSaves()
        {
            _saveDataDic = new Dictionary<SaveDataType, SaveDataBase>();
            TryLoadSaveData<ScoreSaveData>(SaveDataType.Score);
            TryLoadSaveData<StatsSaveData>(SaveDataType.Stats);
            TryLoadSaveData<SettingsSaveData>(SaveDataType.Settings);
            TryLoadSaveData<SettingsSaveData>(SaveDataType.Test);
            
            SaveDataEvents.TriggerOnSaveInitialized();
        }

        private void TryLoadSaveData<T>(SaveDataType type) where T : SaveDataBase, new()
        {
            if (SaveSystem.LoadSaveFile<T>(type, out var saveData))
            {
                _saveDataDic[type] = saveData;
            }
            else
            {
                SaveSystem.Save<T>(new T(), type);
                TryLoadSaveData<T>(type);
            }
        }

        public void ClearSaveData<T>(SaveDataType type) where T : SaveDataBase, new()
        {
            if (_saveDataDic.ContainsKey(type))
            {
                _saveDataDic[type] = new T();
                SaveSystem.ClearSaveFile<T>(type);
            }
        }

        public void SaveGameData<T>(T data, SaveDataType type) where T : SaveDataBase
        {
            if (_saveDataDic.ContainsKey(type))
            {
                _saveDataDic[type] = data;
                SaveSystem.Save<T>(data, type);
            }
        }

        public T GetData<T>(SaveDataType saveType) where T : SaveDataBase
        {
            return _saveDataDic[saveType] as T;
        }
    }
}