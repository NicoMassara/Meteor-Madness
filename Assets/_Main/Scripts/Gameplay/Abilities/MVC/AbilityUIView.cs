using _Main.Scripts.Gameplay.Abilities;
using _Main.Scripts.Observer;
using _Main.Scripts.ViewUI;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityUIView : BaseViewUI<AbilityUiPanelSelector,AbilityUIComponents>, 
        IObserver,
        AbilityUIView.IAbilityUIView
    {
        public interface IAbilityUIView
        {
            
        }
        
        [SerializeField] private AbilityUIData abilityUIData;
        
        public override void OnNotify(ulong message, params object[] args)
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