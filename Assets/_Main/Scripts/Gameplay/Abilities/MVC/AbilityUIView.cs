using _Main.Scripts.Gameplay.Abilities;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityUIView : ManagedBehavior, IObserver
    {
        [SerializeField] private AbilityUIData abilityUIData;
        [SerializeField] private AbilityUiPanelSelector uiSelector;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.UI;
        
        private AbilityUIComponents GetUIComponents()
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
                    HandleSelectAbility();
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
            if (isEnable)
            {
                GetUIComponents().SetActivePanel(GetUIComponents().MainPanel);
            }
            else
            {
                GetUIComponents().DisableActivePanel();
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
        
        private void HandleSelectAbility()
        {
            abilityUIData.RemoveAbility();
        }
    }
}