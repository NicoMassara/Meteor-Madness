using System;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using _Main.Scripts.Save;
using NicolasMassara.CustomTimerManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeView : ManagedBehavior, IObserver,
        IGameModeSounds
    {
        [Range(0,1f)]
        [SerializeField] private float timeToZoomOutOnEnable = 0.25f;
        [Range(0,1f)]
        [SerializeField] private float timeToZoomInOnDeath = 0.25f;
        [Range(0,1f)]
        [SerializeField] private float timeToZoomOutOnPause = 0.25f;
        
        public event Action<bool> OnEarthRestarted;
        public event Action OnCountdownFinished;
        public event Action OnCountDownStarted;
        public event Action OnCountdownUpdated;
        public event Action OnCountdownUpdatedFinished;
        public event Action OnGameModeEnable;
        public event Action OnGameModeDisable;
        public event Action OnGameModeRestarted;
        public event Action<bool> OnGameModePaused;

        public event Action OnGameModeFinished;
        public event Action OnGameModeStarted;
        public event Action OnEarthDeath;

        #region IGameModeSounds
        public event Action OnStopMusic;

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
                // Enable / Disable 
                case GameModeObserverMessage.Enable:
                    HandleEnable();
                    break;
                case GameModeObserverMessage.Asleep:
                    HandleAsleep();
                    break;
                case GameModeObserverMessage.Disable:
                    HandleDisable();
                    break;
                case GameModeObserverMessage.Leaving:
                    HandleLeaving();
                    break;
                
                
                // Pause / Unpause
                case GameModeObserverMessage.GamePaused:
                    HandleGamePaused();
                    break;
                case GameModeObserverMessage.GameUnPaused:
                    HandleGameUnPaused();
                    break;
                
                // CountDown
                case GameModeObserverMessage.StartCountdown:
                    HandleStartCountdown();
                    break;
                case GameModeObserverMessage.CountdownFinish:
                    HandleCountdownFinish();
                    break;
                case GameModeObserverMessage.UpdateCountdown:
                    HandleCountdown((float)args[0]);
                    break;
                
                // Earth
                case GameModeObserverMessage.EarthStartDestruction:
                    HandleEarthStartDestruction();
                    break;
                case GameModeObserverMessage.EarthShaking:
                    HandleEarthShake();
                    break;
                case GameModeObserverMessage.EarthEndDestruction:
                    HandleEarthEndDestruction();
                    break;
                case GameModeObserverMessage.EarthRestartFinish:
                    HandleEarthRestartFinish((bool)args[0]);
                    break;
                
                // GameMode
                case GameModeObserverMessage.StartGameplay:
                    HandleStartGameplay();
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
                case GameModeObserverMessage.InitializeValues:
                    HandleInitialize();
                    break;

                
                // Meteor
                case GameModeObserverMessage.SetEnableSpawnMeteor:
                    HandleSetEnableMeteorSpawn((bool)args[0]);
                    break;
                case GameModeObserverMessage.GrantProjectileSpawn:
                    HandleGrantProjectileSpawn((int)args[0]);
                    break;


                
                // Score
                case GameModeObserverMessage.PointsGained:
                    HandlePointsGained((Vector2)args[0],(float)args[1],(bool)args[2]);
                    break;
                case GameModeObserverMessage.SaveHighScore:
                    HandleSaveHighScore((float)args[0]);
                    break;
 
                // Screens
                case GameModeObserverMessage.Options:
                    HandleOptions();
                    break;
                case GameModeObserverMessage.TriggerMainMenu:
                    HandleTriggerMainMenu();
                    break;
                
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                // Debug
                case GameModeObserverMessage.MeteorDeflect:
                    HandleMeteorDeflect((float)args[0]);
                    break;
                case GameModeObserverMessage.UpdateHighScore:
                    HandleUpdateHighScore((float)args[0]);
                    break;
#endif
            }
        }




        private void HandleOptions()
        {
            GameManager.Instance.LoadOptionsMenu();
        }

        #region Debug Only

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

        #endregion

        #region Screns

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
            CameraEventCaller.ZoomIn(timeToZoomOutOnPause);
            
            SetEnableInputs(false);
            SetEnableUIInputs(false);
        }
        
        private void HandleAsleep()
        {
            GameScreenEventCaller.DisableScreen(ScreenType.GameMode, EventRequestType.Granted);
        }

        #endregion

        #region Score/Points

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

        #endregion

        #region Pause
        
        private void HandleGameUnPaused()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.IsPaused = false;
#endif
            
            OnGameModePaused?.Invoke(false);
            SetEnableInputs(true);
            AbilitiesEventCaller.EnableUI();
            GameModeEventCaller.SetPause(false);
            GameManager.Instance.UnpauseGame();
            
#if UNITY_ANDROID || UNITY_IOS

            SetEnableUIInputs(true);
                
#endif
            
        }
        
        private void HandleGamePaused()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.IsPaused = true;
            
#endif
            OnGameModePaused?.Invoke(true);
            GameManager.Instance.PauseGame();
            SetEnableInputs(false);
            AbilitiesEventCaller.DisableUI();
            GameModeEventCaller.SetPause(true);
            CameraEventCaller.ZoomIn(timeToZoomInOnDeath);
                
#if UNITY_ANDROID || UNITY_IOS

            SetEnableUIInputs(false);
                
#endif
        }

        #endregion
        
        #region GameMode
        
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
            OnStopMusic?.Invoke();
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
        
        
        private void HandleDisable()
        {
            OnGameModeDisable?.Invoke();
            OnStopMusic?.Invoke();
            EarthEventCaller.DisableDamage();
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
            
            OnGameModeRestarted?.Invoke();
            OnStopMusic?.Invoke();
        }
        
        private void HandleEarthRestartFinish(bool doesRestart)
        {
            OnEarthRestarted?.Invoke(doesRestart);
        }
        
        private void HandleUpdateGameLevel(int currentLevel)
        {
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.CurrentLevel = currentLevel;
#endif
            ProjectileEventCaller.UpdateLevel(currentLevel);
        }
        
        private void HandleLeaving()
        {
            OnStopMusic?.Invoke();
        }

        #endregion
        
        #region Start

        private void HandleStartCountdown()
        {
            CameraEventCaller.ZoomOut(timeToZoomOutOnEnable);
            OnCountDownStarted?.Invoke();
        }
        
        private void HandleCountdownFinish()
        {
            EarthEventCaller.EnableDamage();
            AbilitiesEventCaller.Enable();
            AbilitiesEventCaller.EnableUI();
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
            OnStopMusic?.Invoke();
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