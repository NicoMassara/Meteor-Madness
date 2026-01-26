using System;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Contracts.Interfaces.Analytics;
using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers;
using MeteorMadness.Managers.GameConfig;
using MeteorMadness.Managers.Save;
using NicolasMassara.CustomTimerManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.GameMode
{
    public class GameModeView : ManagedBehavior, IObserver,IGameModeSounds,
        GameModeView.IGameModeView, IGameModeVibration, IGameModeAnalytics
    {
        public interface IGameModeView
        {
            public event Action OnPaused;
            public event Action OnResume;
            
            public event Action OnDataInitialized;
            public event Action<float> OnCountdownUpdated;
            public event Action OnCountDownStarted;
            public event Action OnCountDownFinished;
            
            public event Action OnGameStarted;
            public event Action OnGameStopped;
            public event Action OnScoreSaved;
            public event Action OnGameModeDisable;
        }
        
        #region IGameModeView
        
        public event Action OnPaused;
        public event Action OnResume;
            
        public event Action OnDataInitialized;
        public event Action<float> OnCountdownUpdated;
        public event Action OnInitialized;
        public event Action OnCountDownStarted;
        public event Action OnCountDownFinished;
        public event Action OnGameStarted;
        public event Action OnGameStopped;
        public event Action OnScoreSaved;
        public event Action OnGameModeDisable;
        #endregion

        #region IGameModeSounds

#pragma warning disable CS0067 // Event is never used
        public event Action OnGameModeFinished;
        public event Action OnCountdownUpdatedFinished;
#pragma warning restore CS0067 // Event is never used
        public event Action OnStopMusic;
        public event Action OnPlayMusic;
        public event Action<uint> OnStreakUpdated;

        #endregion

        #region IGameModeAnalytics
        
        public event Action<float> OnPointGained;
        public event Action<AbilityType> OnAbilityTriggered;
        public event Action<int> OnLevelUpdate;
        public event Action OnGameInterrupted;
        
        #endregion
        

        // ReSharper disable Unity.PerformanceAnalysis
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                //=== Disable ===//
                case GameModeObserverMessage.ExecuteDisable:
                    HandleExecuteDisable();
                    break;
                
                case GameModeObserverMessage.StartDisable:
                    HandleStartDisable();
                    break;
                
                case GameModeObserverMessage.TriggerEarthDestruction:
                    HandleTriggerEarthDestruction();
                    break;
                
                //=== Pause ===//
                case GameModeObserverMessage.GamePaused:
                    HandleGamePaused();
                    break;
                case GameModeObserverMessage.GameResume:
                    HandleGameResume();
                    break;
                case GameModeObserverMessage.PauseGameModeScreen:
                    HandlePauseGameModeScreen();
                    break;
                case GameModeObserverMessage.OpenPauseMenu:
                    HandleOpenPauseScreen();
                    break;
                
                //=== Data ===//
                case GameModeObserverMessage.InitializeData:
                    HandleInitializeData();
                    break;
                case GameModeObserverMessage.SaveScore:
                    HandleSaveScore((DataManagerTools.GameplayStatsIdData)args[0]);
                    break;
                
                
                //=== Meteor ===//
                case GameModeObserverMessage.PointsGained:
                    HandlePointsGained((Vector2)args[0],(uint)args[1],(bool)args[2]);
                    break;
                
                //=== Projectile ===//
                case GameModeObserverMessage.GrantProjectileSpawn:
                    HandleGrantProjectileSpawn((int)args[0]);
                    break;
                case GameModeObserverMessage.SetEnableSpawnMeteor:
                    HandleSetEnableMeteorSpawn((bool)args[0]);
                    break;
                
                //=== Countdown ===//
                case GameModeObserverMessage.StartCountdown:
                    HandleStartCountdown((int)args[0]);
                    break;
                case GameModeObserverMessage.UpdateCountdown:
                    HandleUpdateCountdown((float)args[0]);
                    break;
                case GameModeObserverMessage.FinishCountdown:
                    HandleFinishCountdown();
                    break;
                
                //=== Gameplay ===//
                case GameModeObserverMessage.StartGameplay:
                    HandleStartGameplay();
                    break;
                case GameModeObserverMessage.StopGameplay:
                    HandleStopGameplay();
                    break;
                
                //=== Internal Level ===//
                case GameModeObserverMessage.UpdateGameLevel:
                    HandleUpdateGameLevel((int)args[0]);
                    break;
                
                //=== Finish ===//
                case GameModeObserverMessage.StartFinish:
                    HandleStartFinish();
                    break;
                case GameModeObserverMessage.GameFinish:
                    HandleGameFinish();
                    break;
                case GameModeObserverMessage.GameInterrupted:
                    HandleGameInterrupted();
                    break;
                
                // === Ability ===//
                case GameModeObserverMessage.AbilityActive:
                    HandleAbilityActive((AbilityType)args[0]);
                    break;
                
                // === Streak === //
                case GameModeObserverMessage.UpdateStreak:
                    HandleUpdateStreak((uint)args[0]);
                    break;
            }
        }



        #region Streak

        private void HandleUpdateStreak(uint streakAmount) => OnStreakUpdated?.Invoke(streakAmount);

        #endregion
        
        #region Finish

        private void HandleStartFinish()
        {
            OnStopMusic?.Invoke();
            ShieldEventCaller.Disable();
        }
        
        private void HandleGameFinish()
        {
            GameManager.Instance.ResumeGame();
            GameManager.Instance.CanPlay = false;
            ShieldEventCaller.Disable();
        }
        
        private void HandleGameInterrupted()
        {
            OnGameInterrupted?.Invoke();
        }

        #endregion
        
        #region Gameplay
        
        private void HandleStartGameplay()
        {
            if (FlagsManager.GetHasPlayed() == false)
            {
                FlagsManager.SetHasPlayed();
                FlagsManager.SaveFlags();
            }

            GameConfigManager.Instance.SetDamage(DamageTypes.Standard);
            GameModeEventCaller.SetEnablePause(true);
            GameManager.Instance.CanPlay = true;
            OnGameStarted?.Invoke();
            OnPlayMusic?.Invoke();
            ShieldEventCaller.Enable();
            AbilitiesEventCaller.Enable();
            AbilitiesEventCaller.SetCanUse(true);
            AbilitiesEventCaller.EnableUI();
            EarthEventCaller.EnableDamage();
            SetEnableInputs(true);
            
            
#if UNITY_ANDROID || UNITY_IOS

            SetEnableUIInputs(true);
                
#endif
        }
        
        private void HandleStopGameplay()
        {
            EarthEventCaller.DisableDamage();
            AbilitiesEventCaller.DisableUI();
            SetEnableInputs(false);
            OnGameStopped?.Invoke();
            
#if UNITY_ANDROID || UNITY_IOS

            SetEnableUIInputs(false);
                
#endif
        }

        #endregion
        
        #region Meteor

        private void HandlePointsGained(Vector2 position, uint amount, bool isDouble)
        {
            OnPointGained?.Invoke(amount);
            var finalScore = (amount * GameConfigManager.Instance.GetGameplayData().PointsMultiplier);
            FloatingTextEventCaller.Spawn(new FloatingTextValues
            {
                Position = position, 
                Offset = new Vector2(0,1f),
                Text = $"+{finalScore.ToString()}",
                Color = isDouble ? Color.yellow : Color.white,
                DoesFade = true,
                DoesMove = true
            });
        }

        #endregion
        
        #region Disable

        private void HandleTriggerEarthDestruction()
        {
            EarthEventCaller.DestructionStart();
        }
        
        private void HandleExecuteDisable()
        {
            OnStopMusic?.Invoke();
            AbilitiesEventCaller.Disable();
            GameScreenEventCaller.DisableScreen(ScreenType.GameMode, EventRequestType.Granted);
        }
        
        private void HandleStartDisable()
        {
            OnGameModeDisable?.Invoke();
        }
        
        #endregion

        #region Pause

        private void HandleGamePaused()
        {
            SetEnableInputs(false);
            AbilitiesEventCaller.DisableUI();
            GameModeEventCaller.SetPause(true);
            OnPaused?.Invoke();
                
#if UNITY_ANDROID || UNITY_IOS

            SetEnableUIInputs(false);
                
#endif
            GameManager.Instance.PauseGame();
        }
        
        private void HandleGameResume()
        {
            OnResume?.Invoke();
            GameModeEventCaller.SetPause(false);
            AbilitiesEventCaller.EnableUI();
            EarthEventCaller.EnableDamage();
            AbilitiesEventCaller.EnableUI();
            GameManager.Instance.ResumeGame();
            SetEnableInputs(true);
            
#if UNITY_ANDROID || UNITY_IOS

            SetEnableUIInputs(true);
                
#endif
        }
        
        private void HandlePauseGameModeScreen()
        {
            GameScreenEventCaller.DisableScreen(ScreenType.GameMode, EventRequestType.Granted);
        }

        private void HandleOpenPauseScreen()
        {
            GameManager.Instance.LoadPauseScreen();
        }

        #endregion

        #region Data

        private void HandleInitializeData()
        {
            // There's nothing to initialize yet
            // Now works as a bypass
            EarthEventCaller.PreSlice();

            TimerManager.Add(new TimerData(0.5f, () =>
            {
                OnDataInitialized?.Invoke();
            }));
            
            OnInitialized?.Invoke();
        }
        
        private void HandleSaveScore(DataManagerTools.GameplayStatsIdData saveData)
        {
            GameModeEventCaller.SetEnablePause(false);
            GameManager.Instance.CanPlay = false;
            ShieldEventCaller.Disable();
            StatsManager.UpdateRuntimeData(saveData);
            OnScoreSaved?.Invoke();
        }
        

        #endregion
        
        #region Projectile

        private void HandleGrantProjectileSpawn(int typeIndex)
        {
            ProjectileEventCaller.GrantSpawn((ProjectileType)typeIndex);
        }

        private void HandleSetEnableMeteorSpawn(bool canSpawn)
        {
            if (canSpawn)
            {
                ProjectileEventCaller.EnableSpawn();
            }
            else
            {
                ProjectileEventCaller.DisableSpawn();
            }
        }

        #endregion
        
        #region Countdown

        private void HandleStartCountdown(int countdown)
        {
            AdsEvents.Banner_TriggerHide();
            OnCountDownStarted?.Invoke();
        }
        
        private void HandleUpdateCountdown(float time)
        {
            OnCountdownUpdated?.Invoke(time);
        }
        
        private void HandleFinishCountdown()
        {
            OnCountDownFinished?.Invoke();
        }

        #endregion
        
        #region Internal Level

        private void HandleUpdateGameLevel(int currentLevel)
        {
            ProjectileEventCaller.UpdateLevel(currentLevel);
            OnLevelUpdate?.Invoke(currentLevel);
        }

        #endregion
        
        #region Inputs

        private void SetEnableInputs(bool isEnable)
        {
            InputsEventCaller.SetEnable(isEnable);
        }

        private void SetEnableUIInputs(bool isEnable)
        {
            InputsEventCaller.SetUIEnable(isEnable);
        }

        #endregion

        #region Ability

        private void HandleAbilityActive(AbilityType abilityType)
        {
            OnAbilityTriggered?.Invoke(abilityType);
        }

        #endregion

        private void OnApplicationQuit()
        {
            OnGameInterrupted?.Invoke();
        }
    }
}