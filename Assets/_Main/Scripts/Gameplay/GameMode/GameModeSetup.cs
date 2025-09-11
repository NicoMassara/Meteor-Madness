using System;
using _Main.Scripts.Managers;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    [RequireComponent(typeof(GameModeView))]
    [RequireComponent(typeof(GameModeUIView))]
    public class GameModeSetup : MonoBehaviour
    {
        private GameModeMotor _motor;
        private GameModeController _controller;
        
        private GameModeView _view;
        private GameModeUIView _ui;
        
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

            ControllerEventSetup();
        }

        private void Start()
        {
            _controller.Initialize();
        }

        private void Update()
        {
            _controller?.Execute();
            _motor?.Execute();
        }
        
        #region EventBus

        private void ControllerEventSetup()
        {
            var eventBus = GameManager.Instance.EventManager;
            //Add events
            
            //Meteor
            eventBus.Subscribe<MeteorDeflected>(EventBus_OnMeteorDeflected);
            
            //Earth
            eventBus.Subscribe<EarthShake>(EventBus_OnEarthShake);
            eventBus.Subscribe<EarthEndDestruction>(EventBus_OnEarthDestruction);
            eventBus.Subscribe<GameFinished>(EventBus_OnGameFinished);
        }

        private void EventBus_OnGameFinished(GameFinished input)
        {
            _controller.TransitionToFinish();
        }

        private void EventBus_OnMeteorDeflected(MeteorDeflected input)
        {
            _controller.HandleMeteorDeflect();
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