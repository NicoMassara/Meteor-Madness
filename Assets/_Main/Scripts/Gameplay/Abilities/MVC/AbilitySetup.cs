using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Abilities;
using MeteorMadness.GlobalValues.Tools.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities
{
    [RequireComponent(typeof(AbilityView))]
    public class AbilitySetup : ManagedBehavior
    {
        private AbilityMotor _motor;
        private IAbilityView _view;
        private IAbilityUIView _ui;
        
        private void Awake()
        {
            _motor = new AbilityMotor();
            _view = GetComponent<IAbilityView>();
            _ui = GetComponent<IAbilityUIView>();
            

            _motor.Subscribe(_view);
            _motor.Subscribe((IObserver)_ui);
            
            SetViewHandlers();
            EventBusSetup();
        }
        

        private void SetViewHandlers()
        {
            // View
            _view.OnAbilityFinished += () => _motor.FinishAbility();
        }

        #region Event Bus

        private void EventBusSetup()
        {
            AbilitiesEventSubscriber.Enable(EventBus_Ability_Enable);
            AbilitiesEventSubscriber.Disable(EventBus_Ability_Disable);
            AbilitiesEventSubscriber.SetCanUse(EventBus_Ability_CanUse);
            AbilitiesEventSubscriber.Add(EventBus_Ability_Add);
            AbilitiesEventSubscriber.Trigger(EventBus_Ability_Trigger);
        }

        private void EventBus_Ability_Trigger(AbilitiesEvents.Trigger input)
        {
            _motor.TryTriggerAbility();
        }

        private void EventBus_Ability_Enable(AbilitiesEvents.Enable input)
        {
            _motor.SetEnable(true);
        }

        private void EventBus_Ability_Disable(AbilitiesEvents.Disable input)
        {
            _motor.SetEnable(false);
            _motor.ForceFinishAbility();
            _motor.RestartValues();
        }
        
        private void EventBus_Ability_Add(AbilitiesEvents.Add input)
        {
            _motor.TryAddAbility((int)input.AbilityType, input.Position);
        }
        
        private void EventBus_Ability_CanUse(AbilitiesEvents.SetCanUse input)
        {
            _motor.SetCanUse(input.CanUse);
        }

        #endregion
    }
}