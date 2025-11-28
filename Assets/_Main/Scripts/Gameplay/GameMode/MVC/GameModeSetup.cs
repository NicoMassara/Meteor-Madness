using _Main.Scripts.Managers;
using _Main.Scripts.Save;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    [RequireComponent(typeof(GameModeView))]
    [RequireComponent(typeof(GameModeUIView))]
    public class GameModeSetup : ManagedBehavior, IUpdatable
    {
        private GameModeMotor _motor;
        private GameModeController _controller;
        
        private GameModeView _view;
        private GameModeUIView _ui;
        
        private bool _isEnable;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Systems;
        public TickGroup SelfTickGroup { get; } = TickGroup.EightTarget;
        
        private void Awake()
        {
            var gameplayData = GameConfigManager.Instance.GetGameplayData();
            
            _motor = new GameModeMotor(gameplayData.LevelData.GetGameplayLevelRequierment(),
                gameplayData.GameTimeData.StartGameCount);
            _controller = new GameModeController(_motor);
            
            _view = GetComponent<GameModeView>();
            _ui = GetComponent<GameModeUIView>();
            
            _motor.Subscribe(_view);
            _motor.Subscribe(_ui);

            SetViewHandlers();
            SetUIViewHandlers();
            
            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
        }

        private void Start()
        {
            _controller.Initialize();
            
            var saveData = DataManager.Instance.GetData<DataManager.ScoreSaveData>(DataManager.SaveDataType.Score);
            _controller.SetHighScore(saveData.HighScore);
        }
        
        public void ExecuteUpdate(float deltaTime)
        {
            if (_isEnable)
            {
                _controller?.Execute(deltaTime);
            }
        }
        
        private void EnableGameMode()
        {
            if (_isEnable == true) return;
            _controller.SetDoesRestartGameMode(true);
            _controller.TransitionToEnable();
            _isEnable = true;
            SubscribeToEventBus();
        }

        private void DisableGameMode()
        {
            if (_isEnable == false) return;
            
            _isEnable = false;
            UnsubscribeToEventBus();
            _controller.TransitionToDisable();
        }

        #region View Handlers

        private void SetViewHandlers()
        {
            _view.OnEarthRestarted += View_OnEarthRestartedHandler;
            _view.OnCountdownFinished += () =>
            {
                _controller.TransitionToGameplay();
            };
            _view.OnGameModeEnable += () =>
            {
                _controller.TransitionToStart();
            };
        }
        

        private void View_OnEarthRestartedHandler(bool doesRestart)
        {
            if (doesRestart)
            {
                _controller.TransitionToStart();
            }
            else
            {
                GameManager.Instance.LoadMainMenu();
            }
        }

        #endregion

        #region UI View Handlers
        
        private void SetUIViewHandlers()
        {
            _ui.OnRestartButtonPressed += () =>
            {
                _controller.TransitionToRestart();
            };
            _ui.OnMainMenuButtonPressed += UIView_OnMainMenuButtonPressedHandler;

            _ui.OnResumeButtonPressed += () =>
            {
                _controller.TransitionToCountDown();
            };
            _ui.OnPauseButtonPressed += () =>
            {
                _controller.TransitionToPause();
            };
            _ui.OnOptionsButtonPressed += () =>
            {
                _controller.SetToSleep();
                _controller.TriggerOptions();
            };
        }

        private void UIView_OnMainMenuButtonPressedHandler()
        {
            _controller.SetDoesRestartGameMode(false);
            _controller.TransitionToLeaving();
            _controller.TriggerMainMenu();
        }

        #endregion
        
        #region EventBus

        private void SubscribeToEventBus()
        {
            
            EarthEventSubscriber.ShakeStart(EventBus_Earth_ShakeStart);
            EarthEventSubscriber.DestructionFinished(EventBus_Earth_DestructionFinished);
            EarthEventSubscriber.RestartFinished(EventBus_Earth_RestartFinish);
            EarthEventSubscriber.Death(EventBus_Earth_Death);
            //
            AbilitiesEventSubscriber.NotifyIsActive(EventBus_Abilities_SetActive);
            //
            ProjectileEventSubscriber.Deflected(EventBus_Meteor_Deflected);
            ProjectileEventSubscriber.RequestSpawn(EventBus_Projectile_RequestSpawn);
            //
            CameraEventSubscriber.ZoomIn(EventBus_Camera_ZoomIn);
            CameraEventSubscriber.ZoomOut(EventBus_Camera_ZoomOut);
            //
            GameModeEventSubscriber.SetEnablePause(EventBus_GameMode_SetEnablePause);
        }
        

        private void UnsubscribeToEventBus()
        {

            EarthEventUnSubscriber.ShakeStart(EventBus_Earth_ShakeStart);
            EarthEventUnSubscriber.DestructionFinished(EventBus_Earth_DestructionFinished);
            EarthEventUnSubscriber.RestartFinished(EventBus_Earth_RestartFinish);
            EarthEventUnSubscriber.Death(EventBus_Earth_Death);
            //
            AbilitiesEventUnSubscriber.NotifyIsActive(EventBus_Abilities_SetActive);
            //
            ProjectileEventUnSubscriber.Deflected(EventBus_Meteor_Deflected);
            ProjectileEventUnSubscriber.RequestSpawn(EventBus_Projectile_RequestSpawn);
            //
            CameraEventUnSubscriber.ZoomIn(EventBus_Camera_ZoomIn);
            CameraEventUnSubscriber.ZoomOut(EventBus_Camera_ZoomOut);
            //
            GameModeEventUnSubscriber.SetEnablePause(EventBus_GameMode_SetEnablePause);
        }
        

        #region Camera

        private void EventBus_Camera_ZoomOut(CameraEvents.ZoomOut input)
        {
            _controller.HandleCameraZoomOut();
        }

        private void EventBus_Camera_ZoomIn(CameraEvents.ZoomIn input)
        {
            _controller.HandleCameraZoomIn();
        }

        #endregion

        #region Projectile

        private void EventBus_Projectile_RequestSpawn(ProjectileEvents.RequestSpawn input)
        {
            if(input.RequestType == EventRequestType.Requested)
            {
                _controller.GrantProjectileSpawn((int)input.ProjectileType);
            }
        }

        #endregion

        #region Abilities

        private void EventBus_Abilities_SetActive(AbilitiesEvents.NotifyIsActive inputs)
        {
            if (inputs.AbilityType == AbilityType.DoublePoints)
            {
                _controller.SetDoublePoints(inputs.IsActive);
            }
        }

        #endregion
        
        private void EventBus_GameMode_SetEnablePause(GameModeEvents.SetEnablePause input)
        {
            _controller.SetCanPause(input.CanPause);
        }

        #region Meteor

        private void EventBus_Meteor_Deflected(ProjectileEvents.Deflected input)
        {
            _controller.HandleProjectileDeflect(input.Position,input.Value);
        }

        #endregion
        
        #region Earth

        private void EventBus_Earth_RestartFinish(EarthEvents.RestartFinished input)
        {
            _controller.EarthRestartFinish();
        }
        
        private void EventBus_Earth_DestructionFinished(EarthEvents.DestructionFinished destructionFinished)
        {
            _controller.TransitionToDeath();
        }

        private void EventBus_Earth_ShakeStart(EarthEvents.ShakeStart shakeStart)
        {
            _controller.HandleEarthShake();
        }
        
        private void EventBus_Earth_Death(EarthEvents.Death input)
        {
            _controller.TransitionToFinish();
        }

        #endregion

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

        #endregion
    }
}