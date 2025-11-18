using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using _Main.Scripts.Save;
using NicolasMassara.CustomTimerManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeView : ManagedBehavior, IObserver
    {

        public event Action<bool> OnEarthRestarted;
        public event Action OnCountdownFinished;
        public event Action OnCountDownStarted;
        public event Action OnCountdownUpdated;
        public event Action OnCountdownUpdatedFinished;
        public event Action OnGameModeEnable;

        public event Action OnGameModeFinished;
        public event Action OnGameModeStarted;
        public event Action OnEarthDeath;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        
        
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
                case GameModeObserverMessage.StartCountdown:
                    HandleStartCountdown();
                    break;
                case GameModeObserverMessage.CountdownFinish:
                    HandleCountdownFinish();
                    break;
                case GameModeObserverMessage.UpdateCountdown:
                    HandleCountdown((float)args[0]);
                    break;
                case GameModeObserverMessage.StartGameplay:
                    HandleStartGameplay();
                    break;
                case GameModeObserverMessage.EarthStartDestruction:
                    HandleEarthStartDestruction();
                    break;
                case GameModeObserverMessage.EarthShaking:
                    HandleEarthShake();
                    break;
                case GameModeObserverMessage.EarthEndDestruction:
                    HandleEarthEndDestruction();
                    break;
                case GameModeObserverMessage.SetEnableSpawnMeteor:
                    HandleSetEnableMeteorSpawn((bool)args[0]);
                    break;
                case GameModeObserverMessage.GameFinish:
                    HandleGameFinish();
                    break;
                case GameModeObserverMessage.UpdateGameLevel:
                    HandleUpdateGameLevel((int)args[0]);
                    break;
                case GameModeObserverMessage.GameRestart:
                    HandleGameRestart();
                    break;
                case GameModeObserverMessage.GamePaused:
                    HandleGamePaused((bool)args[0]);
                    break;
                case GameModeObserverMessage.EarthRestartFinish:
                    HandleEarthRestartFinish((bool)args[0]);
                    break;
                case GameModeObserverMessage.Disable:
                    HandleDisable();
                    break;
                case GameModeObserverMessage.InitializeValues:
                    HandleInitialize();
                    break;
                case GameModeObserverMessage.PointsGained:
                    HandlePointsGained((Vector2)args[0],(float)args[1],(bool)args[2]);
                    break;
                case GameModeObserverMessage.GrantProjectileSpawn:
                    HandleGrantProjectileSpawn((int)args[0]);
                    break;
                case GameModeObserverMessage.Enable:
                    HandleEnable();
                    break;
                case GameModeObserverMessage.TriggerMainMenu:
                    HandleTriggerMainMenu();
                    break;
                case GameModeObserverMessage.SaveHighScore:
                    HandleSaveHighScore((float)args[0]);
                    break;

                
                
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        
                case GameModeObserverMessage.MeteorDeflect:
                    HandleMeteorDeflect((float)args[0]);
                    break;
                case GameModeObserverMessage.UpdateHighScore:
                    HandleUpdateHighScore((float)args[0]);
                    break;
#endif
                
            }
        }



#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
        private void HandleMeteorDeflect(float deflectedAmount)
        {
            _debugData.DeflectedMeteor = deflectedAmount;
        }
        
        private void HandleUpdateHighScore(float highScore)
        {
            _debugData.HighScore = highScore;
        }
            
#endif
        
        private void HandleSaveHighScore(float highScore)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.HighScore = highScore;
