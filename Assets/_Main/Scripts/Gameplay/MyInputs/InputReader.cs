using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using System;
using NicolasMassara.CustomUpdateManager;

namespace _Main.Scripts.Gameplay.MyInputs
{
    public class InputReader : ManagedBehavior, IInputReader, IUpdatable
    {
        private IInput input;

        private int _rotateDirection;
        private bool _areInputsEnable;
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private InputsDebugData _debugData;
        private LateAbilityUpdater _lateAbilityUpdater;
        
        private class LateAbilityUpdater : ManagedComponent, ILateUpdatable
        {
            public bool HasUsedAbility { get; private set; }
            public bool AreInputsEnabled { get; set; }
            public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Inputs;
            public TickGroup SelfTickGroup { get; } = TickGroup.EightTarget;
            
            public void ExecuteLateUpdate(float deltaTime)
            {
                if (AreInputsEnabled || HasUsedAbility)
                {
                    HasUsedAbility = false;
                }
            }
        }
#endif
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Inputs;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;
        
        public event Action<int> OnMovementDirectionChanged;
        public event Action OnStopMovement;
        public event Action<bool> OnAbilityTriggered;
        
        
        private void Awake()
        {
            GameEventCaller.Subscribe<InputsEvents.SetEnable>(EventBus_Inputs_SetEnable);
            GameManager.Instance.SetInputReader(this);
        }

        private void Start()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData = new InputsDebugData();
            _lateAbilityUpdater = new LateAbilityUpdater();
#endif
            
#if UNITY_ANDROID || UNITY_IOS
            input = new TouchInput();
#else
            input = new KeyInput();
#endif
        }

        public void ExecuteUpdate(float deltaTime)
        {
            if(_areInputsEnable == false) return;
            
            if (_rotateDirection != 0)
            {
                OnMovementDirectionChanged?.Invoke(_rotateDirection);
            }
        }

        private void EnableInputs()
        {
            if(_areInputsEnable == true) return;

            input.Enable();
            
            input.OnTriggerAbility += TriggerAbility;
            input.OnUpdateDirection += UpdateDirection;
            input.OnPaused += TriggerPause;
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.HorizontalAxis = 0;
            _lateAbilityUpdater.AreInputsEnabled = _areInputsEnable;
#endif

            _areInputsEnable = true;
    
        }

        private void DisableInputs()
        {
            if(_areInputsEnable == false) return;
            
            input.OnTriggerAbility -= TriggerAbility;
            input.OnUpdateDirection -= UpdateDirection;
            input.OnPaused -= TriggerPause;
            
            input.Disable();
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.HorizontalAxis = 0;
            _lateAbilityUpdater.AreInputsEnabled = _areInputsEnable;
#endif
            
            _areInputsEnable = false;

        }

        #region Actions

        private void TriggerPause()
        {
            GameManager.Instance.EventManager.Publish(
                new GameModeEvents.SetPause{IsPaused = !GameManager.Instance.IsPaused});
        }
        
        private void TriggerAbility(bool isPressed)
        {
            OnAbilityTriggered?.Invoke(isPressed);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.TriggerAbility = _lateAbilityUpdater.HasUsedAbility;
            
#endif
        }

        private void UpdateDirection(int direction)
        {
            _rotateDirection = direction;
            
            if (_rotateDirection == 0)
            {
                OnStopMovement?.Invoke();
            }
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.HorizontalAxis = _rotateDirection;
#endif
        }

        #endregion
        


        private void EventBus_Inputs_SetEnable(InputsEvents.SetEnable input)
        {
            if (input.IsEnable)
            {
                EnableInputs();
            }
            else
            {
                UpdateDirection(0);
                DisableInputs();
            }
        }
    }
}