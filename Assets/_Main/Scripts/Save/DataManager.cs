using System;
using System.Collections;
using _Main.Scripts.MyComponents;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
                string encryptedJson  = SaveEncryption.Encrypt(json);
                byte checksum = ChecksumCalculator.CalculateXorChecksum(encryptedJson);
                string finalString = encryptedJson + "|" + checksum;
                string obfuscatedData = Obfuscator.XorObfuscate(finalString);
                
                try
                {
                    bool isNewSave = GetDoesSaveExist() == false;
                    
                    using (StreamWriter writer = new StreamWriter(path))
                    {
                        writer.Write(obfuscatedData);
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
                string encryptedJson  = SaveEncryption.Encrypt(json);
                byte checksum = ChecksumCalculator.CalculateXorChecksum(encryptedJson);
                string finalString = encryptedJson + "|" + checksum;
                string obfuscatedData = Obfuscator.XorObfuscate(finalString);
                
                try
                {
                    using (StreamWriter writer = new StreamWriter(GetSavePath()))
                    {
                        writer.Write(obfuscatedData);
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
                        string obfuscated = reader.ReadToEnd();
                        
                        if (string.IsNullOrWhiteSpace(obfuscated))
                        {
                            SaveDataEvents.TriggerOnSaveDataCorrupted();
                            return false;
                        }

                        string combined;
                        try
                        {
                            combined = Obfuscator.XorDeobfuscate(obfuscated);
                        }
                        catch (FormatException)
                        {
                            Debug.LogWarning("Save file not base64 / malformed");
                            SaveDataEvents.TriggerOnSaveDataCorrupted();
                            return false;
                        }
                        
                        int sepIndex = combined.LastIndexOf(ChecksumCalculator.Separator);
                        if (sepIndex <= 0 || sepIndex == combined.Length - 1)
                        {
                            Debug.LogWarning("Save format invalid (separator issue)");
                            SaveDataEvents.TriggerOnSaveDataCorrupted();
                            return false;
                        }
                        
                        string encrypted = combined.Substring(0, sepIndex);
                        string checksumStr = combined.Substring(sepIndex + 1);
                        
                        // 3) Parseo seguro del checksum
                        if (!byte.TryParse(checksumStr, out byte checksumOriginal))
                        {
                            Debug.LogWarning("Save checksum parse failed");
                            SaveDataEvents.TriggerOnSaveDataCorrupted();
                            return false;
                        }
                        
                        // 4) Recalcular checksum y comparar
                        byte checksumCalc = ChecksumCalculator.CalculateXorChecksum(encrypted);
                        if (checksumOriginal != checksumCalc)
                        {
                            Debug.LogWarning("Save file has been modified or corrupted (checksum mismatch)");
                            SaveDataEvents.TriggerOnSaveDataCorrupted();
                            return false;
                        }
                        
                        string json;
                        try
                        {
                            json = SaveEncryption.Decrypt(encrypted);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning("Decrypt failed: " + e.Message);
                            SaveDataEvents.TriggerOnSaveDataCorrupted();
                            return false;
                        }
                        
                        saveData = JsonUtility.FromJson<MainSaveData>(json);
                        if (saveData == null)
                        {
                            Debug.LogWarning("Deserialized saveData is null");
                            SaveDataEvents.TriggerOnSaveDataCorrupted();
                            return false;
                        }
                        return true;
                    }
                }
                catch (FileNotFoundException)
                {
                    Debug.LogWarning("Save file not found");
                    return false;
                }
                catch (Exception e)
                {
                    SaveDataEvents.TriggerOnSaveDataCorrupted();
                    Debug.LogWarning("Save file corrupted: " + e.Message);
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
        private static class SaveEncryption
        {
            private static readonly string EncryptionKey = "G7d$k9V2pL#8sQ1rT6wZ4mN5xY0bC3@!";
            public static string Encrypt(string plainText)
            {
                using Aes aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.GenerateIV();
                byte[] iv = aes.IV;

                using var encryptor = aes.CreateEncryptor(aes.Key, iv);
                using var ms = new MemoryStream();
                ms.Write(iv, 0, iv.Length);
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
            public static string Decrypt(string encryptedText)
            {
                byte[] buffer = Convert.FromBase64String(encryptedText);
                using Aes aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);

                byte[] iv = new byte[aes.BlockSize / 8];
                Array.Copy(buffer, iv, iv.Length);

                using var decryptor = aes.CreateDecryptor(aes.Key, iv);
                using var ms = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
        }
        private static class Obfuscator
        {
            public static string XorObfuscate(string input, byte key = 0x5A)
            {
                byte[] data = Encoding.UTF8.GetBytes(input);
                for (int i = 0; i < data.Length; i++)
                    data[i] ^= key;
                return Convert.ToBase64String(data);
            }

            public static string XorDeobfuscate(string input, byte key = 0x5A)
            {
                byte[] data = Convert.FromBase64String(input);
                for (int i = 0; i < data.Length; i++)
                    data[i] ^= key;
                return Encoding.UTF8.GetString(data);
            }
        }

        private static class ChecksumCalculator
        {
            public const char Separator = '|';
            
            
            public static byte CalculateXorChecksum(string data)
            {
                byte checksum = 0;
                foreach (char c in data)
                {
                    checksum ^= (byte)c;
                }
                return checksum;
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