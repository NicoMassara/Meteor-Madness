using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Save;
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
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        public TickGroup SelfTickGroup { get; } = TickGroup.HalfTick;
        public float LastUpdateTime { get; set; }
        
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
            
            GameEventCaller.Subscribe<GameScreenEvents.EnableScreen>(EventBus_GameScreen_Enable);
            GameEventCaller.Subscribe<GameScreenEvents.DisableScreen>(EventBus_GameScreen_Disable);
            
        }

        private void Start()
        {
            _controller.Initialize();
            
            var saveData = DataManager.Instance.GetData<ScoreSaveData>(SaveDataType.Score);
            _controller.SetHighScore(saveData.HighScore);
        }
        
        public void ExecuteUpdate()
        {
            if (_isEnable)
            {
                _controller?.Execute(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
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
            _view.OnCountdownFinished += View_OnCountdownFinishedHandler;
            _view.OnGameModeEnable += ViewOnGameModeEnableHandler;
        }

        private void ViewOnGameModeEnableHandler()
        {
            _controller.TransitionToStart();
        }

        private void View_OnCountdownFinishedHandler()
        {
            _controller.TransitionToGameplay();
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
            _ui.OnRestartButtonPressed += UIView_OnRestartButtonPressedHandler;
            _ui.OnMainMenuButtonPressed += UIView_OnMainMenuButtonPressedHandler;
            _ui.OnPauseButtonPressed += UIView_OnPauseButtonPressedHandler;
        }

        private void UIView_OnPauseButtonPressedHandler()
        {
            GameModeEventCaller.SetPause(true);
        }

        private void UIView_OnMainMenuButtonPressedHandler()
        {
            _controller.SetDoesRestartGameMode(false);
            _controller.TriggerMainMenu();
        }

        private void UIView_OnRestartButtonPressedHandler()
        {
            _controller.TransitionToRestart();
        }

        #endregion
        
        #region EventBus

        private void SubscribeToEventBus()
        {

            GameEventCaller.Subscribe<EarthEvents.ShakeStart>(EventBus_Earth_ShakeStart);
            GameEventCaller.Subscribe<EarthEvents.DestructionFinished>(EventBus_Earth_DestructionFinished);
            GameEventCaller.Subscribe<EarthEvents.RestartFinished>(EventBus_Earth_RestartFinish);
            GameEventCaller.Subscribe<EarthEvents.Death>(EventBus_Earth_Death);
            //
            GameEventCaller.Subscribe<AbilitiesEvents.NotifyIsActive>(EventBus_Abilities_SetActive);
            //
            GameEventCaller.Subscribe<ProjectileEvents.Deflected>(EventBus_Meteor_Deflected);
            GameEventCaller.Subscribe<ProjectileEvents.RequestSpawn>(EventBus_Projectile_RequestSpawn);
            //
            GameEventCaller.Subscribe<CameraEvents.ZoomIn>(EventBus_Camera_ZoomIn);
            GameEventCaller.Subscribe<CameraEvents.ZoomOut>(EventBus_Camera_ZoomOut);
            //;
            GameEventCaller.Subscribe<GameModeEvents.SetPause>(EventBus_GameMode_SetPaused);
            GameEventCaller.Subscribe<GameModeEvents.SetEnablePause>(EventBus_GameMode_SetEnablePause);
        }
        

        private void UnsubscribeToEventBus()
        {

            GameEventCaller.Unsubscribe<EarthEvents.ShakeStart>(EventBus_Earth_ShakeStart);
            GameEventCaller.Unsubscribe<EarthEvents.Death>(EventBus_Earth_Death);
            GameEventCaller.Unsubscribe<EarthEvents.DestructionFinished>(EventBus_Earth_DestructionFinished);
            GameEventCaller.Unsubscribe<EarthEvents.RestartFinished>(EventBus_Earth_RestartFinish);
            //
            GameEventCaller.Unsubscribe<AbilitiesEvents.NotifyIsActive>(EventBus_Abilities_SetActive);
            //
            GameEventCaller.Unsubscribe<ProjectileEvents.RequestSpawn>(EventBus_Projectile_RequestSpawn);
            GameEventCaller.Unsubscribe<ProjectileEvents.Deflected>(EventBus_Meteor_Deflected);
            //
            GameEventCaller.Unsubscribe<GameModeEvents.SetPause>(EventBus_GameMode_SetPaused);
            GameEventCaller.Unsubscribe<GameModeEvents.SetEnablePause>(EventBus_GameMode_SetEnablePause);
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
        
        #region GameMode
        
        private void EventBus_GameMode_SetEnablePause(GameModeEvents.SetEnablePause input)
        {
            _controller.SetCanPause(input.CanPause);
        }

        private void EventBus_GameMode_SetPaused(GameModeEvents.SetPause input)
        {
            _controller.SetGamePause(input.IsPaused);
        }
        #endregion

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