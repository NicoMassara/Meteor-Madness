using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
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
            
            GameEventCaller.Subscribe<GameScreenEvents.SetScreen>(EventBus_GameScreen_SetScreen);
        }

        private void Start()
        {
            _controller.Initialize();
        }
        
        public void ManagedUpdate()
        {
            if (_isEnable)
            {
                _controller?.Execute(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
            }
        }
        
        private void EnableGameMode()
        {
            if (_isEnable == true) return;
            SubscribeToEventBus();
            _controller.SetDoesRestartGameMode(true);
            _controller.TransitionToStart();
            _isEnable = true;
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
            EnableGameMode();
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
        }

        private void UIView_OnMainMenuButtonPressedHandler()
        {
            _motor.SetDoesRestartGameMode(false);
            _controller.TransitionToDisable();
        }

        private void UIView_OnRestartButtonPressedHandler()
        {
            _controller.TransitionToRestart();
        }

        #endregion
        
        #region EventBus

        private void SubscribeToEventBus()
        {
            GameEventCaller.Subscribe<GameModeEvents.Finish>(EventBus_GameMode_Finished);
            GameEventCaller.Subscribe<GameModeEvents.SetPause>(EventBus_GameMode_SetPaused);
            GameEventCaller.Subscribe<ProjectileEvents.Deflected>(EventBus_Meteor_Deflected);
            GameEventCaller.Subscribe<EarthEvents.ShakeStart>(EventBus_Earth_ShakeStart);
            GameEventCaller.Subscribe<EarthEvents.DestructionFinished>(EventBus_Earth_DestructionFinished);
            GameEventCaller.Subscribe<EarthEvents.RestartFinished>(EventBus_Earth_RestartFinish);
            GameEventCaller.Subscribe<AbilitiesEvents.SetActive>(EventBus_Abilities_SetActive);
            GameEventCaller.Subscribe<ProjectileEvents.RequestSpawn>(EventBus_Projectile_RequestSpawn);
        }
        
        private void UnsubscribeToEventBus()
        {
            GameEventCaller.Unsubscribe<GameModeEvents.Finish>(EventBus_GameMode_Finished);
            GameEventCaller.Unsubscribe<GameModeEvents.SetPause>(EventBus_GameMode_SetPaused);
            GameEventCaller.Unsubscribe<ProjectileEvents.Deflected>(EventBus_Meteor_Deflected);
            GameEventCaller.Unsubscribe<EarthEvents.ShakeStart>(EventBus_Earth_ShakeStart);
            GameEventCaller.Unsubscribe<EarthEvents.DestructionFinished>(EventBus_Earth_DestructionFinished);
            GameEventCaller.Unsubscribe<EarthEvents.RestartFinished>(EventBus_Earth_RestartFinish);
            GameEventCaller.Unsubscribe<AbilitiesEvents.SetActive>(EventBus_Abilities_SetActive);
            GameEventCaller.Unsubscribe<ProjectileEvents.RequestSpawn>(EventBus_Projectile_RequestSpawn);
        }

        private void EventBus_GameScreen_SetScreen(GameScreenEvents.SetScreen input)
        {
            if (input.ScreenType == ScreenType.GameMode &&
                input.IsEnable == true)
            {
                _controller.TransitionToEnable();
            }
            else
            {
                DisableGameMode();
            }
        }

        private void EventBus_Projectile_RequestSpawn(ProjectileEvents.RequestSpawn input)
        {
            if(input.RequestType == EventRequestType.Request)
            {
                _controller.GrantProjectileSpawn((int)input.ProjectileType);
            }
        }

        private void EventBus_Abilities_SetActive(AbilitiesEvents.SetActive inputs)
        {
            if (inputs.AbilityType == AbilityType.DoublePoints)
            {
                _controller.SetDoublePoints(inputs.IsActive);
            }
        }

        private void EventBus_GameMode_SetPaused(GameModeEvents.SetPause input)
        {
            _controller.SetGamePause(input.IsPaused);
        }

        private void EventBus_Earth_RestartFinish(EarthEvents.RestartFinished input)
        {
            _controller.EarthRestartFinish();
        }

        private void EventBus_GameMode_Finished(GameModeEvents.Finish input)
        {
            _controller.TransitionToFinish();
        }

        private void EventBus_Meteor_Deflected(ProjectileEvents.Deflected input)
        {
            _controller.HandleProjectileDeflect(input.Position,input.Value);
        }

        private void EventBus_Earth_DestructionFinished(EarthEvents.DestructionFinished destructionFinished)
        {
            _controller.TransitionToDeath();
        }

        private void EventBus_Earth_ShakeStart(EarthEvents.ShakeStart shakeStart)
        {
            _controller.HandleEarthShake();
        }

        #endregion

    }
}