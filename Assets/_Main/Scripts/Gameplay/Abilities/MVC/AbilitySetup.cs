using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilies
{
    [RequireComponent(typeof(AbilityView))]
    [RequireComponent(typeof(AbilityUIView))]
    public class AbilitySetup : ManagedBehavior
    {
        private AbilityMotor _motor;
        private AbilityController _controller;
        private IInputReader _inputReader;
        
        private AbilityView _view;
        private AbilityUIView _ui;
        

        private void Awake()
        {
            _motor = new AbilityMotor(GameParameters.GameplayValues.MaxAbilityCount);
            _controller = new AbilityController(_motor);
            
            _view = GetComponent<AbilityView>();
            _ui = GetComponent<AbilityUIView>();
            
            _motor.Subscribe(_view);
            _motor.Subscribe(_ui);
            
            SetViewHandlers();
            EventBusSetup();
        }

        private void Start()
        {
            _controller.Initialize();
            _inputReader = GameManager.Instance.InputReader;

            if (_inputReader != null)
            {
                _inputReader.OnAbilityTriggered += (isPressed) =>
                {
                    if (isPressed)
                    {
                        _controller.SelectAbility();
                    }
                };
            }
        }

        #region View Handlers

        private void SetViewHandlers()
        {
            _view.OnAbilityFinished += View_OnAbilityFinishedHandler;
            _view.OnAbilitySelected += View_OnAbilitySelectedHandler;
        }

        private void View_OnAbilitySelectedHandler()
        {
            _controller.TransitionToRunning();
        }

        private void View_OnAbilityFinishedHandler()
        {
            _controller.TransitionToEnable();
        }

        #endregion

        #region Event Bus

        private void EventBusSetup()
        {
            GameEventCaller.Subscribe<AbilitiesEvents.Disable>(EventBus_Ability_Disable);
            GameEventCaller.Subscribe<AbilitiesEvents.Enable>(EventBus_Ability_Enable);
            GameEventCaller.Subscribe<AbilitiesEvents.SetEnableUI>(EventBus_Ability_SetEnableUI);
            GameEventCaller.Subscribe<AbilitiesEvents.SetCanUse>(EventBus_Ability_CanUse);
            GameEventCaller.Subscribe<AbilitiesEvents.Add>(EventBus_Ability_Add);
            GameEventCaller.Subscribe<AbilitiesEvents.RunTimer>(EventBus_Ability_RunTimer);
        }

        #region Ability

        private void EventBus_Ability_Enable(AbilitiesEvents.Enable input)
        {
            _controller.TransitionToEnable();
        }

        private void EventBus_Ability_Disable(AbilitiesEvents.Disable input)
        {
            _controller.TransitionToDisable();
        }
        
        private void EventBus_Ability_SetEnableUI(AbilitiesEvents.SetEnableUI input)
        {
            _controller.SetEnableUI(input.IsEnable);
        }
        
        private void EventBus_Ability_Add(AbilitiesEvents.Add input)
        {
            _controller.TryAddAbility((int)input.AbilityType, input.Position);
        }
        
        private void EventBus_Ability_CanUse(AbilitiesEvents.SetCanUse input)
        {
            _controller.SetCanUse(input.CanUse);
        }
        
        private void EventBus_Ability_RunTimer(AbilitiesEvents.RunTimer input)
        {
            _controller.RunActiveTimer();
        }

        #endregion

        #endregion
    }
}