using System;
using System.Collections;
using _Main.Scripts.MyComponents;
using System.IO;
using UnityEngine;

namespace _Main.Scripts.Save
{
    public class SaveDataEvents
    {
        public static event Action OnSaveInitialized;
        public static event Action OnSaveDataCorrupted;
        
        public static void TriggerOnSaveInitialized()
        {
            OnSaveInitialized?.Invoke();
        }

        public static void TriggerOnSaveDataCorrupted()
        {
            OnSaveDataCorrupted?.Invoke();
        }
    }
    
    public class DataManager : SingletonBehaviour<DataManager>
    {
        #region Private Clases

        [System.Serializable]
        private class MainSaveData
        {
            public ScoreSaveData Score = new(); 
            public StatsSaveData Stats = new(); 
            public SettingsSaveData Settings = new(); 
            public SkinSaveData Skin = new(); 
        }
        
        private static class SaveSystem
        {
            private const string FolderName = "saves";
            private const string FileExtension = "sav";
            private const string FileName = "saveData";

            // ReSharper disable Unity.PerformanceAnalysis
            private static string GetSavePath()
            {
                var fileName = FileName.Replace("/", "_").Replace("\\", "_");
                return Path.Combine(GetSaveFolder(), $"{fileName}.{FileExtension}");
            }

            public static bool GetDoesSaveExist() => File.Exists(GetSavePath());

            private static string GetSaveFolder()
            {
                string folderPath = Path.Combine(Application.persistentDataPath, FolderName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                
                return folderPath;
            }

            public static void Save(MainSaveData data)
            {
                string path = GetSavePath();
                string json = JsonUtility.ToJson(data, true);
                try
                {
                    bool isNewSave = GetDoesSaveExist() == false;
                    
                    using (StreamWriter writer = new StreamWriter(path))
                    {
                        writer.Write(json);
                    }

                    if (isNewSave)
                    {
                        Debug.Log($"Save File Created");
                    }
                    else
                    {
                        Debug.Log($"Game saved");
                    }


                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to save game: " + e.Message);
                }
            }

            public static void CreateSaveFile()
            {
                if (GetDoesSaveExist())
                {
                    Debug.Log("Save file already exists!");
                    return;
                }
                
                string json = JsonUtility.ToJson(new MainSaveData() , true);
                try
                {
                    using (StreamWriter writer = new StreamWriter(GetSavePath()))
                    {
                        writer.Write(json);
                    }
                    
                    Debug.Log($"Save File Created");
                    
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to save game: " + e.Message);
                }
            }
            

            public static bool TryLoadSaveFileIfNotCorrupted(out MainSaveData saveData)
            {
                saveData = null;
                
                string path = GetSavePath();
                
                try
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        string json = reader.ReadToEnd();

                        if (string.IsNullOrWhiteSpace(json))
                        {
                            SaveDataEvents.TriggerOnSaveDataCorrupted();
                            return false;
                        }

                        saveData = JsonUtility.FromJson<MainSaveData>(json);

                        if (saveData == null)
                        {
                            SaveDataEvents.TriggerOnSaveDataCorrupted();
                            return false;
                        }

                        return true;
                    }
                }
                catch (Exception e)
                {
                    SaveDataEvents.TriggerOnSaveDataCorrupted();
                    Debug.LogWarning($"Save file is empty or corrupted: " + e.Message);
                    return false;
                }
            }

            public static void ClearSaveFile()
            {
                if (GetDoesSaveExist())
                {
                    Save(new MainSaveData());
                    //Debug.Log($"Save File Cleared at: {path}");
                }
                else
                {
                    //Debug.LogWarning($"No save file found at: {path}");
                }
            }
        }

        #endregion

        #region Public Classes

        public enum SaveDataType
        {
            Settings,
            Stats,
            Score,
            Skin,
            Test
        }
        
        [System.Serializable]
        public abstract class SaveDataBase
        {
            public abstract SaveDataType Type { get;}
        
        }

        [System.Serializable]
        public class ScoreSaveData : SaveDataBase
        {
            public override SaveDataType Type => SaveDataType.Score;
            public float HighScore;
        }
    
        [System.Serializable]
        public class StatsSaveData : SaveDataBase
        {
            public override SaveDataType Type => SaveDataType.Stats;
            public int DeflectAmount;
            public int CollisionAmount;
        }
    
        [System.Serializable]
        public class SettingsSaveData : SaveDataBase
        {
            public override SaveDataType Type => SaveDataType.Settings;
            public int LanguageIndex = -1;
            public float MasterVolume = 1;
            public bool VibrationEnable = true;
        }
    
        [System.Serializable]
        public class SkinSaveData : SaveDataBase
        {
            public override SaveDataType Type => SaveDataType.Skin;
            public int SkinIndex = 0;
        }

        #endregion

        private MainSaveData _mainSaveData;
        private bool _hasLoaded = false;
        
        private void Awake()
        {
            InitializeSaves();
        }

        private void InitializeSaves()
        {
            StartCoroutine(LoadSaveFile());
        }

        private IEnumerator LoadSaveFile()
        {
            var currentTries = 0;
            var maxTries = 1000;
            
            while (_hasLoaded == false)
            {
                if (SaveSystem.GetDoesSaveExist())
                {
                    var isValid = SaveSystem.TryLoadSaveFileIfNotCorrupted(out _mainSaveData);

                    if (isValid)
                    {
                        _hasLoaded = true;
                    }
                    else
                    {
                        Debug.Log("Save file was corrupted, clearing data and creating new");
                        SaveSystem.ClearSaveFile();
                    }
                }
                else
                {
                    _mainSaveData = new MainSaveData();
                    SaveSystem.CreateSaveFile();
                    _hasLoaded = true;
                }
                
                currentTries++;
                yield return null;

                if (currentTries > maxTries && _hasLoaded == false)
                {
                    throw new Exception("Save file could not be loaded");
                }
                yield return null;
            }
            
            SaveDataEvents.TriggerOnSaveInitialized();
            
            yield return null;
        }

        public T GetData<T>(SaveDataType saveType) where T : SaveDataBase
        {
            return GetDataByType<T>(saveType);
        }
        
        public void SaveGameData<T>(T data, SaveDataType type) where T : SaveDataBase
        {
            SetDataByType(data, type);
            SaveSystem.Save(_mainSaveData);
        }
        
        public void ClearSaveData()
        {
            SaveSystem.ClearSaveFile();
        }

        private T GetDataByType<T>(SaveDataType saveType) where T : SaveDataBase
        {
            return saveType switch
            {
                SaveDataType.Settings => _mainSaveData.Settings as T,
                SaveDataType.Stats => _mainSaveData.Stats as T,
                SaveDataType.Score => _mainSaveData.Score as T,
                SaveDataType.Skin => _mainSaveData.Skin as T,
                _ => throw new ArgumentOutOfRangeException(nameof(saveType), saveType, null)
            };
        }
        
        private void SetDataByType<T>(T data, SaveDataType saveType) where T : SaveDataBase
        {
            switch (saveType)
            {
                case SaveDataType.Settings:
                    _mainSaveData.Settings = data as SettingsSaveData;
                    break;
                case SaveDataType.Stats:
                    _mainSaveData.Stats = data as StatsSaveData;
                    break;
                case SaveDataType.Score:
                    _mainSaveData.Score = data as ScoreSaveData;
                    break;
                case SaveDataType.Skin:
                    _mainSaveData.Skin = data as SkinSaveData;
                    break;
            }
        }
        
    }
}