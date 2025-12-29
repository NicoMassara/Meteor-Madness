using System;
using System.Collections;
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
            [Range(0, 1)]
            [SerializeField] private float deflectCount;
            //
            [Range(0, 1)]
            [SerializeField] private float collisionCount;
            //
            [Range(0, 1)]
            [SerializeField] private float abilityUseCount;
            //
            [Range(0, 1)]
            [SerializeField] private float timesPlayed;
            //
            [Range(0, 1)]
            [SerializeField] private float maxStreak;
            //
            [Range(0, 1)]
            [SerializeField] private float longestTime;
            //
            [Range(0, 1)]
            [SerializeField] private float highScore;
            //
            [Range(0, 1)]
            [SerializeField] private float historicalScore;


            public uint DeflectCount => LerpFullRangeUInt(deflectCount);
            public uint CollisionCount => LerpFullRangeUInt(collisionCount);
            public uint AbilityUseCount => LerpFullRangeUInt(abilityUseCount);
            public uint TimesPlayed => LerpFullRangeUInt(timesPlayed);
            public uint MaxStreak => LerpFullRangeUInt(maxStreak);
            public uint LongestTime => LerpFullRangeUInt(longestTime, uint.MaxValue-1);
            public uint HighScore => LerpFullRangeUInt(highScore);
            public uint HistoricalScore => LerpFullRangeUInt(historicalScore);
            
            
            public static uint LerpFullRangeUInt(float t,  uint max = uint.MaxValue)
            {
                t = Math.Clamp(t, 0f, 1f);

                uint min = uint.MinValue;
                
                return (uint)Math.Round(min + (max - min) * t);
            }
            
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