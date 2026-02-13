using System;
using System.Collections;
using MeteorMadness.Contracts.Events;
using MeteorMadness.GlobalValues.Events;
using MeteorMadness.Managers.Localization;
using MeteorMadness.Managers.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MeteorMadness.Debug._Main.Scripts.Debug
{
    public class TestTools
    {
        public static IEnumerator LoadScreen(string[] modulesName, int mainSystemsCount, int subSystemsCount, 
            Action setDataAction, Action loadScreenAction)
        {
            
            var currentMainSystems = 0;
            var currentSubSystems = 0;
            var hasLoadedData = false;
            var hasLoadedLocalization = false;

            BootEvents.OnMainSystemInitialized += () => currentMainSystems++;
            BootEvents.OnSubSystemInitialized += () => currentSubSystems++;
            SaveDataEvents.OnSaveInitialized += () => hasLoadedData = true;
            LocalizationEvents.OnLocalizationLoaded += () => hasLoadedLocalization = true;
            
            DataManager.LoadInstance();
            LocalizationManager.LoadInstance();
            
            yield return new WaitForSeconds(0.1f);
            
            yield return new WaitUntil(()=> hasLoadedData);
            yield return new WaitUntil(()=> hasLoadedLocalization);
            
            
            foreach (string sceneName in modulesName)
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                
                while (!asyncLoad.isDone)
                    yield return null;
            }
            
            yield return new WaitForEndOfFrame();
            
            BootEvents.InitializeMainSystem();
            
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(()=> currentMainSystems >= mainSystemsCount);
            
            BootEvents.InitializeSubSystems();
            
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(()=> currentSubSystems >= subSystemsCount);
            
            setDataAction?.Invoke();
            
            yield return new WaitForEndOfFrame();
            
            loadScreenAction?.Invoke();
            
            yield return null;
        }
    }
}