using _Main.Scripts.Gameplay.Abilities;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityUIView : ManagedBehavior, IUpdatable, IObserver
    {
        [SerializeField] private AbilityUIData abilityUIData;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.UI;
        public void ManagedUpdate()
        {

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
        
        private void HandleSelectAbility(int abilityTypeIndex)
        {
            abilityUIData.RemoveAbility();
        }
    }
}