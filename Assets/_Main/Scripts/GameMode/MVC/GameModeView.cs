using System;
using _Main.Scripts.CustomId;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Interfaces.Vibration;
using _Main.Scripts.Interfaces.Analytics;
using _Main.Scripts.Managers;
using _Main.Scripts.GameConfig;
using _Main.Scripts.GlobalEvents;
using _Main.Scripts.Observer;
using _Main.Scripts.SecurityData;
using NicolasMassara.CustomTimerManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.GameMode
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

        #endregion

        #region IGameModeAnalytics
        
        public event Action<float> OnPointGained;
        public event Action<AbilityType> OnAbilityTriggered;
        public event Action<int> OnLevelUpdate;
        public event Action OnGameInterrupted;
        
        #endregion
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        
        private GameModeDebugData _debugData;
        
#endif

        private void Awake()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData = new GameModeDebugData();
#endif
        }
        

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
                case GameModeObserverMessage.GameUnPaused:
                    HandleGameUnPaused();
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
                    HandleSaveScore((GeneratedId)args[0]);
                    break;
                case GameModeObserverMessage.SaveStats:
                    HandleSaveStats((GeneratedId)args[0],(GeneratedId)args[1],(GeneratedId)args[2]);
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
            }
        }

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
            OnGameStarted?.Invoke();
            EarthEventCaller.EnableDamage();
            AbilitiesEventCaller.Enable();
            AbilitiesEventCaller.EnableUI();
            SetEnableInputs(true);
            SetEnableUIInputs(true);
        }
        
        private void HandleStopGameplay()
        {
            EarthEventCaller.DisableDamage();
            AbilitiesEventCaller.DisableUI();
            SetEnableInputs(false);
            SetEnableUIInputs(false);
            CameraEventCaller.ZoomIn(0.5F);
            OnGameStopped?.Invoke();
        }

        #endregion
        
        #region Meteor

        private void HandlePointsGained(Vector2 position, uint amount, bool isDouble)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.PointsGained += amount;
            
#endif
            OnPointGained?.Invoke(amount);
            var finalScore = (ushort)(amount * GameConfigManager.Instance.GetGameplayData().PointsMultiplier);
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
            GameManager.Instance.VisualPoints = 0;
            GameScreenEventCaller.DisableScreen(ScreenType.GameMode, EventRequestType.Granted);
        }
        
        private void HandleStartDisable()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.PointsGained = 0;
            
#endif
            
            OnGameModeDisable?.Invoke();
        }
        
        #endregion

        #region Pause

        private void HandleGamePaused()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.IsPaused = true;
            
#endif
            SetEnableInputs(false);
            AbilitiesEventCaller.DisableUI();
            GameModeEventCaller.SetPause(true);
            CameraEventCaller.ZoomIn(0.5f);
            OnPaused?.Invoke();
                
#if UNITY_ANDROID || UNITY_IOS

            SetEnableUIInputs(false);
                
#endif
            GameManager.Instance.PauseGame();
        }
        
        private void HandleGameUnPaused()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.IsPaused = false;
#endif
            
            OnResume?.Invoke();
            SetEnableInputs(true);
            AbilitiesEventCaller.EnableUI();
            GameModeEventCaller.SetPause(false);
            GameManager.Instance.ResumeGame();
            
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
        
        private void HandleSaveScore(GeneratedId generatedId)
        {
            GameModeEventCaller.SetEnablePause(false);
            GameManager.Instance.CanPlay = false;
            ShieldEventCaller.Disable();
            GameManager.Instance.CurrentScoreSecuredId = generatedId;
            
            if (SecureValueManager.GetDoesContainValue<uint>(generatedId,
                    out var currentScore))
            {
                //Debug.LogWarning($"Current Score: {currentScore}");
            }
            OnScoreSaved?.Invoke();
        }
        
        private void HandleSaveStats(
            GeneratedId collisionCount, 
            GeneratedId abilityUseCount, 
            GeneratedId deflectCount)
        {
            GameManager.Instance.CollisionCountId = collisionCount;
            GameManager.Instance.AbilityUseCountId = abilityUseCount;
            GameManager.Instance.DeflectCountId = deflectCount;
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
            CameraEventCaller.ZoomOut(0.5f);
            OnCountDownStarted?.Invoke();
        }
        
        private void HandleUpdateCountdown(float time)
        {
            OnCountdownUpdated?.Invoke(time);
        }
        
        private void HandleFinishCountdown()
        {
            GameModeEventCaller.SetEnablePause(true);
            GameManager.Instance.CanPlay = true;
            ShieldEventCaller.Enable();
            GameConfigManager.Instance.SetDamage(DamageTypes.Standard);
            OnCountDownFinished?.Invoke();
            OnPlayMusic?.Invoke();
            
        }

        #endregion
        
        #region Internal Level

        private void HandleUpdateGameLevel(int currentLevel)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.CurrentLevel = currentLevel;
#endif
            ProjectileEventCaller.UpdateLevel(currentLevel);
            OnLevelUpdate?.Invoke(currentLevel);
        }

        #endregion
        
        #region Inputs

        private void SetEnableInputs(bool isEnable)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.InputsEnable = isEnable;
#endif
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