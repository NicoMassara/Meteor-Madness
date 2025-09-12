using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Managers.UpdateManager.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Main.Scripts.Gameplay.Shield
{
    [RequireComponent(typeof(ShieldView))]
    public class ShieldSetup : ManagedBehavior, IUpdatable
    {
        [SerializeField] private InputReader inputReader;
        private ShieldMotor _motor;
        private ShieldController _controller;
        private ShieldView _view;
        
        public UpdateManager.UpdateGroup UpdateGroup { get; } = UpdateManager.UpdateGroup.Inputs;
        
        private void Awake()
        {
            _view = GetComponent<ShieldView>();

            _motor = new ShieldMotor();
            _controller = new ShieldController(_motor);
            
            _motor.Subscribe(_view);
            
            SetEventBus();
        }

        private void Start()
        {
            _controller.Initialize();
        }
        
        public void ManagedUpdate()
        {
            if (inputReader != null && GameManager.Instance.CanPlay)
            {
                HandleInputs();
            }

            _controller?.Execute();
        }

        private void HandleInputs()
        {
            if (inputReader.MovementDirection != 0)
            {
                _controller.Rotate(inputReader.MovementDirection);
            }
            else
            {
                _controller.StopRotate();
            }
        }

        #region EventBus

        private void SetEventBus()
        {
            var eventManager = GameManager.Instance.EventManager;
            
            eventManager.Subscribe<MeteorDeflected>(EventBus_ShieldDeflection);
            eventManager.Subscribe<ShieldEnable>(EventBus_OnEarthShake);
        }

        private void EventBus_ShieldDeflection(MeteorDeflected input)
        {
            _controller.HandleHit(input.Position);
        }

        private void EventBus_OnEarthShake(ShieldEnable input)
        {
            if (input.IsEnabled)
            {
                _controller.TransitionToActive();
            }
            else
            {
                _controller.TransitionToUnactive();
            }
        }

        #endregion
    }
}