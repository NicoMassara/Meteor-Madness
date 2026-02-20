using System;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities
{
    public class AbilityUIView : BaseViewUI<AbilityUiPanelSelector,AbilityUIComponents>, 
        IObserver,
        IAbilityUIView
    {
        [SerializeField] private AbilityUIData abilityUIData;
        
        public event Action OnTriggerButtonPressed;

        private void Start()
        {
            UIComponents.AddListenerToTriggerButton(OnTriggerButtonPressedHandler);
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case AbilityObserverMessage.AddAbility:
                    HandleAddAbility((int)args[0]);
                    break;
                case AbilityObserverMessage.RestartValues:
                    HandleRestartAbilities();
                    break;
                case AbilityObserverMessage.TriggerAbility:
                    HandleTriggerAbilityI();
                    break;
            }
        }

        private void HandleRestartAbilities()
        {
            abilityUIData.RestartValues();
        }

        private void HandleAddAbility(int abilityTypeIndex)
        {
            abilityUIData.AddAbility((AbilityType)abilityTypeIndex);
        }
        
        private void HandleTriggerAbilityI()
        {
            abilityUIData.RemoveAbility();
        }
        
        private void OnTriggerButtonPressedHandler()
        {
            OnTriggerButtonPressed?.Invoke();
        }
    }
}