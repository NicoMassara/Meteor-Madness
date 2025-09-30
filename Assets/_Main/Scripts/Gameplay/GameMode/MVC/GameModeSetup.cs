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

            _view.SetController(_controller);
            _ui.SetController(_controller);

            SetViewHandlers();
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

        #region ViewHandlers

        private void SetViewHandlers()
        {
            _view.OnEarthRestarted += View_OnEarthRestartedHandler;
        }

        private void View_OnEarthRestartedHandler()
        {
            if (_isDisable == false)
            {
                _controller.TransitionToStart();
            }
        }

        #endregion
        
        #region EventBus

        private void SetEventBus()
        {
            var eventBus = GameManager.Instance.EventManager;
            //Add events
            
            //Game
            eventBus.Subscribe<GameFinished>(EventBus_OnGameFinished);
            eventBus.Subscribe<GamePause>(EventBus_OnGamePaused);
            eventBus.Subscribe<MainMenuScreenEnable>(EventBus_OnMainMenu);
            eventBus.Subscribe<GameModeScreenEnable>(EventBus_OnGameModeScreenEnable);
            
            //Meteor
            eventBus.Subscribe<MeteorDeflected>(EventBus_OnMeteorDeflected);
            
            //Earth
            eventBus.Subscribe<EarthShake>(EventBus_OnEarthShake);
            eventBus.Subscribe<EarthEndDestruction>(EventBus_OnEarthDestruction);
            eventBus.Subscribe<EarthRestartFinish>(EventBus_OnEarthRestartFinish);
        }

        private void EventBus_OnGameModeScreenEnable(GameModeScreenEnable input)
        {
            _isDisable = false;
            _controller.TransitionToStart();
        }

        private void EventBus_OnMainMenu(MainMenuScreenEnable input)
        {
            _isDisable = true;
            _controller.TransitionToDisable();
        }

        private void EventBus_OnGamePaused(GamePause input)
        {
            _controller.SetGamePause(input.IsPaused);
        }

        private void EventBus_OnEarthRestartFinish(EarthRestartFinish input)
        {
            _controller.EarthRestartFinish();
        }

        private void EventBus_OnGameFinished(GameFinished input)
        {
            _controller.TransitionToFinish();
        }

        private void EventBus_OnMeteorDeflected(MeteorDeflected input)
        {
            _controller.HandleMeteorDeflect(input.Value);
        }

        private void EventBus_OnEarthDestruction(EarthEndDestruction input)
        {
            _controller.TransitionToDeath();
        }

        private void EventBus_OnEarthShake(EarthShake input)
        {
            _controller.HandleEarthShake();
        }

        #endregion

    }
}