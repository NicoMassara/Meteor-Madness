using System.Collections;
using _Main.Scripts.GlobalEvents;
using _Main.Scripts.Localization;
using _Main.Scripts.Managers;
using _Main.Scripts.MySettings;
using _Main.Scripts.Save;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.MyTest.CosmeticUI
{

#if UNITY_EDITOR
    
    public class CosmeticUITester : MonoBehaviour
    {
        private void Awake()
        {
            LocalizationEvents.OnLocalizationLoaded += () =>
            {
                StartCoroutine(Coroutine_LoadCoreScene());
            };
            
            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
        }

        private void Start()
        {
            LocalizationManager.LoadInstance();
            DataManager.LoadInstance();
            SettingsManager.LoadInstance();
            SoundManager.LoadInstance();
        }

        private IEnumerator Coroutine_LoadCoreScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("CosmeticsModule", LoadSceneMode.Additive);
                
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            yield return new WaitForEndOfFrame();
            
            BootEvents.InitializeMainSystem();

            yield return new WaitForEndOfFrame();
                
            BootEvents.InitializeSubSystems();
            
            yield return new WaitForEndOfFrame();
            
            SoundEvents.InitializeSoundManager();
            
            yield return new WaitForSeconds(1);
            
            BootEvents.TriggerOnGameLoaded();
            TestEvents.ShowEarth();
            
            yield return new WaitForEndOfFrame();
            
            GameManager.Instance.LoadCosmeticMenu();
        }

        private IEnumerator Coroutine_ReloadScreen()
        {
            yield return new WaitForSeconds(0.5f);
            
            GameManager.Instance.LoadCosmeticMenu();
        }

        #region Event Bus
        
        private void EventBus_GameScreen_Enable(GameScreenEvents.EnableScreen input)
        {
            if (input.RequestType == EventRequestType.Requested)
            {
                if (input.ScreenType == ScreenType.Cosmetic)
                {
                    GameScreenEventCaller.EnableScreen(ScreenType.Cosmetic, EventRequestType.Granted);
                }
                else
                {
                    GameScreenEventCaller.DisableScreen(ScreenType.Cosmetic, EventRequestType.Requested);
                }

            }
        }
        
        private void EventBus_GameScreen_Disable(GameScreenEvents.DisableScreen input)
        {
            if (input.RequestType == EventRequestType.Granted)
            {
                StartCoroutine(Coroutine_ReloadScreen());
            }
        }

        #endregion
    }
    
#endif
}