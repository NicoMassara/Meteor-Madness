using MeteorMadness.GlobalValues;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities
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
                case AbilityUIObserverMessage.Add:
                    HandleAddAbility((int)args[0]);
                    break;
                case AbilityUIObserverMessage.Select:
                    HandleSelectAbility();
                    break;
                case AbilityUIObserverMessage.Restart:
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