using System;
using System.Collections;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Debug._Main.Scripts.Debug;
using MeteorMadness.Managers;
using MeteorMadness.Managers.GameConfig;
using UnityEngine;

namespace _Main.Scripts.MyTest.Stats
{
    public class StatsScreenTester : MonoBehaviour
    {
        [Serializable]
        private class StatsData
        {
            [Min(0)]
            public uint DeflectCount;
            [Min(0)]
            public uint CollisionCount;
            [Min(0)]
            public uint AbilityUseCount;
            [Min(0)]
            public uint TimesPlayed;
            [Min(0)]
            public uint MaxStreak;
            [Min(0)]
            public uint LongestTime;
            [Min(0)]
            public uint HighScore;
            [Min(0)]
            public uint HistoricalScore;

        }
        
        [SerializeField] private StatsData statsData;
        

        private void Awake()
        {
            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
        }
            
        private void Start()
        {
            SettingsManager.LoadInstance();
            GameConfigManager.LoadInstance();
            StatsManager.LoadInstance();
            FlagsManager.LoadInstance();

            LoadScreen();
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
            StatsManager.ModifyValueByStat(StatType.LongestTime, statsData.LongestTime);
        }

        private void LoadScreen()
        {
            var enumerator = TestTools.LoadScreen(
                new string[] { "StatsModule" }, 
                3, 0, 
                SetStats,
                GameManager.Instance.LoadStatsScreen);
            StartCoroutine(enumerator);
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