using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using UnityEngine;
using UnityEngine.Events;

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
        
        //Hack
        private bool _isDisable;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        
        private void Awake()
        {
            _motor = new GameModeMotor();
            _controller = new GameModeController(_motor);
            
            _view = GetComponent<GameModeView>();
            _ui = GetComponent<GameModeUIView>();
            
            _motor.Subscribe(_view);
            _motor.Subscribe(_ui);

            SetViewHandlers();
            SetUIViewHandlers();
            SetEventBus();
        }

        private void Start()
        {
            _controller.Initialize();
        }
        
        public void ManagedUpdate()
        {
            _controller?.Execute(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
        }

        #region View Handlers

        private void SetViewHandlers()
        {
            _view.OnEarthRestarted += View_OnEarthRestartedHandler;
            _view.OnCountdownFinished += View_OnCountdownFinishedHandler;
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

        private void SetEventBus()
        {
            var eventBus = GameManager.Instance.EventManager;
            //Add events
            
            //Game
            eventBus.Subscribe<GameModeEvents.Finish>(EventBus_OnGameFinished);
            eventBus.Subscribe<GameModeEvents.SetPause>(EventBus_OnGamePaused);
            eventBus.Subscribe<GameScreenEvents.MainMenuEnable>(EventBus_OnMainMenu);
            eventBus.Subscribe<GameScreenEvents.GameModeEnable>(EventBus_OnGameModeScreenEnable);
            
            //Meteor
            eventBus.Subscribe<MeteorEvents.Deflected>(EventBus_OnMeteorDeflected);
            
            //Earth
            eventBus.Subscribe<EarthEvents.ShakeStart>(EventBus_OnEarthShake);
            eventBus.Subscribe<EarthEvents.DestructionFinished>(EventBus_OnEarthDestruction);
            eventBus.Subscribe<EarthEvents.RestartFinished>(EventBus_OnEarthRestartFinish);
            
            //Abilities
            eventBus.Subscribe<AbilitiesEvents.SetActive>(EventBus_Abilities_SetActive);
        }

        private void EventBus_Abilities_SetActive(AbilitiesEvents.SetActive inputs)
        {
            if (inputs.AbilityType == AbilityType.DoublePoints)
            {
                _controller.SetDoublePoints(inputs.IsActive);
            }
        }

        private void EventBus_OnGameModeScreenEnable(GameScreenEvents.GameModeEnable input)
        {
            _controller.SetDoesRestartGameMode(true);
            _controller.TransitionToStart();
        }

        private void EventBus_OnMainMenu(GameScreenEvents.MainMenuEnable input)
        {
            _controller.TransitionToDisable();
        }

        private void EventBus_OnGamePaused(GameModeEvents.SetPause input)
        {
            _controller.SetGamePause(input.IsPaused);
        }

        private void EventBus_OnEarthRestartFinish(EarthEvents.RestartFinished input)
        {
            _controller.EarthRestartFinish();
        }

        private void EventBus_OnGameFinished(GameModeEvents.Finish input)
        {
            _controller.TransitionToFinish();
        }

        private void EventBus_OnMeteorDeflected(MeteorEvents.Deflected input)
        {
            _controller.HandleMeteorDeflect(input.Position,input.Value);
        }

        private void EventBus_OnEarthDestruction(EarthEvents.DestructionFinished destructionFinished)
        {
            _controller.TransitionToDeath();
        }

        private void EventBus_OnEarthShake(EarthEvents.ShakeStart shakeStart)
        {
            _controller.HandleEarthShake();
        }

        #endregion

    }
}