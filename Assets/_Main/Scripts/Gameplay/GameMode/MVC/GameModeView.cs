﻿using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Observer;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeView : ManagedBehavior, IObserver, IUpdatable
    {
        [Header("Sounds")] 
        [SerializeField] private SoundBehavior gameplayTheme;
        [SerializeField] private SoundBehavior deathTheme;
        
        private GameModeController _controller;
        private GameModeUIView _uiView;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        public void ManagedUpdate()
        {

        }

        public void SetController(GameModeController controller)
        {
            _controller = controller;
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
                case GameModeObserverMessage.SpawnRingMeteor:
                    HandleSpawnRingMeteor();
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
                    HandleEarthRestartFinish();
                    break;
                case GameModeObserverMessage.MainMenu:
                    HandleMainMenu();
                    break;
            }
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
                        CustomTime.SetChannelPaused(UpdateGroup.Inputs, true);
                    },
                    OnEndAction = () =>
                    {
                        CustomTime.SetChannelPaused(UpdateGroup.Inputs, false);
                    },
                    
                }, UpdateGroup.Always);
            }
            
            GameManager.Instance.IsPaused = isPaused;
        }

        private void HandleMainMenu()
        {
            TimerManager.Clear();
            ActionManager.Clear();
            GameManager.Instance.EventManager.Clear();
            
            TimerManager.Add(new TimerData
            {
                Time = 1f,
                OnEndAction = () =>
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
                    
                    GameManager.Instance.LoadMainMenuScene();
                }
            }, UpdateGroup.Always);

        }

        private void HandleGameFinish()
        {
            gameplayTheme?.StopSound();
            GameManager.Instance.CanPlay = false;
            GameManager.Instance.EventManager.Publish(new ShieldEnable{IsEnabled = false});
            GameManager.Instance.EventManager.Publish(new RecycleAllMeteors());
            GameManager.Instance.EventManager.Publish(new SetEnableInputs{IsEnable = false});
        }
        
        private void HandleGameRestart()
        {
            var tempActions = new ActionData[]
            {
                new (()=>GameManager.Instance.EventManager.Publish(new GameRestart()),
                    GameRestartTimeValues.TriggerRestart),
                new (()=>GameManager.Instance.EventManager.Publish(new EarthRestart()),
                    GameRestartTimeValues.RestartEarth),
            };
            
            ActionManager.Add(new ActionQueue(tempActions),SelfUpdateGroup);
        }
        
        private void HandleEarthRestartFinish()
        {
            _controller.TransitionToStart();
        }

        #region Start

        private void HandleStartCountdown()
        {
            deathTheme?.StopSound();
            GameManager.Instance.EventManager.Publish(new CameraZoomOut());
        }
        
        private void HandleCountdownFinish()
        {
            GameManager.Instance.EventManager.Publish(new GameStart());
            GameManager.Instance.EventManager.Publish(new SetEnableInputs{IsEnable = true});
            GameManager.Instance.EventManager.Publish(new SetEnableAbility{IsEnable = true});
            _controller.TransitionToGameplay();
        }

        private void HandleStartGameplay()
        {
            GameManager.Instance.CanPlay = true;
            GameManager.Instance.EventManager.Publish(new ShieldEnable{IsEnabled = true});
            gameplayTheme?.PlaySound();
        }

        #endregion
        
        #region Earth

        private void HandleEarthStartDestruction()
        {
            GameManager.Instance.EventManager.Publish(new EarthStartDestruction());
        }
        
        private void HandleEarthShake()
        {
            GameManager.Instance.EventManager.Publish(new CameraZoomIn());
        }
        
        private void HandleEarthEndDestruction()
        {
            deathTheme?.PlaySound();
        }

        #endregion

        #region Meteor

        private void HandleSetEnableMeteorSpawn(bool canSpawn)
        {
            GameManager.Instance.EventManager.Publish(new EnableMeteorSpawn { CanSpawn = canSpawn });
        }
        
        private void HandleSpawnRingMeteor()
        {
            GameManager.Instance.EventManager.Publish(new SpawnRingMeteor{});
        }

        #endregion
        
        private void HandleUpdateGameLevel(int currentLevel)
        {
            GameManager.Instance.EventManager.Publish(new UpdateLevel{CurrentLevel = currentLevel});
        }
    }
}