#endif
            
            DataManager.Instance.SaveGameData(new ScoreSaveData
            {
                HighScore = highScore,
            }, SaveDataType.Score);
        }
        
        private void HandleTriggerMainMenu()
        {
            CustomTime.SetChannelPaused(new []
            {
                UpdateGroup.Gameplay,
                UpdateGroup.Ability, 
                UpdateGroup.Shield,
                UpdateGroup.Earth,
                UpdateGroup.Effects,
                UpdateGroup.Camera
                
            }, false);

            AbilitiesEventCaller.Disable();
            ShieldEventCaller.Disable();
            EarthEventCaller.Restart();
            
            SetEnableInputs(false);
            SetEnableUIInputs(false);
        }
        

        private void HandleCountdown(float amount)
        {
            if (amount > 1)
            {
                OnCountdownUpdated?.Invoke();
            }
            else if (amount <= 1 && amount > 0)
            {
                OnCountdownUpdatedFinished?.Invoke();
            }
        }

        private void HandleEnable()
        {
            OnGameModeEnable?.Invoke();
        }

        private void HandleGrantProjectileSpawn(int projectileTypeIndex)
        {
            ProjectileEventCaller.GrantSpawn((ProjectileType)projectileTypeIndex);
        }

        private void HandleInitialize()
        {
            GameConfigManager.Instance.SetDamage(DamageTypes.Standard);
            GameModeEventCaller.InitializeValues();
        }
        
        private void HandlePointsGained(Vector2 position, float pointsAmount, bool isDouble = false)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.PointsGained += pointsAmount;
            
#endif
            var finalScore = (int)(pointsAmount * GameConfigManager.Instance.GetGameplayData().PointsMultiplier);
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
        
        private void HandleGamePaused(bool isPaused)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.IsPaused = isPaused;
            
#endif
            
            AbilitiesEventCaller.SetEnableUI(!isPaused);

            CustomTime.SetChannelPaused(new []
            {
                UpdateGroup.Gameplay,
                UpdateGroup.Ability, 
                UpdateGroup.Shield,
                UpdateGroup.Earth,
                UpdateGroup.Effects,
                UpdateGroup.Camera
                
            }, isPaused);

#if UNITY_ANDROID || UNITY_IOS
            SetEnableInputs(!isPaused);
            SetEnableUIInputs(!isPaused);
#else

            if (isPaused == true)
            {
                TimerManager.Add(new TimerData(0.5f, () =>
                {
                    SetEnableInputs(false);
                }, () =>
                {
                    SetEnableInputs(true);
                }));
            }
#endif
            
            GameManager.Instance.IsPaused = isPaused;
        }

        private void HandleDisable()
        {
            EarthEventCaller.SetToDefault();
            GameScreenEventCaller.DisableScreen(ScreenType.GameMode, EventRequestType.Granted);
        }

        private void HandleGameFinish()
        {
            GameManager.Instance.CanPlay = false;
            AbilitiesEventCaller.Disable();
            ShieldEventCaller.Disable();
            SetEnableInputs(false);
            SetEnableUIInputs(false);
        }
        
        private void HandleGameRestart()
        {
            var temp = GameConfigManager.Instance.GetGameplayData().GameTimeData;
            
            TimerManager.Add(new TimerData(time: temp.RestartEarth,
                onEndAction: EarthEventCaller.Restart));
        }
        
        private void HandleEarthRestartFinish(bool doesRestart)
        {
            OnEarthRestarted?.Invoke(doesRestart);
        }

        #region Start

        private void HandleStartCountdown()
        {
            CameraEventCaller.ZoomOut();
            OnCountDownStarted?.Invoke();
        }
        
        private void HandleCountdownFinish()
        {
            AbilitiesEventCaller.Enable();
            SetEnableInputs(true);
            SetEnableUIInputs(true);
            OnCountdownFinished?.Invoke();
#if UNITY_EDITOR || DEVELOPMENT_BUILD

            _debugData.PointsGained = 0;
#endif
        }

        private void HandleStartGameplay()
        {
            GameModeEventCaller.SetEnablePause(true);
            GameManager.Instance.CanPlay = true;
            ShieldEventCaller.Enable();
            GameConfigManager.Instance.SetDamage(DamageTypes.Standard);
            OnGameModeStarted?.Invoke();
        }

        #endregion
        
        #region Earth

        private void HandleEarthStartDestruction()
        {
            EarthEventCaller.DestructionStart();
        }
        
        private void HandleEarthShake()
        {
            OnEarthDeath?.Invoke();
            GameEventCaller.Publish(new CameraEvents.ZoomIn());
        }
        
        private void HandleEarthEndDestruction()
        {
            OnGameModeFinished?.Invoke();
        }

        #endregion

        #region Meteor

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
        
        private void HandleUpdateGameLevel(int currentLevel)
        {
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.CurrentLevel = currentLevel;
#endif
            ProjectileEventCaller.UpdateLevel(currentLevel);
        }

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
    }
}