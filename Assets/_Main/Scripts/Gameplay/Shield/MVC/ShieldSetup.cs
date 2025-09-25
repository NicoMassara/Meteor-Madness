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
            
            _controller.StopRotate();
        }
        
        private void Input_OnMovementDirectionChangedHandler(int direction)
        {
            if (GameManager.Instance.CanPlay == false) return;
            
            _controller.Rotate(direction);
        }

        public void ManagedUpdate()
        {
            _controller?.Execute(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
        }

        #region EventBus

        private void SetEventBus()
        {
            var eventManager = GameManager.Instance.EventManager;
            
            eventManager.Subscribe<MeteorDeflected>(EventBus_ShieldDeflection);
            eventManager.Subscribe<ShieldEnable>(EventBus_OnEarthShake);
            eventManager.Subscribe<SetSuperShield>(EventBus_OnSetTotalShield);
            eventManager.Subscribe<SetNormalShield>(EventBus_OnSetNormalShield);
        }

        private void EventBus_OnSetNormalShield(SetNormalShield input)
        {
            _controller.TransitionToActive();
        }


        private void EventBus_OnSetTotalShield(SetSuperShield input)
        {
            _controller.TransitionToSuper();
        }

        private void EventBus_ShieldDeflection(MeteorDeflected input)
        {
            _controller.HandleHit(input.Position, input.Rotation,input.Direction);
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