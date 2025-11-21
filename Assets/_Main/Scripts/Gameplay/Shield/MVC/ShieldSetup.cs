using _Main.Scripts.Managers;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    [RequireComponent(typeof(ShieldView))]
    public class ShieldSetup : ManagedBehavior, IUpdatable
    {
        private ShieldMotor _motor;
        private ShieldController _controller;
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
            GameManager.Instance.InputReader.OnStopMovement += Input_OnStopMovementHandler;
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

        public void ExecuteUpdate(float deltaTime)
        {
            _controller?.Execute(deltaTime);
        }

        #region EventBus

        private void SetEventBus()
        {
            ShieldEventSubscriber.Disable(EventBus_Shield_Disable);
            ShieldEventSubscriber.Enable(EventBus_Shield_Enable);
            ShieldEventSubscriber.SetGold(EventBus_Shield_SetGold);
            ShieldEventSubscriber.SetAutomatic(EventBus_Shield_SetAutomatic);
            ShieldEventSubscriber.SetSlow(EventBus_Shield_SetSlow);
            ShieldEventSubscriber.RestartPosition(EventBus_Shield_RestartPosition);
            ShieldEventSubscriber.EnableSuperShield(EventBus_Shield_EnableSuperShield);
            ShieldEventSubscriber.EnableNormalShield(EventBus_Shield_EnableNormalShield);
            //
            ProjectileEventSubscriber.Deflected(EventBus_Meteor_Deflected);
        }
        
        #region Shield

        private void EventBus_Shield_Enable(ShieldEvents.Enable input)
        {
            _controller.TransitionToEnable();
        }
        
        private void EventBus_Shield_RestartPosition(ShieldEvents.RestartPosition input)
        {
            _controller.RestartPosition();
        }
        
        private void EventBus_Shield_SetAutomatic(ShieldEvents.SetAutomatic input)
        {
            if (input.IsActive)
            {
                _controller.TransitionToAutomatic();
            }
            else
            {
                _controller.TransitionToEnable();
            }
        }
        
        private void EventBus_Shield_SetGold(ShieldEvents.SetGold input)
        {
            if (input.IsActive)
            {
                _controller.TransitionToGold();
            }
            else
            {
                _controller.TransitionToEnable();
            }
        }
        
        private void EventBus_Shield_SetSlow(ShieldEvents.SetSlow input)
        {
            if (input.IsActive)
            {
                _controller.TransitionToSlow();
            }
            else
            {
                _controller.TransitionToEnable();
            }
        }
        
        private void EventBus_Shield_Disable(ShieldEvents.Disable input)
        {
            _controller.TransitionToDisable();
        }
        
        private void EventBus_Shield_EnableNormalShield(ShieldEvents.EnableNormalShield input)
        {
            _controller.TransitionToEnable();
        }

        private void EventBus_Shield_EnableSuperShield(ShieldEvents.EnableSuperShield input)
        {
            _controller.TransitionToSuper();
        }

        #endregion

        #region Projectile

        private void EventBus_Meteor_Deflected(ProjectileEvents.Deflected input)
        {
            _controller.HandleHit(input.Position, input.Rotation,input.Direction);
        }

        #endregion
        
        #endregion
    }
}