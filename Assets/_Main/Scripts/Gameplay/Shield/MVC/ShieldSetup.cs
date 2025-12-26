using MeteorMadness.Gameplay.Shield;
using MeteorMadness.Managers;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.Gameplay.Shield
{
    [RequireComponent(typeof(ShieldView))]
    public class ShieldSetup : ManagedBehavior, IUpdatable
    {
        private ShieldMotor _motor;
        private ShieldController.IShieldController _controller;
        private ShieldView _view;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

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
            GameManager.Instance.InputReader.OnMovementDirectionChanged += Input_OnMovementDirectionChangedHandler;
            //GameManager.Instance.InputReader.OnStopMovement += Input_OnStopMovementHandler;
        }

        private void Input_OnStopMovementHandler()
        {
            if (GameManager.Instance.CanPlay == false) return;
            
            _controller.TryStop();
        }
        
        private void Input_OnMovementDirectionChangedHandler(int direction)
        {
            if (GameManager.Instance.CanPlay == false) return;
            
            _controller.TryRotate(direction);
        }

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