using System;
using System.Collections;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Managers;
using MeteorMadness.Managers.Localization;
using MeteorMadness.Managers.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.MyTest.Stats
{
    public class StatsScreenTester : MonoBehaviour
    {
        [Serializable]
        private class StatsData
        {
            [Range(0, 1000)]
            public uint Score;
            [Range(0, 1000)]
            public uint CollisionCount;
            [Range(0, 1000)]
            public uint AbilityUseCount;
            [Range(0, 1000)]
            public uint DeflectCount;
            [Range(0, 1000)]
            public uint MaxStreak;
            [Range(0, 1000)]
            public uint HighScore;
            [Range(0, 1000)]
            public uint TimesPlayed;
            [Range(0, 1000)]
            public uint HistoricalScore;
            [Range(0f, 1000f)]
            public float LongestTime;
        }
        
        [SerializeField] private StatsData statsData;
        

        private void Awake()
        {
            LocalizationEvents.OnLocalizationLoaded += () => LoadScreen();

            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
        }
            
        private void Start()
        {
            LocalizationManager.LoadInstance();
            DataManager.LoadInstance();
            SettingsManager.LoadInstance();
        }

        private void SetStats()
        {
            StatsManager.ModifyValueByStat(StatType.HighScore, statsData.HighScore);
            StatsManager.ModifyValueByStat(StatType.Collision, statsData.CollisionCount);
            StatsManager.ModifyValueByStat(StatType.Deflect, statsData.DeflectCount);
            StatsManager.ModifyValueByStat(StatType.Ability, statsData.AbilityUseCount);
            StatsManager.ModifyValueByStat(StatType.Streak, statsData.MaxStreak);
            StatsManager.ModifyValueByStat(StatType.TimesPlayed, statsData.TimesPlayed);
            StatsManager.ModifyValueByStat(StatType.TotalScored, statsData.HistoricalScore);
            StatsManager.ModifyValueByStat(StatType.Time, statsData.LongestTime);
        }

        private void LoadScreen()
        {
            StartCoroutine(Coroutine_LoadScreen());
        }

        private IEnumerator Coroutine_LoadScreen()
        {
            yield return new WaitForSeconds(0.25f);
                
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("StatsModule", LoadSceneMode.Additive);
                
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            yield return new WaitForEndOfFrame();
                
            SetStats();
            
            yield return new WaitForEndOfFrame();
                
            GameManager.Instance.LoadStatsScreen();
            
            yield return null;
        }
        
        private void ReloadScreen()
        {
            StartCoroutine(Coroutine_ReloadScreen());
        }

        private IEnumerator Coroutine_ReloadScreen()
        {
            yield return new WaitForSeconds(0.25f);
                
            SetStats();
            
            yield return new WaitForEndOfFrame();
                
            GameManager.Instance.LoadStatsScreen();
            
            yield return null;
        }
        
        #region Event Bus

        private void EventBus_GameScreen_Disable(GameScreenEvents.DisableScreen input)
        {
            if (input.RequestType == EventRequestType.Granted)
            {
                ReloadScreen();
            }
        }
        
        private void EventBus_GameScreen_Enable(GameScreenEvents.EnableScreen input)
        {
            if (input.RequestType == EventRequestType.Requested)
            {
                if (input.ScreenType == ScreenType.Stats)
                {
                    GameScreenEventCaller.EnableScreen(ScreenType.Stats, EventRequestType.Granted);
                }
                else
                {
                    GameScreenEventCaller.DisableScreen(ScreenType.Stats, EventRequestType.Requested);
                }
            }
        }

        #endregion
    }
}