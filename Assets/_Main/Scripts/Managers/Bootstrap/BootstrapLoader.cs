using System;
using System.Collections;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.GlobalValues.Events;
using MeteorMadness.Managers.Cosmetics;
using MeteorMadness.Managers.GameConfig;
using MeteorMadness.Managers.Localization;
using MeteorMadness.Managers.Save;
using MeteorMadness.Services.AdsSystem;
using MeteorMadness.Services.MyAnalytics;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MeteorMadness.Managers.Boostrap
{
    public interface IBoostrap
    {
        public event Action OnStartLoading;
        public event Action<string> OnLoadingAsset;
    }

    public class BootstrapLoader : MonoBehaviour, IBoostrap
    {
        [SerializeField] private string coreScene = "MainMenu";
        [SerializeField] private string[] additiveScenes;
        [SerializeField] private string cameraModule;
        [SerializeField] private float delayBeforeLoad = 0.1f;
        [SerializeField] private Image progressBar;

        private const int MainSystemsCount = 5;
        private const int SubSystemsCount = 7;
        
        public const bool DebugDisabled = true;
        private bool _hasLocalizationLoaded;
        private bool _hasLoadedData;
        private bool _hasLoadedSkins;
        
        private int _mainSystemLoadedCount;
        private int _subSystemLoadedCount;
        
        // === Mobile Only === //
        
#if UNITY_ANDROID || UNITY_IOS
        
        private bool _hasLoadedAds;

#endif
        
        public event Action<string> OnLoadingAsset;
        public event Action OnStartLoading;
        
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

            BootEvents.OnSubSystemInitialized += () =>
            {
                _subSystemLoadedCount++;
            };
            
            BootEvents.OnMainSystemInitialized += () =>
            {
                _mainSystemLoadedCount++;
            };

            SoundEvents.OnSoundManagerInitialized += () =>
            {
                _mainSystemLoadedCount++;
            };

#if UNITY_ANDROID || UNITY_IOS

            AdsEvents.OnAdsInitialized += () =>
            {
                _hasLoadedAds = true;
            };
#endif

            LocalizationManager.LoadInstance();
            SoundManager.LoadInstance();
            AnalyticsManager.LoadInstance();
            GameConfigManager.LoadInstance();
            StatsManager.LoadInstance();
            FlagsManager.LoadInstance();
        }

        private void Start()
        {
            StartCoroutine(LoadCoreScene());
        }

        private IEnumerator LoadCoreScene()
        {
            var boostrapScene = SceneManager.GetActiveScene();
            
            AsyncOperation cameraSceneAsync = SceneManager.LoadSceneAsync(cameraModule, LoadSceneMode.Additive);
            cameraSceneAsync.allowSceneActivation = false;
            
            while (cameraSceneAsync.progress < 0.9f)
            {
                yield return null;
            }
            
            cameraSceneAsync.allowSceneActivation = true;
            
            yield return new WaitForSeconds(delayBeforeLoad);
            
            OnStartLoading?.Invoke();
            // Save Data
            OnLoadingAsset?.Invoke("Loading Saves");
            DataManager.LoadInstance();
            yield return new WaitForSeconds(delayBeforeLoad);
            yield return new WaitUntil(()=> _hasLoadedData);
            
            // Localization
            OnLoadingAsset?.Invoke("Loading Texts");

            yield return new WaitForSeconds(delayBeforeLoad);
            yield return new WaitUntil(()=> _hasLocalizationLoaded);
            
            // Skins
            OnLoadingAsset?.Invoke("Loading Skins");
            SkinManager.LoadInstance();
            yield return new WaitForSeconds(delayBeforeLoad);
            yield return new WaitUntil(()=> _hasLoadedSkins);

            
            //Ads
#if UNITY_ANDROID || UNITY_IOS

#pragma warning disable CS0162 // Unreachable code detected
            if (GameParameters.GameplayValues.AdsEnable)
            {
                OnLoadingAsset?.Invoke("Loading Ads");
                AdManager.LoadInstance();
                AdsEvents.InitializeAds();
                yield return new WaitForSeconds(delayBeforeLoad);
                yield return new WaitUntil(()=> _hasLoadedAds);
            }
            else
            {
                _mainSystemLoadedCount++;
            }
#pragma warning restore CS0162 // Unreachable code detected

#else
            _mainSystemLoadedCount++;
#endif
            
            OnLoadingAsset?.Invoke("Loading Main Scene");
            yield return new WaitForSeconds(delayBeforeLoad);
            
            foreach (string sceneName in additiveScenes)
            {
                AsyncOperation secondarySceneAsyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                // Espera hasta que la escena termine de cargar
                while (!secondarySceneAsyncLoad.isDone)
                {
                    yield return null;
                }
            }
            
            yield return new WaitForEndOfFrame();
            
            OnLoadingAsset?.Invoke("Loading Secondary Scenes");
            
            AsyncOperation coreSceneAsyncLoad = SceneManager.LoadSceneAsync(coreScene, LoadSceneMode.Additive);
            coreSceneAsyncLoad.allowSceneActivation = false;
            
            while (coreSceneAsyncLoad.progress < 0.9f)
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(delayBeforeLoad);
            
            coreSceneAsyncLoad.allowSceneActivation = true;

            yield return null;
            
            yield return new WaitUntil(() => coreSceneAsyncLoad.isDone);
            
            yield return new WaitForEndOfFrame();
            
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(coreScene));
            
            OnLoadingAsset?.Invoke("Initializing Data");
            
            yield return new WaitForSeconds(delayBeforeLoad);
            
            BootEvents.InitializeMainSystem();
            SoundEvents.InitializeSoundManager();
            
            OnLoadingAsset?.Invoke("Initializing Main Systems");
            
            yield return new WaitUntil(GetHasLoadedMainSystems);
            yield return new WaitForSeconds(delayBeforeLoad);
            
            BootEvents.InitializeSubSystems();
            
            OnLoadingAsset?.Invoke("Initializing Sub Systems");
            
            yield return new WaitUntil(GetHasLoadedSubsystems);
            yield return new WaitForSeconds(delayBeforeLoad);

            OnLoadingAsset?.Invoke("Initializing Game");
            
            yield return new WaitForSeconds(delayBeforeLoad);
            
            BootEvents.TriggerOnGameLoaded();
            
            yield return new WaitForEndOfFrame();

            SceneManager.UnloadSceneAsync(boostrapScene);
        }

        private bool GetHasLoadedMainSystems()
        {
            return _mainSystemLoadedCount >= MainSystemsCount;
        }

        private bool GetHasLoadedSubsystems()
        {
            return _subSystemLoadedCount >= SubSystemsCount;
        }
    }
}