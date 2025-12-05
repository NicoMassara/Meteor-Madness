using System;
using _Main.Scripts.CustomId;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeView : ManagedBehavior, IObserver,
        GameModeView.IGameModeView
    {
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Always;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;
        
        public interface IGameModeView
        {
            public event Action OnDataInitialized;
            public event Action OnCountdownUpdated;
            public event Action OnCountDownStarted;
            public event Action OnCountDownFinished;
            
            public event Action OnGameStarted;
            public event Action OnGameStopped;
            public event Action OnScoreSaved;
            public event Action OnGameModeDisable;
        }
        
        
        public event Action OnGameModeDisable;
        public event Action OnScoreSaved;
        public event Action OnCountdownUpdated;
        public event Action OnGameStarted;
        public event Action OnGameStopped;
        public event Action OnDataInitialized;
        public event Action OnCountDownFinished;
        public event Action OnCountDownStarted;
        public event Action OnStopMusic;
        
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
                
                //=== Pause ===//
                case GameModeObserverMessage.GamePaused:
                    HandleGamePaused();
                    break;
                case GameModeObserverMessage.GameUnPaused:
                    HandleGameUnPaused();
                    break;
                case GameModeObserverMessage.TriggerPause:
                    HandleTriggerPause();
                    break;
                
                //=== Data ===//
                case GameModeObserverMessage.InitializeData:
                    HandleInitializeData();
                    break;
                case GameModeObserverMessage.SaveScore:
                    HandleSaveScore((GeneratedId)args[0]);
                    break;
                
                //=== Meteor ===//
                case GameModeObserverMessage.PointsGained:
                    HandlePointsGained((Vector2)args[0],(float)args[1],(bool)args[2]);
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
                case GameModeObserverMessage.GameFinish:
                    HandleGameFinish();
                    break;
                
                //=== Internal Level ===//
                case GameModeObserverMessage.UpdateGameLevel:
                    HandleUpdateGameLevel((int)args[0]);
                    break;
                
            }
        }

        #region Gameplay
        
        private void HandleStartGameplay()
        {
            EarthEventCaller.EnableDamage();
            AbilitiesEventCaller.Enable();
            AbilitiesEventCaller.EnableUI();
            SetEnableInputs(true);
            SetEnableUIInputs(true);
            OnGameStarted?.Invoke();
        }
        
        private void HandleStopGameplay()
        {
            EarthEventCaller.DisableDamage();
            AbilitiesEventCaller.Disable();
            AbilitiesEventCaller.DisableUI();
            SetEnableInputs(false);
            SetEnableUIInputs(false);
            CameraEventCaller.ZoomIn(0.5F);
            OnGameStopped?.Invoke();
        }
        
        private void HandleGameFinish()
        {
            GameModeEventCaller.SetEnablePause(false);
            GameManager.Instance.CanPlay = false;
            ShieldEventCaller.Disable();
        }

        #endregion
        
        #region Meteor

        private void HandlePointsGained(Vector2 position, float amount, bool isDouble)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.PointsGained += amount;
            
#endif
            var finalScore = (int)(amount * GameConfigManager.Instance.GetGameplayData().PointsMultiplier);
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
        
        private void HandleExecuteDisable()
        {
            OnStopMusic?.Invoke();
            EarthEventCaller.DestructionStart();
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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.IsPaused = true;
            
#endif
            GameManager.Instance.PauseGame();
            SetEnableInputs(false);
            AbilitiesEventCaller.DisableUI();
            GameModeEventCaller.SetPause(true);
            CameraEventCaller.ZoomIn(0.5f);
                
#if UNITY_ANDROID || UNITY_IOS

            SetEnableUIInputs(false);
                
#endif
            GameManager.Instance.LoadPauseScreen();
        }
        
        private void HandleGameUnPaused()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.IsPaused = false;
#endif
            
            SetEnableInputs(true);
            AbilitiesEventCaller.EnableUI();
            GameModeEventCaller.SetPause(false);
            GameManager.Instance.UnpauseGame();
            
#if UNITY_ANDROID || UNITY_IOS

            SetEnableUIInputs(true);
                
#endif
        }
        
        private void HandleTriggerPause()
        {
            GameScreenEventCaller.DisableScreen(ScreenType.GameMode, EventRequestType.Granted);
        }

        #endregion

        #region Data

        private void HandleInitializeData()
        {
            // There's nothing to initialize yet
            // Now works as a bypass
            EarthEventCaller.PreSlice();
            OnDataInitialized?.Invoke();
        }
        
        private void HandleSaveScore(GeneratedId generatedId)
        {
            GameManager.Instance.CurrentScoreSecuredId = generatedId;
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
            CameraEventCaller.ZoomOut(0.5f);
            OnCountDownStarted?.Invoke();
        }
        
        private void HandleFinishCountdown()
        {
            GameModeEventCaller.SetEnablePause(true);
            GameManager.Instance.CanPlay = true;
            ShieldEventCaller.Enable();
            GameConfigManager.Instance.SetDamage(DamageTypes.Standard);
            OnCountDownFinished?.Invoke();
        }

        #endregion
        
        #region Internal Level

        private void HandleUpdateGameLevel(int currentLevel)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.CurrentLevel = currentLevel;
#endif
            ProjectileEventCaller.UpdateLevel(currentLevel);
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
        
    }
}