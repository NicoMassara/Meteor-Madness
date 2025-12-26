using System;
using System.Collections;
using MeteorMadness.GlobalValues;
using MeteorMadness.GlobalValues.Events;
using MeteorMadness.Managers;
using MeteorMadness.Managers.Cosmetics;
using MeteorMadness.Managers.Localization;
using MeteorMadness.Managers.Save;
using MeteorMadness.ScreenFlow.Defeat;
using MeteorMadness.Services.AdsSystem;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.MyTest.Defeat
{
        [AddComponentMenu("_Main/ModuleTester/Defeat Screen Tester")]
        public class DefeatScreenTester : MonoBehaviour
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            [SerializeField] private SkinType deathTitleCode;
            [Range(0,100)]
            [SerializeField] private uint startCoins;
            [Range(0,1000)]
            [SerializeField] private uint scoreAmount;
            [Range(0,1000)]
            [SerializeField] private uint highScore;
            
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
            }
            
            private void SetScoreValue()
            {
                GameManager.Instance.StatsController.SetStatsIdData(new DataManagerTools.GameplayStatsIdData
                {
                    CurrentScoreId = SecureValueManager.RegisterValue(scoreAmount)
                });

                SecureValueManager.ModifyValue(GameManager.Instance.StatsController.GetHighScoreSecuredId(), highScore);
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
                
                //yield return new WaitUntil(()=> _hasInitializedManager == true);
                yield return new WaitForEndOfFrame();
                
                BootEvents.InitializeSubSystems();
                
                yield return new WaitForEndOfFrame();

                GameManager.Instance.DeathTitle = GetDeathTitleCode();

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
                
                GameManager.Instance.DeathTitle = GetDeathTitleCode();
                
                yield return new WaitForEndOfFrame();
                
                SetScoreValue();
                
                yield return new WaitForEndOfFrame();
                
                GameManager.Instance.LoadDefeatScreen();

                yield return new WaitForEndOfFrame();
                
                EarthEventCaller.DestructionFinished();
            }

            private string GetDeathTitleCode()
            {
                return deathTitleCode switch
                {
                    SkinType.Default => "Cosmetic.SkinData.Default.DeathTitle",
                    SkinType.Pizza => "Cosmetic.SkinData.Pizza.DeathTitle",
                    _ => "Cosmetic.SkinData.Default.DeathTitle"
                };
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
