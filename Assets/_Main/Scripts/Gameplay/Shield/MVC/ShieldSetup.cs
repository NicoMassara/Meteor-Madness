﻿using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    [RequireComponent(typeof(ShieldView))]
    public class ShieldSetup : ManagedBehavior, IUpdatable
    {
        [SerializeField] private InputReader inputReader;
        private ShieldMotor _motor;
        private ShieldController _controller;
        private ShieldView _view;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        
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
            inputReader.OnMovementDirectionChanged += Input_OnMovementDirectionChangedHandler;
            inputReader.OnStopMovement += Input_OnStopMovementHandler;
        }

        private void Input_OnStopMovementHandler()
        {
            if (GameManager.Instance.CanPlay == false) return;
            
            _controller.TryStopRotate();
        }
        
        private void Input_OnMovementDirectionChangedHandler(int direction)
        {
            if (GameManager.Instance.CanPlay == false) return;
            
            _controller.TryRotate(direction);
        }

        public void ManagedUpdate()
        {
            _controller?.Execute(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
        }

        #region EventBus

        private void SetEventBus()
        {
            var eventManager = GameManager.Instance.EventManager;
            
            eventManager.Subscribe<MeteorEvents.Deflected>(EventBus_ShieldDeflection);
            eventManager.Subscribe<ShieldEvents.SetEnable>(EventBus_OnSetEnable);
            eventManager.Subscribe<ShieldEvents.EnableSuperShield>(EventBus_OnEnableSuperShield);
            eventManager.Subscribe<ShieldEvents.EnableNormalShield>(EventBus_OnEnableNormalShield);
            eventManager.Subscribe<GameModeEvents.SetEnable>(EventBus_OnGameModeEnable);
        }

        private void EventBus_OnGameModeEnable(GameModeEvents.SetEnable input)
        {
            if (input.IsEnabled)
            {
                
            }
            else
            {
                _controller.TransitionToUnactive();
            }
        }

        private void EventBus_OnEnableNormalShield(ShieldEvents.EnableNormalShield input)
        {
            _controller.TransitionToActive();
        }

        private void EventBus_OnEnableSuperShield(ShieldEvents.EnableSuperShield input)
        {
            _controller.TransitionToSuper();
        }

        private void EventBus_ShieldDeflection(MeteorEvents.Deflected input)
        {
            _controller.HandleHit(input.Position, input.Rotation,input.Direction);
        }

        private void EventBus_OnSetEnable(ShieldEvents.SetEnable input)
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