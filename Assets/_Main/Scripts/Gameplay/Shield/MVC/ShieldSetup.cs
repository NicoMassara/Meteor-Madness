using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
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
        public TickGroup SelfTickGroup { get; } = TickGroup.FullTick;
        public float LastUpdateTime { get; set; }

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

        public void ExecuteUpdate()
        {
            _controller?.Execute(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
        }

        #region EventBus

        private void SetEventBus()
        {
            GameEventCaller.Subscribe<ShieldEvents.Disable>(EventBus_Shield_Disable);
            GameEventCaller.Subscribe<ShieldEvents.Enable>(EventBus_Shield_Enable);
            GameEventCaller.Subscribe<ShieldEvents.SetGold>(EventBus_Shield_SetGold);
            GameEventCaller.Subscribe<ShieldEvents.SetAutomatic>(EventBus_Shield_SetAutomatic);
            GameEventCaller.Subscribe<ShieldEvents.SetSlow>(EventBus_Shield_SetSlow);
            GameEventCaller.Subscribe<ShieldEvents.RestartPosition>(EventBus_Shield_RestartPosition);
            GameEventCaller.Subscribe<ShieldEvents.EnableSuperShield>(EventBus_Shield_EnableSuperShield);
            GameEventCaller.Subscribe<ShieldEvents.EnableNormalShield>(EventBus_Shield_EnableNormalShield);
            //
            GameEventCaller.Subscribe<ProjectileEvents.Deflected>(EventBus_Meteor_Deflected);
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