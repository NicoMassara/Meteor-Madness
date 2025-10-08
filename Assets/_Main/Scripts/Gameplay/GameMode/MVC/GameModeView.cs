﻿using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Observer;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeView : ManagedBehavior, IObserver
    {
        [Header("Sounds")] 
        [SerializeField] private SoundBehavior gameplayTheme;
        [SerializeField] private SoundBehavior deathTheme;
        
        public event Action<bool> OnEarthRestarted;
        public event Action OnCountdownFinished;
        public event Action OnGameModeEnable;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        
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
                EarthEventCaller.Restart();
            }
            
            gameplayTheme?.StopSound();
            deathTheme?.StopSound();
            _isFirstDisable = false;
            SetEnableInputs(false);
            GameModeEventCaller.Disable();
        }

        private void HandleGameFinish()
        {
            gameplayTheme?.StopSound();
            GameManager.Instance.CanPlay = false;
            ShieldEventCaller.SetEnableShield(false);
            MeteorEventCaller.RecycleAll();
            SetEnableInputs(false);
        }
        
        private void HandleGameRestart()
        {
            var temp = GameConfigManager.Instance.GetGameplayData().GameTimeData;
            
            var tempActions = new ActionData[]
            {
                new (GameModeEventCaller.Restart, temp.TriggerRestart),
                new (EarthEventCaller.Restart, temp.RestartEarth),
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
            CameraEventCaller.ZoomOut();
        }
        
        private void HandleCountdownFinish()
        {
            GameModeEventCaller.Start();
            SetEnableInputs(true);
            AbilitiesEventCaller.SetCanUse(true);
            OnCountdownFinished?.Invoke();
        }

        private void HandleStartGameplay()
        {
            GameManager.Instance.CanPlay = true;
            ShieldEventCaller.SetEnableShield(true);
            gameplayTheme?.PlaySound();
        }

        #endregion
        
        #region Earth

        private void HandleEarthStartDestruction()
        {
            EarthEventCaller.DestructionStart();
        }
        
        private void HandleEarthShake()
        {
            GameEventCaller.Publish(new CameraEvents.ZoomIn());
        }
        
        private void HandleEarthEndDestruction()
        {
            deathTheme?.PlaySound();
        }

        #endregion

        #region Meteor

        private void HandleSetEnableMeteorSpawn(bool canSpawn)
        {
            MeteorEventCaller.EnableSpawn(canSpawn);
        }

        #endregion
        
        private void HandleUpdateGameLevel(int currentLevel)
        {
            GameModeEventCaller.UpdateLevel(currentLevel);
        }

        private void SetEnableInputs(bool isEnable)
        {
            InputsEventCaller.SetEnable(isEnable);
        }
    }
}