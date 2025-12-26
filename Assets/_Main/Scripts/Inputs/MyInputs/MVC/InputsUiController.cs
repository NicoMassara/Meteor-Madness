using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.MyInputs.MVC
{
    [RequireComponent(typeof(InputsUiView))]
    public class InputsUiController : ManagedBehavior
    {
#if UNITY_ANDROID
        private InputsUiMotor _motor;
        private InputsUiView _view;
        private bool _isGameplayEnable;
        private ulong _timerId;
#endif

        private void Awake()
        {
#if UNITY_STANDALONE_WIN

            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                Destroy(gameObject);
                return;
            }

#else      
            _view = GetComponent<InputsUiView>();
            
            _motor = new InputsUiMotor();
            _motor.Subscribe(_view);
            
            SetEventBus();
#endif
        }
        
#if UNITY_ANDROID

        private void Start()
        {
            //Input Reader
            GameManager.Instance.InputReader.OnMovementDirectionChanged += Input_OnMovementDirectionChangedHandler;
            GameManager.Instance.InputReader.OnAbilityTriggered += Input_OnAbilityTriggeredHandler;
            _motor.Initialize(GameConfigManager.Instance.GetGameplayData().TouchInputData);
        }

        #region Handlers

        private void Input_OnMovementDirectionChangedHandler(int direction)
        {
            _motor.SetDirection(direction);
        }
        
        private void Input_OnAbilityTriggeredHandler(bool isTriggered)
        {
            _motor.SetHasTriggeredAbility(isTriggered);
        }

        #endregion

        #region EventBus

        private void SetEventBus()
        {
            InputsEventSubscriber.SetUIEnable(EventBus_Inputs_SetUIEnable);
        }

        private void EventBus_Inputs_SetUIEnable(InputsEvents.SetUIEnable input)
        {
            _motor.SetEnable(input.IsEnable);
        }

        #endregion
#endif
    }

#if UNITY_ANDROID
    public class InputsUiMotor : ObservableComponent
    {
        private int _currentDirection;
        private bool _hasTriggeredAbility;

        public void Initialize(ITouchInputData data)
        {
            NotifyAll(InputsUIObserverMessage.Initialize, data);
        }
        
        public void Destroy()
        {
            NotifyAll(InputsUIObserverMessage.Destroy);
        }

        public void SetEnable(bool isEnable)
        {
            NotifyAll(InputsUIObserverMessage.SetEnableUI, isEnable);
        }

        public void SetDirection(int direction)
        {
            _currentDirection = direction;
            
            switch (_currentDirection)
            {
                case 0:
                    NotifyAll(InputsUIObserverMessage.SetEnableClock, false);
                    NotifyAll(InputsUIObserverMessage.SetEnableCounterClock, false);
                    break;
                case -1: // ClockWise
                    NotifyAll(InputsUIObserverMessage.SetEnableClock, true);
                    NotifyAll(InputsUIObserverMessage.SetEnableCounterClock, false);
                    break;
                case 1: // CounterClock
                    NotifyAll(InputsUIObserverMessage.SetEnableClock, false);
                    NotifyAll(InputsUIObserverMessage.SetEnableCounterClock, true);
                    break;
            }
        }

        public void SetHasTriggeredAbility(bool hasTriggeredAbility)
        {
            _hasTriggeredAbility = hasTriggeredAbility;

            if (_hasTriggeredAbility)
            {
                NotifyAll(InputsUIObserverMessage.SetEnableClock, true);
                NotifyAll(InputsUIObserverMessage.SetEnableCounterClock, true);
            }
        }
    }
#endif
}