using System;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.GlobalValues;
using MeteorMadness.GlobalValues.Events;
using MeteorMadness.Managers;
using MeteorMadness.Managers.GameConfig;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.GameMode
{
    [RequireComponent(typeof(GameModeView))]
    [RequireComponent(typeof(GameModeUIView))]
    [RequireComponent(typeof(GameModeViewAnimation))]
    public class GameModeSetup : ManagedBehavior, IUpdatable
    {
        private GameModeController.IGameModeController _controller;
        private GameModeView.IGameModeView _view;
        private GameModeUIView.IGameModeUIView _ui;
        private GameModeViewAnimation.IGameModeViewAnimation _animation;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Always;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;


        private void Awake()
        {
            BootEvents.OnSubSystemRequestInitialize += Initialize;
        }
        
        public void ExecuteUpdate(float deltaTime)
        {
            _controller?.Execute(deltaTime);
        }

        private void Initialize()
        {
            BootEvents.OnSubSystemRequestInitialize -= Initialize;
            //
            var gameplayData = GameConfigManager.Instance.GetGameplayData();
            
            var motor = new GameModeMotor(gameplayData.LevelData.GetGameplayLevelRequierment());
            _controller = new GameModeController(motor);
            
            var view = GetComponent<GameModeView>();
            var ui = GetComponent<GameModeUIView>();
            var anim = GetComponent<GameModeViewAnimation>();
            
            motor.Subscribe(view);
            motor.Subscribe(ui);
            motor.Subscribe(anim);
            
            _view = view;
            _ui = ui;
            _animation = anim;
            
            SetViewHandlers();
            
            //
            
            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
            
            _controller.InitializeController(gameplayData.GameTimeData);
            
            BootEvents.SubSystemInitialized();
        }
        
        private void EnableGameMode()
        {
            _controller.TransitionToInitialize();
            SubscribeToEventBus();
        }

        private void DisableGameMode()
        {
            UnsubscribeToEventBus();
            _controller.TransitionToDisable();
        }

        #region View Handlers

        private void SetViewHandlers()
        {
            _view.OnDataInitialized += _controller.TransitionToCountDown;
            _view.OnScoreSaved += GameManager.Instance.LoadDefeatScreen;
            _view.OnGameModeDisable += _controller.ExecuteDisable;
            _view.OnPaused += () =>
            {
                EarthEventSubscriber.Restart(EventBus_Earth_Restart_Started);
                EarthEventSubscriber.RestartFinished(EventBus_Earth_Restart_Finished);
            };
            _view.OnResume += () =>
            {
                EarthEventUnSubscriber.RestartFinished(EventBus_Earth_Restart_Finished);
                EarthEventUnSubscriber.Restart(EventBus_Earth_Restart_Started);
            };
            //
            _ui.OnPauseButtonPressed += _controller.TransitionToPaused;
            _ui.OnFinishAddingPoints += _controller.TriggerFinishAddingPoints;
            //
            _animation.OnUiClosed += _controller.TriggerPauseMenu;
            _animation.OnCountdownFinished += _controller.TransitionToPlaying;
        }
        
        #endregion
        
        #region EventBus

        private void SubscribeToEventBus()
        {
            EarthEventSubscriber.Death(EventBus_Earth_Death);
            //
            AbilitiesEventSubscriber.NotifyIsActive(EventBus_Abilities_SetActive);
            //
            ProjectileEventSubscriber.Deflected(EventBus_Meteor_Deflected);
            ProjectileEventSubscriber.RequestSpawn(EventBus_Projectile_RequestSpawn);
            ProjectileEventSubscriber.Collision(EventBus_Projectile_Collision);
            //

            //
            GameModeEventSubscriber.SetEnablePause(EventBus_GameMode_SetEnablePause);
            //
            AbilitiesEventSubscriber.NotifyIsActive(EventBus_Abilities_IsActive);
        }

        private void UnsubscribeToEventBus()
        {
            EarthEventUnSubscriber.Death(EventBus_Earth_Death);
            //
            AbilitiesEventUnSubscriber.NotifyIsActive(EventBus_Abilities_SetActive);
            //
            ProjectileEventUnSubscriber.Deflected(EventBus_Meteor_Deflected);
            ProjectileEventUnSubscriber.RequestSpawn(EventBus_Projectile_RequestSpawn);
            //
            CameraEventSubscriber.NotifyTransportStarted(EventBus_Camera_Transport_Started);
            CameraEventSubscriber.NotifyTransportFinished(EventBus_Camera_Transport_Finished);
            //
            GameModeEventUnSubscriber.SetEnablePause(EventBus_GameMode_SetEnablePause);
            //
            AbilitiesEventUnSubscriber.NotifyIsActive(EventBus_Abilities_IsActive);
        }

        #region GameScreen

        private void EventBus_GameScreen_Disable(GameScreenEvents.DisableScreen input)
        {
            if(input.ScreenType != ScreenType.GameMode) return;
            
            if (input.RequestType == EventRequestType.Requested)
            {
                DisableGameMode();
            }
        }

        private void EventBus_GameScreen_Enable(GameScreenEvents.EnableScreen input)
        {
            if(input.ScreenType != ScreenType.GameMode) return;
            
            if (input.RequestType == EventRequestType.Granted)
            {
                EnableGameMode();
            }
        }

        #endregion
        
        #region Earth

        private void EventBus_Earth_Death(EarthEvents.Death input)
        {
            _controller.TransitionToSaveScore();
        }
        
        private void EventBus_Earth_Restart_Finished(EarthEvents.RestartFinished input)
        {
            EarthEventUnSubscriber.RestartFinished(EventBus_Earth_Restart_Finished);
            //
            DisableGameMode();
        }

        private void EventBus_Earth_Restart_Started(EarthEvents.Restart input)
        {
            EarthEventUnSubscriber.Restart(EventBus_Earth_Restart_Started);
            //
            _controller.TransitionToFinished();
        }

        #endregion
        
        #region Abilities

        private void EventBus_Abilities_SetActive(AbilitiesEvents.NotifyIsActive input)
        {
            if(input.IsActive)
            {
                _controller.IncreaseAbilityUseCount();

                if (input.AbilityType == AbilityType.DoublePoints)
                {
                    _controller.SetDoublePoints(true);
                }
            }
            else
            {
                _controller.SetDoublePoints(false);
            }
        }

        #endregion
        
        #region Projectile

        private void EventBus_Projectile_RequestSpawn(ProjectileEvents.RequestSpawn input)
        {
            if (input.RequestType == EventRequestType.Requested)
            {
                _controller.GrantProjectileSpawn((int)input.ProjectileType);
            }
        }
        
        private void EventBus_Projectile_Collision(ProjectileEvents.Collision input)
        {
            _controller.IncreaseCollisionCount();
        }

        private void EventBus_Meteor_Deflected(ProjectileEvents.Deflected input)
        {
            _controller.HandleMeteorDeflect(input.Position, input.Value);
            _controller.IncreaseDeflectCount();
        }

        #endregion

        #region CameraTransport

        private void EventBus_Camera_Transport_Started(CameraEvents.TransportStarted input)
        {
            if(input.Type == CameraTransportType.ZoomIn)
                _controller.DisableGameplayUI();
        }

        private void EventBus_Camera_Transport_Finished(CameraEvents.TransportFinished input)
        {
            if(input.Type == CameraTransportType.ZoomOut)
                _controller.EnableGameplayUI();
        }

        #endregion
        
        #region Pause

        private void EventBus_GameMode_SetEnablePause(GameModeEvents.SetEnablePause input)
        {
            if (input.CanPause)
            {
                _controller.EnablePause();
            }
            else
            {
                _controller.DisablePause();
            }
        }

        #endregion

        #region Abilities

        private void EventBus_Abilities_IsActive(AbilitiesEvents.NotifyIsActive input)
        {
            if (input.IsActive)
            {
                _controller.NotifyAbilityActive(input.AbilityType);
            }
        }

        #endregion

        #endregion

#if !UNITY_EDITOR

        private void OnApplicationFocus(bool hasFocus) => _controller.SetHasLoseFocus(!hasFocus);
        private void OnApplicationPause(bool pauseStatus) => _controller.SetHasLoseFocus(pauseStatus);
        
#endif
        
    }
}