using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Managers;
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
            
            eventManager.Subscribe<ShieldEvents.SetGold>(EventBus_Shield_SetGold);
            eventManager.Subscribe<MeteorEvents.Deflected>(EventBus_Meteor_Deflected);
            eventManager.Subscribe<ShieldEvents.SetEnable>(EventBus_Shield_SetEnable);
            eventManager.Subscribe<ShieldEvents.EnableSuperShield>(EventBus_Shield_EnableSuperShield);
            eventManager.Subscribe<ShieldEvents.EnableNormalShield>(EventBus_Shield_EnableNormalShield);
            eventManager.Subscribe<GameModeEvents.Disable>(EventBus_GameMode_Disable);
        }

        private void EventBus_Shield_SetGold(ShieldEvents.SetGold input)
        {
            _controller.SetActiveGold(input.IsActive);
        }

        private void EventBus_GameMode_Disable(GameModeEvents.Disable input)
        {
            _controller.TransitionToUnactive();
        }

        private void EventBus_Shield_EnableNormalShield(ShieldEvents.EnableNormalShield input)
        {
            _controller.TransitionToActive();
        }

        private void EventBus_Shield_EnableSuperShield(ShieldEvents.EnableSuperShield input)
        {
            _controller.TransitionToSuper();
        }

        private void EventBus_Meteor_Deflected(MeteorEvents.Deflected input)
        {
            _controller.HandleHit(input.Position, input.Rotation,input.Direction);
        }

        private void EventBus_Shield_SetEnable(ShieldEvents.SetEnable input)
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