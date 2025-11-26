using System;
using System.IO;
using UnityEngine;

namespace _Main.Scripts.Save
{
    public struct SaveParameters
    {
        public const string FolderName = "saves";
        public const string FileExtension = "sav";
        //
        public const string SettingsFileName = "settings";
        public const string ScoreFileName = "score";
        public const string StatsFileName = "stats";
        public const string SkinFileName = "skin";
        public const string TestFileName = "test";
    }

    public static class SaveSystem
    {
        private static string GetSavePath(string fileName)
        {
            fileName = fileName.Replace("/", "_").Replace("\\", "_");
            return Path.Combine(GetSaveFolder(), $"{fileName}.{SaveParameters.FileExtension}");
        }

        private static string GetSaveFolder()
        {
            string folderPath = Path.Combine(Application.persistentDataPath, SaveParameters.FolderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Debug.Log($"Save Folder Created At: {folderPath}");
            }
            
            return folderPath;
        }

        private static string GetFileName(SaveDataType saveType)
        {
            string fileName = saveType switch
            {
                SaveDataType.Settings => SaveParameters.SettingsFileName,
                SaveDataType.Stats => SaveParameters.StatsFileName,
                SaveDataType.Score => SaveParameters.ScoreFileName,
                SaveDataType.Skin => SaveParameters.SkinFileName,
                SaveDataType.Test => SaveParameters.TestFileName,
                _ => throw new ArgumentOutOfRangeException(nameof(saveType), saveType, null)
            };
            
            return fileName;
        }

        public static void Save<T>(T data, SaveDataType saveType) where T : SaveDataBase
        {
            string path = GetSavePath(GetFileName(saveType));
            string json = JsonUtility.ToJson(data, true);
            try
            {
                bool isNewSave = !File.Exists(path);
                
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(json);
                }

                if (isNewSave)
                {
                    Debug.Log($"Save File Created at: {path}");
                }
                else
                {
                    Debug.Log($"Game saved at: {path}");
                }


            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save game: " + e.Message);
            }
        }
        

        public static bool LoadSaveFile<T>(SaveDataType saveType, out T saveData) where T : SaveDataBase
        {
            saveData = null;
            
            string path = GetSavePath(GetFileName(saveType));
            
            if (!File.Exists(path))
            {
                Debug.LogWarning($"No save file found at: {path}");
                return false;
            }

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string json = reader.ReadToEnd();
                    saveData = JsonUtility.FromJson<T>(json);
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load game: " + e.Message);
                return false;
            }
        }

        public static void ClearSaveFile<T>(SaveDataType saveType) where T : SaveDataBase, new()
        {
            string path = GetSavePath(GetFileName(saveType));
            if (File.Exists(path))
            {
                Save<T>(new T(),saveType);
                Debug.Log($"Save File Cleared at: {path}");
            }
            else
            {
                Debug.LogWarning($"No save file found at: {path}");
            }
        }
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
    
    

    public enum SaveDataType
    {
        Settings,
        Stats,
        Score,
        Skin,
        Test
    }
}