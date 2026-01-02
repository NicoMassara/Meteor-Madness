using System;
using System.Collections;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Managers;
using MeteorMadness.Managers.Cosmetics;
using MeteorMadness.Managers.GameConfig;
using MeteorMadness.Managers.Localization;
using MeteorMadness.Managers.Save;
using MeteorMadness.ScreenFlow.Defeat;
using MeteorMadness.Services.AdsSystem;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.MyTest.Defeat
{
        public class DefeatScreenTester : MonoBehaviour
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            [SerializeField] private SkinType deathTitleCode;
            [Range(0,100)]
            [SerializeField] private uint startCoins;
            [Range(0,1000)]
            [SerializeField] private uint scoreAmount;
            [Range(0,1000)]
            [SerializeField] private uint storedHighScore;
            
            private bool _hasInitializedAds;
            private bool _hasInitializedManager;
            
#pragma warning disable CS0414 // Field is assigned but its value is never used
            private bool _canReload;
#pragma warning restore CS0414 // Field is assigned but its value is never used
            
            private void Awake()
            {
                LocalizationEvents.OnLocalizationLoaded += () =>
                {
                    StartCoroutine(LoadDefeatScreen());
                };

                DebugDefeatEvents.OnDefeatScreenAnimationFinished += () =>
                {
                    StartCoroutine(Coroutine_DisableDefeatScreen());
                };

                DebugDefeatEvents.OnDefeatScreenClosed += () =>
                {
                    Debug.Log("Defeat Screen Closed");
                    EarthEventCaller.RestartFinished();
                };
                
                AdsEvents.OnAdsInitialized += () =>
                {
                    _hasInitializedAds = true;
                };
            
                BootEvents.OnMainSystemInitialized += () =>
                {
                    _hasInitializedManager = true;
                };

                GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
                GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
            }
            
            private void Start()
            {
                LocalizationManager.LoadInstance();
                SkinManager.LoadInstance();
                DataManager.LoadInstance();
                SettingsManager.LoadInstance();
                SoundManager.LoadInstance();
                GameConfigManager.LoadInstance();
            }
            
            private void SetScoreValue()
            {
                StatsManager.UpdateRuntimeData(new DataManagerTools.GameplayStatsIdData
                {
                    RuntimeScoreId = SecureValueManager.RegisterValue(scoreAmount * 100)
                });
                
                StatsManager.ModifyValueByStat(StatType.HighScore, storedHighScore * 100);
            }

            private IEnumerator Coroutine_DisableDefeatScreen()
            {
                yield return new WaitForEndOfFrame();
                
                GameManager.Instance.LoadGameMode();
                
                yield return null;
            }

            private IEnumerator LoadDefeatScreen()
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("DefeatModule", LoadSceneMode.Additive);
                
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }
                
                yield return new WaitForEndOfFrame();
                
                SoundEvents.InitializeSoundManager();
                
                yield return new WaitForEndOfFrame();
                

                if (GameParameters.GameplayValues.AdsEnable)
                {
                    AdManager.LoadInstance();
                
                    yield return new WaitForEndOfFrame();
                    
                    AdsEvents.InitializeAds();
                
                    yield return new WaitForEndOfFrame();
                    yield return new WaitUntil(()=> _hasInitializedAds == true);
                }
                
                BootEvents.InitializeMainSystem();
                
                yield return new WaitForEndOfFrame();
                
                BootEvents.InitializeSubSystems();
                
                yield return new WaitForEndOfFrame();
                

                yield return new WaitForEndOfFrame();

                SkinManager.Instance.TryAddCoins(startCoins * 20);
                
                yield return new WaitForEndOfFrame();
                
                SetScoreValue();
                
                yield return new WaitForEndOfFrame();
                
                GameManager.Instance.LoadDefeatScreen();

                yield return new WaitForSeconds(0.25f);
                
                EarthEventCaller.DestructionFinished();
            }

            private IEnumerator ReloadDefeatScreen()
            {
                yield return new WaitForEndOfFrame();
                
                
                yield return new WaitForEndOfFrame();
                
                SetScoreValue();
                
                yield return new WaitForEndOfFrame();
                
                GameManager.Instance.LoadDefeatScreen();

                yield return new WaitForEndOfFrame();
                
                EarthEventCaller.DestructionFinished();
            }

            #region Event Bus

            private void EventBus_GameScreen_Disable(GameScreenEvents.DisableScreen input)
            {
                if (input.RequestType == EventRequestType.Granted)
                {
                    StartCoroutine(ReloadDefeatScreen());
                }
            }

            private void EventBus_GameScreen_Enable(GameScreenEvents.EnableScreen input)
            {
                if (input.RequestType == EventRequestType.Requested)
                {
                    if (input.ScreenType == ScreenType.Defeat)
                    {
                        GameScreenEventCaller.EnableScreen(ScreenType.Defeat, EventRequestType.Granted);
                    }
                    else
                    {
                        GameScreenEventCaller.DisableScreen(ScreenType.Defeat, EventRequestType.Requested);
                        _canReload = true;
                    }
                }
            }

            #endregion
#endif
        }
}
