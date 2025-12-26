using System;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers;
using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities
{
    [RequireComponent(typeof(AbilityUIView))]
    [RequireComponent(typeof(AbilityViewAnimation))]
    public class AbilityUISetup : MonoBehaviour
    {
        #region Motor

        private class AbilityUIMotor : ObservableComponent
        {
            public void Initialize() => NotifyAll(AbilityUIObserverMessage.Initialize);
            public void Enable() => NotifyAll(AbilityUIObserverMessage.Enable);
            public void Disable() => NotifyAll(AbilityUIObserverMessage.Disable);
            public void Restart() => NotifyAll(AbilityUIObserverMessage.Restart);
            public void Add(int inputAbilityIndex) => NotifyAll(AbilityUIObserverMessage.Add,inputAbilityIndex);
            public void Select() => NotifyAll(AbilityUIObserverMessage.Select);
        }

        #endregion

        //Todo: Implemente Ability MVC from Gameplay with This
        
        private AbilityUIMotor _motor;
        private AbilityUIView.IAbilityUIView _ui;
        private AbilityViewAnimation.IAbilityViewAnimation _animation;

        private void Awake()
        {
            var ui = GetComponent<AbilityUIView>();
            var anim = GetComponent<AbilityViewAnimation>();
            
            _motor = new AbilityUIMotor();
            _motor.Subscribe(ui);
            _motor.Subscribe(anim);
            
            _ui = ui;
            _animation = anim;
            EventBusSetup();
        }

        private void EventBusSetup()
        {
            AbilitiesUISubscriber.Add(EventBus_AbilitiesUI_Add);
            AbilitiesUISubscriber.SelectAbility(EventBus_AbilitiesUI_Select);
            AbilitiesUISubscriber.Initialize(EventBus_AbilitiesUI_Initialize);
            AbilitiesUISubscriber.Restart(EventBus_AbilitiesUI_Restart);
            AbilitiesUISubscriber.EnableUI(EventBus_AbilitiesUI_Enable);
            AbilitiesUISubscriber.DisableUI(EventBus_AbilitiesUI_Disable);
        }

        
        private void EventBus_AbilitiesUI_Add(AbilitiesUIEvents.Add input)
        {
            _motor.Add(input.AbilityIndex);
        }
        private void EventBus_AbilitiesUI_Select(AbilitiesUIEvents.SelectAbility input)
        {
            _motor.Select();
        }
        private void EventBus_AbilitiesUI_Initialize(AbilitiesUIEvents.Initialize input)
        {
            _motor.Initialize();
        }
        private void EventBus_AbilitiesUI_Restart(AbilitiesUIEvents.Restart input)
        {
            _motor.Restart();
        }
        private void EventBus_AbilitiesUI_Enable(AbilitiesUIEvents.EnableUI input)
        {
            _motor.Enable();
        }
        private void EventBus_AbilitiesUI_Disable(AbilitiesUIEvents.DisableUI input)
        {
            _motor.Disable();
        }
    }


}