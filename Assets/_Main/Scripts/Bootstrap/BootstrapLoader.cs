using System;
using System.Collections;
using _Main.Scripts.Cosmetics;
using _Main.Scripts.Localization;
using _Main.Scripts.Managers;
using _Main.Scripts.Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Main.Scripts.Bootstrap
{
    public interface IBoostrap
    {
        public event Action<string> OnLoadingAsset;
    }

    public class BootstrapLoader : MonoBehaviour, IBoostrap
    {
        [SerializeField] private string coreScene = "MainMenu"; // o el nombre de tu primera escena real
        [SerializeField] private string[] additiveScenes;
        [SerializeField] private float delayBeforeLoad = 0.1f;    // opcional, da tiempo al splash
        [SerializeField] private Image progressBar;
        
        public const bool DebugDisabled = true;
        private bool _hasLocalizationLoaded;
        private bool _hasLoadedData;
        private bool _hasLoadedSkins;
        
        public event Action<string> OnLoadingAsset;
        
        private void Awake()
        {
            LocalizationEvents.OnLocalizationLoaded += () =>
            {
                _hasLocalizationLoaded = true;
            };
            
            SaveDataEvents.OnSaveInitialized += () =>
            {
                _hasLoadedData = true;
            };
            
            SaveDataEvents.OnSaveDataCorrupted += () =>
            {
                GameManager.Instance.HadCorruptedSaveData = true;
            };
            
            SkinEvents.OnAssetsLoaded += () =>
            {
                _hasLoadedSkins = true;
            };
            
            
            var localization = LocalizationManager.Instance;
            var dataManager = DataManager.Instance;
            var skinManager = SkinManager.Instance;
        }

        private void Start()
        {
            StartCoroutine(LoadCoreScene());
        }

        private IEnumerator LoadCoreScene()
        {
            var boostrapScene = SceneManager.GetActiveScene();
            
            yield return new WaitForSeconds(delayBeforeLoad);
            
            // Save Data
            OnLoadingAsset?.Invoke("Loading Saves");
            yield return new WaitUntil(()=> _hasLoadedData);
            PrintDebug("Save Data loaded");
            
            // Localization
            OnLoadingAsset?.Invoke("Loading Texts");
            yield return new WaitUntil(()=> _hasLocalizationLoaded);
            PrintDebug("Localization loaded");
            
            // Skins
            OnLoadingAsset?.Invoke("Loading Skins");
            yield return new WaitUntil(()=> _hasLoadedSkins);
            PrintDebug("Skins loaded");
            
            yield return new WaitForSeconds(delayBeforeLoad);
            
            OnLoadingAsset?.Invoke("Loading Scenes");
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(coreScene, LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = false;
            
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }
            
            PrintDebug("Core Module Loaded");
            
            yield return new WaitForSeconds(delayBeforeLoad);
            
            asyncLoad.allowSceneActivation = true;
            
            PrintDebug("Activating Core Module");

            yield return null;
            
            yield return new WaitUntil(() => asyncLoad.isDone);
            
            PrintDebug("Game Loaded");
            
            OnLoadingAsset?.Invoke("Game Loaded");
            
            yield return new WaitForEndOfFrame();
            
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(coreScene));
            GameEvents.TriggerOnGameLoaded();

            yield return new WaitForEndOfFrame();
            
            PrintDebug("Removing Boostrap");

            SceneManager.UnloadSceneAsync(boostrapScene);
        }

        private void PrintDebug(string message)
        {
            if(DebugDisabled) return;
            
#pragma warning disable CS0162 // Unreachable code detected
            Debug.Log($"[Boostrap] - {message}");
#pragma warning restore CS0162 // Unreachable code detected
        }
    }
}