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
            eventBus.Subscribe<EarthDeath>(EventBus_OnEarthDeath);
            eventBus.Subscribe<EarthDestruction>(EventBus_OnEarthDestruction);
            eventBus.Subscribe<EarthShake>(EventBus_OnEarthShake);
        }

        private void EventBus_OnEarthDeath(EarthDeath input)
        {
            _controller.HandleEarthDeath();
        }

        private void EventBus_OnMeteorDeflected(MeteorDeflected input)
        {
            _controller.HandleMeteorDeflect();
        }

        private void EventBus_OnEarthDestruction(EarthDestruction input)
        {
            _controller.HandleEarthDestruction();
        }

        private void EventBus_OnEarthShake(EarthShake input)
        {
            _controller.HandleEarthShake();
        }

        #endregion
    }
}