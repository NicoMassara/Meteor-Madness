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
        public const string SettingsFile = "settings";
        public const string SaveFile = "data";
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

        public static void Save<T>(T data, string fileName = "savefile") where T : SaveDataBase
        {
            string path = GetSavePath(fileName);
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
        

        public static T LoadSaveFile<T>(string fileName) where T : SaveDataBase
        {
            string path = GetSavePath(fileName);
            
            if (!File.Exists(path))
            {
                Debug.LogWarning($"No save file found at: {path}");
                return null;
            }

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string json = reader.ReadToEnd();
                    T data = JsonUtility.FromJson<T>(json);
                    return data;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load game: " + e.Message);
                return null;
            }
        }

        public static void DeleteSaveFile(string fileName)
        {
            string path = GetSavePath(fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"Save File Deleted at: {path}");
            }
            else
            {
                Debug.LogWarning($"No save file found at: {path}");
            }
        }

        public static void DeleteAllSaves()
        {
            string folder = GetSaveFolder();

            if (Directory.Exists(folder))
            {
                try
                {
                    string[] files = Directory.GetFiles(folder);
                    foreach (var file in files)
                    {
                        File.Delete(file);
                        Debug.Log($"Delete save at: {file}");
                    }
                    Debug.Log($"All saves deleted");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to delete all saves: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"Save Folder Not Found at: {folder}");
            }
        }

    }

    [System.Serializable]
    public abstract class SaveDataBase {}
}