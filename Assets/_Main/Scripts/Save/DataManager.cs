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
        private const bool DoesDebug = false;

        private enum DebugMode
        {
            Default, 
            Warning,
            Error,
        }

        private static void SaveDebug(DebugMode debugMode, string debugData)
        {
            if(DoesDebug == false) return;
            
#pragma warning disable CS0162 // Unreachable code detected
            switch (debugMode)
            {
                case DebugMode.Default: Debug.Log(debugData); break;
                case DebugMode.Warning: Debug.LogWarning(debugData); break;
                case DebugMode.Error: Debug.LogError(debugData); break;
            }
#pragma warning restore CS0162 // Unreachable code detected

        }

        #region Private Clases

        [System.Serializable]
        private class MainSaveData
        {
            public StatsSaveData Stats = new(); 
            public SettingsSaveData Settings = new(); 
            public SkinSaveData Skin = new(); 
            public FlagsSaveData Flags = new(); 
        }
        private static class SaveSystem
        {
            private const string FolderName = "saves";
            private const string FileExtension = "sav";
            private const string BackupExtension = "bak";
            private const string FileName = "savedata";

            // ReSharper disable Unity.PerformanceAnalysis
            private static string GetSavePath()
            {
                var fileName = FileName.Replace("/", "_").Replace("\\", "_");
                return Path.Combine(GetSaveFolder(), $"{fileName}.{FileExtension}");
            }
            
            private static string GetBackupPath()
            {
                var fileName = FileName.Replace("/", "_").Replace("\\", "_");
                return Path.Combine(GetSaveFolder(), $"{fileName}.{BackupExtension}");
            }

            public static bool GetDoesSaveExist() => File.Exists(GetSavePath());
            public static bool GetDoesBackupExist() => File.Exists(GetBackupPath());

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
                string backupPath = GetBackupPath();
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
                    
                    File.Copy(path, backupPath, true);
                    
                    SaveDebug(DebugMode.Default,isNewSave ? $"Save File Created" : $"Game saved");
                }
                catch (Exception e)
                {
                    SaveDebug(DebugMode.Error,"Failed to save game: " + e.Message);
                }
            }
            public static void CreateSaveFile()
            {
                if (GetDoesSaveExist())
                {
                    SaveDebug(DebugMode.Default,"Save file already exists!");
                    return;
                }
                
                string path = GetSavePath();
                string backupPath = GetBackupPath();
                string json = JsonUtility.ToJson(new MainSaveData() , true);
                string encryptedJson  = SaveEncryption.Encrypt(json);
                byte checksum = ChecksumCalculator.CalculateXorChecksum(encryptedJson);
                string finalString = encryptedJson + "|" + checksum;
                string obfuscatedData = Obfuscator.XorObfuscate(finalString);
                
                try
                {
                    using (StreamWriter writer = new StreamWriter(path))
                    {
                        writer.Write(obfuscatedData);
                    }
                    
                    File.Copy(path, backupPath, true);
                    
                    SaveDebug(DebugMode.Default,$"Save File Created");
                    
                }
                catch (Exception e)
                {
                    SaveDebug(DebugMode.Error,"Failed to save game: " + e.Message);
                }
            }
            public static bool TryLoadSaveFileIfNotCorrupted(out MainSaveData saveData)
            {
                if (TryLoadFromPath(GetSavePath(), out saveData))
                    return true;

                SaveDebug(DebugMode.Warning,"Main save corrupted. Trying backup...");

                if (TryLoadFromPath(GetBackupPath(), out saveData))
                {
                    SaveDebug(DebugMode.Warning,"Backup save loaded successfully. Restoring main save.");
                    Save(saveData); // reescribimos el main save con backup
                    return true;
                }

                SaveDataEvents.TriggerOnSaveDataCorrupted();
                return false;
            }
            private static bool TryLoadFromPath(string path, out MainSaveData saveData)
            {
                saveData = null;
    
                if (!File.Exists(path))
                    return false;

                try
                {
                    string obfuscated = File.ReadAllText(path);
                    if (string.IsNullOrWhiteSpace(obfuscated))
                        return false;

                    string combined = Obfuscator.XorDeobfuscate(obfuscated);
                    int sepIndex = combined.LastIndexOf(ChecksumCalculator.Separator);
                    if (sepIndex <= 0 || sepIndex == combined.Length - 1)
                        return false;

                    string encrypted = combined.Substring(0, sepIndex);
                    string checksumStr = combined.Substring(sepIndex + 1);

                    if (!byte.TryParse(checksumStr, out byte checksumOriginal))
                        return false;

                    byte checksumCalc = ChecksumCalculator.CalculateXorChecksum(encrypted);
                    if (checksumOriginal != checksumCalc)
                        return false;

                    string json = SaveEncryption.Decrypt(encrypted);
                    saveData = JsonUtility.FromJson<MainSaveData>(json);
                    if (saveData == null)
                        return false;

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            public static void ClearSaveFile()
            {
                if (GetDoesSaveExist() == false)
                {
                    SaveDebug(DebugMode.Warning,$"No save file found at");
                    return;
                }
                
                string path = GetSavePath();
                string backupPath = GetBackupPath();
                File.Delete(path);
                File.Delete(backupPath);
                
                SaveDebug(DebugMode.Default,$"Save file cleared");
            }
        }
        private static class KeyEncryptor
        {
            private static readonly string[] keyParts =
            {
                "\u0045\u0031\u0038\u0032\u0033\u0037\u004D\u0054", // Parte 1 XOR
                "\u0042\u0036\u0039\u0034\u0053\u0034\u004C\u0033", // Parte 2 XOR
                "\u0051\u0038\u0055\u0046\u0032\u0035\u004B\u0037", // Parte 3 XOR
                "\u0044\u0031\u0039\u0047\u0038\u0040\u0035\u0050"  // Parte 4 XOR
            };

            private const char obfuscator = '\x55'; // XOR Key

            public static string GetEncryptionKey()
            {
                StringBuilder keyBuilder = new StringBuilder();
                foreach (var p in keyParts)
                {
                    foreach (char ch in p)
                        keyBuilder.Append((char)(ch ^ obfuscator)); // 🔓 Desofuscar
                }
                return keyBuilder.ToString();
            }
        }
        private static class SaveEncryption
        {
            public static string Encrypt(string plainText)
            {
                using Aes aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(KeyEncryptor.GetEncryptionKey());
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
                aes.Key = Encoding.UTF8.GetBytes(KeyEncryptor.GetEncryptionKey());

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
            Skin,
            Flags,
            Test
        }
        
        [System.Serializable]
        public abstract class SaveDataBase
        {
            public abstract SaveDataType Type { get;}
        }
        
    
        [System.Serializable]
        public class StatsSaveData : SaveDataBase
        {
            public override SaveDataType Type => SaveDataType.Stats;
            public uint HighScore;
            public uint DeflectAmount;
            public uint CollisionAmount;
            public uint AbilityUseAmount;
            public uint GamesPlayed;
            public float LongestTime;
            public uint LongestStreak;
            public uint TotalScore;
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

        [System.Serializable]
        public class FlagsSaveData : SaveDataBase
        {
            public override SaveDataType Type => SaveDataType.Flags;
            public bool HasPlayed;
            public bool HasCompletedTutorial;
            public bool HasOpenedCosmetics;
            public bool HasOpenedStats;
            public bool HasOpenedLore;
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
                        SaveDebug(DebugMode.Warning,"Save file was corrupted, clearing data and creating new");
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
            SaveDebug(DebugMode.Default,"Save data cleared");
            SaveSystem.ClearSaveFile();
        }

        private T GetDataByType<T>(SaveDataType saveType) where T : SaveDataBase
        {
            return saveType switch
            {
                SaveDataType.Settings => _mainSaveData.Settings as T,
                SaveDataType.Stats => _mainSaveData.Stats as T,
                SaveDataType.Skin => _mainSaveData.Skin as T,
                SaveDataType.Flags => _mainSaveData.Flags as T,
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
                case SaveDataType.Skin:
                    _mainSaveData.Skin = data as SkinSaveData;
                    break;
                case SaveDataType.Flags:
                    _mainSaveData.Flags = data as FlagsSaveData;
                    break;
            }
        }
        
    }
}