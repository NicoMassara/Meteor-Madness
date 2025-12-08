using System.Collections;
using _Main.Scripts.Defeat;
using _Main.Scripts.Localization;
using _Main.Scripts.Managers;
using _Main.Scripts.Save;
using _Main.Scripts.SecurityData;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.MyTest.Defeat
{
#if UNITY_EDITOR   
        [AddComponentMenu("_Main/ModuleTester/DefeatScreenTester")]
        public class DefeatScreenTester : MonoBehaviour
        {
            [Range(0,1000)]
            [SerializeField] private uint scoreAmount;
            [Range(0,1000)]
            [SerializeField] private uint highScore;
            
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
                    EarthEventCaller.RestartFinished();
                };

                GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
                GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
            }
            
            private void Start()
            {
                var localization = LocalizationManager.Instance;
                var dataManager = DataManager.Instance;
            }
            
            private void SetScoreValue()
            {
                GameManager.Instance.CurrentScoreSecuredId = 
                    SecureValueManager.RegisterValue(scoreAmount);

                SecureValueManager.ModifyValue(GameManager.Instance.GetHighScoreSecuredId(), highScore);
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
                
                BootEvents.InitializeMainSystem();

                yield return new WaitForSeconds(1);
                
                BootEvents.InitializeSubSystems();

                yield return new WaitForEndOfFrame();
                
                SetScoreValue();
                
                yield return new WaitForEndOfFrame();
                
                GameManager.Instance.LoadDefeatScreen();

                yield return new WaitForEndOfFrame();
                
                EarthEventCaller.DestructionFinished();
            }

            private IEnumerator ReloadDefeatScreen()
            {
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
        }
#endif
}
