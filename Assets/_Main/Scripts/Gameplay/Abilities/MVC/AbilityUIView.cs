using System;
using _Main.Scripts.Gameplay.Abilities;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityUIView : ManagedBehavior, IObserver
    {
        [SerializeField] private AbilityUIData abilityUIData;
        [SerializeField] private AbilityUiPanelSelector uiSelector;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.UI;

        private void Start()
        {
            GetAbilityUIComponents().MainPanel.SetActive(false);
        }
        
        private AbilityUIComponents GetAbilityUIComponents()
        {
            return uiSelector.GetPanelData();
        }


        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case AbilityObserverMessage.AddAbility:
                    HandleAddAbility((int)args[0]);
                    break;
                case AbilityObserverMessage.SelectAbility:
                    HandleSelectAbility((int)args[0]);
                    break;
                case AbilityObserverMessage.RestartAbilities:
                    HandleRestartAbilities();
                    break;
                case AbilityObserverMessage.SetEnableUI:
                    HandleSetEnableUI((bool)args[0]);
                    break;
            }
        }

        private void HandleSetEnableUI(bool isEnable)
        {
            GetAbilityUIComponents().MainPanel.SetActive(isEnable);
        }

        private void HandleRestartAbilities()
        {
            abilityUIData.RestartValues();
        }

        private void HandleAddAbility(int abilityTypeIndex)
        {
            abilityUIData.AddAbility((AbilityType)abilityTypeIndex);
        }
        
        private void HandleSelectAbility(int abilityTypeIndex)
        {
            abilityUIData.RemoveAbility();
        }
    }
}