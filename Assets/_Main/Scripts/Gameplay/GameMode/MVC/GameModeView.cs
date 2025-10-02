using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Observer;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeView : ManagedBehavior, IObserver, IUpdatable
    {
        [Header("Sounds")] 
        [SerializeField] private SoundBehavior gameplayTheme;
        [SerializeField] private SoundBehavior deathTheme;
        
        private GameModeUIView _uiView;
        
        public UnityAction<bool> OnEarthRestarted;
        public UnityAction OnCountdownFinished;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        public void ManagedUpdate() { }
        
        //Hack
        private bool _isFirstDisable = true;
        
        
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
                case GameModeObserverMessage.Initialize:
                    HandleInitialize();
                    break;
                
            }
        }
        private void HandleInitialize()
        {
            GameManager.Instance.EventManager.Publish(new GameModeEvents.Initialize());
        }
        
        private void HandleGamePaused(bool isPaused)
        {
            CustomTime.SetChannelPaused(new []
            {
                UpdateGroup.Gameplay,
                UpdateGroup.Ability, 
                UpdateGroup.Shield,
                UpdateGroup.Earth,
                UpdateGroup.Effects,
                UpdateGroup.Camera
                
            }, isPaused);

            if (isPaused == true)
            {
                TimerManager.Add(new TimerData
                {
                    Time = 0.5f,
                    OnStartAction = () =>
                    {
                        SetEnableInputs(false);
                    },
                    OnEndAction = () =>
                    {
                        SetEnableInputs(true);
                    },
                    
                }, UpdateGroup.Always);
            }
            
            GameManager.Instance.IsPaused = isPaused;
        }

        private void HandleDisable()
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

            if (_isFirstDisable == false)
            {
                GameManager.Instance.EventManager.Publish(new EarthEvents.Restart());
            }
            
            gameplayTheme?.StopSound();
            deathTheme?.StopSound();
            _isFirstDisable = false;
            SetEnableInputs(false);
            GameManager.Instance.EventManager.Publish(new GameModeEvents.Disable());
        }

        private void HandleGameFinish()
        {
            gameplayTheme?.StopSound();
            GameManager.Instance.CanPlay = false;
            GameManager.Instance.EventManager.Publish(new ShieldEvents.SetEnable{IsEnabled = false});
            GameManager.Instance.EventManager.Publish(new MeteorEvents.RecycleAll());
            SetEnableInputs(false);
        }
        
        private void HandleGameRestart()
        {
            var tempActions = new ActionData[]
            {
                new (()=>GameManager.Instance.EventManager.Publish(new GameModeEvents.Restart()),
                    GameParameters.TimeValues.Restart.TriggerRestart),
                new (()=>GameManager.Instance.EventManager.Publish(new EarthEvents.Restart()),
                    GameParameters.TimeValues.Restart.RestartEarth),
            };
            
            ActionManager.Add(new ActionQueue(tempActions),SelfUpdateGroup);
        }
        
        private void HandleEarthRestartFinish(bool doesRestart)
        {
            OnEarthRestarted?.Invoke(doesRestart);
        }

        #region Start

        private void HandleStartCountdown()
        {
            deathTheme?.StopSound();
            GameManager.Instance.EventManager.Publish(new CameraEvents.ZoomOut());
        }
        
        private void HandleCountdownFinish()
        {
            GameManager.Instance.EventManager.Publish(new GameModeEvents.Start());
            SetEnableInputs(true);
            GameManager.Instance.EventManager.Publish(new AbilitiesEvents.SetEnable{IsEnable = true});
            OnCountdownFinished?.Invoke();
        }

        private void HandleStartGameplay()
        {
            GameManager.Instance.CanPlay = true;
            GameManager.Instance.EventManager.Publish(new ShieldEvents.SetEnable{IsEnabled = true});
            gameplayTheme?.PlaySound();
        }

        #endregion
        
        #region Earth

        private void HandleEarthStartDestruction()
        {
            GameManager.Instance.EventManager.Publish(new EarthEvents.DestructionStart());
        }
        
        private void HandleEarthShake()
        {
            GameManager.Instance.EventManager.Publish(new CameraEvents.ZoomIn());
        }
        
        private void HandleEarthEndDestruction()
        {
            deathTheme?.PlaySound();
        }

        #endregion

        #region Meteor

        private void HandleSetEnableMeteorSpawn(bool canSpawn)
        {
            GameManager.Instance.EventManager.Publish(new MeteorEvents.EnableSpawn { CanSpawn = canSpawn });
        }

        #endregion
        
        private void HandleUpdateGameLevel(int currentLevel)
        {
            GameManager.Instance.EventManager.Publish(new GameModeEvents.UpdateLevel{CurrentLevel = currentLevel});
        }

        private void SetEnableInputs(bool isEnable)
        {
            GameManager.Instance.EventManager.Publish(new InputsEvents.SetEnable{IsEnable = isEnable});
        }
    }
}