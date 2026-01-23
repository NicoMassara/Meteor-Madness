using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield;
using MeteorMadness.Managers;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.Gameplay.Shield
{
    [RequireComponent(typeof(ShieldView))]
    public class ShieldSetup : ManagedBehavior, IUpdatable
    {
        [SerializeField] private ShieldInput inputReader;
        private ShieldMotor _motor;
        private ShieldController.IShieldController _controller;
        private ShieldView _view;
        private IInputReader _inputReader;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

        private void Awake()
        {
            _view = GetComponent<ShieldView>();
            _motor = new ShieldMotor();
            _controller = new ShieldController(_motor);
            _inputReader = inputReader.GetInputReader();

            if (_inputReader == null)
            {
                Debug.LogWarning("No IInputReader component found");
            }

            _motor.Subscribe(_view);
            
            SetEventBus();

            BootEvents.OnSubSystemRequestInitialize += Initialize;
        }

        private void Initialize()
        {
            BootEvents.OnSubSystemRequestInitialize -= Initialize;
            _controller.Initialize();
            _inputReader.OnMagnitudeChanged += InputReader_OnMagnitudeChangedHandler;
            _inputReader.OnStopInput += InputReader_OnStopInputHandler;
            _inputReader.OnMoved += InputReader_OnMovedHandler;
            
            BootEvents.SubSystemInitialized();
        }

        #region Handlers

        private void InputReader_OnMagnitudeChangedHandler(float magnitude)
        {
            if (GameManager.Instance.CanPlay == false) return;
            _controller.ChangeInputMagnitude(magnitude);
        }
        
        private void InputReader_OnMovedHandler(float inputAngle)
        {
            if (GameManager.Instance.CanPlay == false) return;
            _controller.TryRotate(inputAngle);
        }

        private void InputReader_OnStopInputHandler()
        {
            if (GameManager.Instance.CanPlay == false) return;

            _controller.StopInput();
        }

        #endregion

        public void ExecuteUpdate(float deltaTime)
        {
            _controller?.Execute(deltaTime);
        }

        #region EventBus

        private void SetEventBus()
        {
            ShieldEventSubscriber.Disable(EventBus_Shield_Disable);
            ShieldEventSubscriber.Enable(EventBus_Shield_Enable);
            ShieldEventSubscriber.RequestEnableShieldType(EventBus_Shield_EnableType);
            ShieldEventSubscriber.RequestDisableShieldType(EventBus_Shield_DisableType);
            //
            ProjectileEventSubscriber.Deflected(EventBus_Meteor_Deflected);
        }
        
        #region Shield

        private void EventBus_Shield_Enable(ShieldEvents.Enable input)
        {
            _controller.TryEnable();
        }
        
        private void EventBus_Shield_Disable(ShieldEvents.Disable obj)
        {
            _controller.TryDisable();
        }
        
        private void EventBus_Shield_EnableType(ShieldEvents.RequestEnableShieldType input)
        {
            _controller.TryEnableType(input.Type);
        }
        
        private void EventBus_Shield_DisableType(ShieldEvents.RequestDisableShieldType input)
        {
            _controller.TryDisableShieldType();
        }

        #endregion

        #region Projectile

        private void EventBus_Meteor_Deflected(ProjectileEvents.Deflected input)
        {
            _controller.TryHit(input.Position, input.Rotation,input.Direction);
        }

        #endregion
        
        #endregion
    }
}