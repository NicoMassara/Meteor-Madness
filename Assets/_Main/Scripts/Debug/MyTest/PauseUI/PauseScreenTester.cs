using System.Collections;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Managers;
using MeteorMadness.Managers.Localization;
using MeteorMadness.Managers.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.MyTest.PauseUI
{
#if UNITY_EDITOR
    [AddComponentMenu("_Main/ModuleTester/Pause Screen Tester")]
    public class PauseScreenTester : MonoBehaviour
    {
        private void Awake()
        {
            LocalizationEvents.OnLocalizationLoaded += () =>
            {
                StartCoroutine(Coroutine_EnablePauseScreen());
            };
            
            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
        }
        
        private void Start()
        {
            var settings = SettingsManager.Instance;
            var localization = LocalizationManager.Instance;
            var dataManager = DataManager.Instance;
        }

        private void ReloadScreen()
        {
            StartCoroutine(Coroutine_ReloadScreen());
        }

        private IEnumerator Coroutine_ReloadScreen()
        {
            yield return new WaitForSeconds(0.5f);
            
            GameManager.Instance.LoadPauseScreen();
        }
        
        private IEnumerator Coroutine_EnablePauseScreen()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("PauseModule", LoadSceneMode.Additive);
                
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
                
            yield return new WaitForEndOfFrame();
                
            BootEvents.InitializeMainSystem();

            yield return new WaitForEndOfFrame(); ;
                
            BootEvents.InitializeSubSystems();

            yield return new WaitForEndOfFrame();
            
            GameManager.Instance.LoadPauseScreen();
        }

        private void EventBus_GameScreen_Enable(GameScreenEvents.EnableScreen input)
        {
            if (input.RequestType == EventRequestType.Requested)
            {
                if(input.ScreenType == ScreenType.Pause)
                {
                    GameScreenEventCaller.EnableScreen(ScreenType.Pause, EventRequestType.Granted);
                }
                else
                {
                    GameScreenEventCaller.DisableScreen(ScreenType.Pause, EventRequestType.Requested);
                }
            }
        }
        
        private void EventBus_GameScreen_Disable(GameScreenEvents.DisableScreen input)
        {
            if (input.RequestType == EventRequestType.Granted)
            {
                ReloadScreen();
            }
        }
    }
#endif
}