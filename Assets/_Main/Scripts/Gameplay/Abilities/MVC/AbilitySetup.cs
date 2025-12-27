using MeteorMadness.GlobalValues.Interfaces;
using MeteorMadness.Managers;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities
{
    [RequireComponent(typeof(AbilityView))]
    public class AbilitySetup : ManagedBehavior
    {
        private AbilityMotor _motor;
        private AbilityController.IAbilityController _controller;
        private IInputReader _inputReader;
        
        private AbilityView.IAbilityView _view;
        
        private void Awake()
        {
            _motor = new AbilityMotor();
            _controller = new AbilityController(_motor);
            
            var view = GetComponent<AbilityView>();
            
            _motor.Subscribe(view);
            
            _view = view;
            
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
            _view.OnAbilityFinished += _controller.TryEnableAbility;
            _view.OnAbilitySelected += _controller.TryTriggerAbility;
        }

        #endregion

        #region Event Bus

        private void EventBusSetup()
        {
            AbilitiesEventSubscriber.Enable(EventBus_Ability_Enable);
            AbilitiesEventSubscriber.Disable(EventBus_Ability_Disable);
            AbilitiesEventSubscriber.EnableUI(EventBus_Ability_UI_Enable);
            AbilitiesEventSubscriber.DisableUI(EventBus_Ability_UI_Disable);
            AbilitiesEventSubscriber.SetCanUse(EventBus_Ability_CanUse);
            AbilitiesEventSubscriber.Add(EventBus_Ability_Add);
            AbilitiesEventSubscriber.RunTimer(EventBus_Ability_RunTimer);
            //
        }
        
        #region Ability

        private void EventBus_Ability_Enable(AbilitiesEvents.Enable input)
        {
            _controller.TryEnableAbility();
        }

        private void EventBus_Ability_Disable(AbilitiesEvents.Disable input)
        {
            _controller.TryDisableAbility();
        }
        
        private void EventBus_Ability_UI_Enable(AbilitiesEvents.EnableUI input)
        {
            _controller.TryEnableUI();
        }
        
        private void EventBus_Ability_UI_Disable(AbilitiesEvents.DisableUI input)
        {
            _controller.TryDisableUI();
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