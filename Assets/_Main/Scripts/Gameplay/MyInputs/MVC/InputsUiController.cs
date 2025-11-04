using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.MyInputs.MVC
{
    [RequireComponent(typeof(InputsUiView))]
    public class InputsUiController : ManagedBehavior
    {
        private InputsUiView _view;
        private bool _isGameplayEnable;
        private ulong _timerId;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.UI;

        private void Awake()
        {
            if (SystemInfo.deviceType != DeviceType.Handheld)
            {
                Destroy(gameObject);
                return;
            }

            _view = GetComponent<InputsUiView>();
            
            SetEventBus();
        }

        private void Start()
        {
            //Input Reader
            GameManager.Instance.InputReader.OnStopMovement += Input_OnStopMovementHandler;
            GameManager.Instance.InputReader.OnMovementDirectionChanged += Input_OnMovementDirectionChangedHandler;
            GameManager.Instance.InputReader.OnAbilityTriggered += Input_OnAbilityTriggeredHandler;
            _view.InitializeImages(GameConfigManager.Instance.GetGameplayData().TouchInputData);
        }

        #region Handlers

        private void Input_OnMovementDirectionChangedHandler(int direction)
        {
            _view.SetCurrentImage(direction);
        }

        private void Input_OnStopMovementHandler()
        {
            _view.DisableActiveImage();
        }
        
        private void Input_OnAbilityTriggeredHandler(bool isTriggered)
        {
            if (isTriggered)
            {
                _view.SetActiveBothImages();
            }
            else
            {
                _view.SetInactiveBothImages();
            }
        }

        #endregion

        #region EventBus

        private void SetEventBus()
        {
            GameEventCaller.Subscribe<InputsEvents.SetEnable>(EventBus_Inputs_SetEnable);
        }

        private void EventBus_Inputs_SetEnable(InputsEvents.SetEnable input)
        {
            _view.SetEnablePanel(input.IsEnable);
        }

        #endregion
    }
}