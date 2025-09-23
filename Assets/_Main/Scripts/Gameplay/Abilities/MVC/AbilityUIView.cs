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

        public void OnNotify(string message, params object[] args)
        {
            switch (message)
            {
                case AbilityObserverMessage.AddAbility:
                    HandleAddAbility((AbilityType)args[0]);
                    break;
                
                case AbilityObserverMessage.SelectAbility:
                    HandleSelectAbility((AbilityType)args[0]);
                    break;
            }
        }

        private void HandleAddAbility(AbilityType abilityType)
        {
            Debug.Log($"Ability Added: {abilityType}");
            abilityUIData.AddAbility(abilityType);
        }
        
        private void HandleSelectAbility(AbilityType abilityType)
        {
            abilityUIData.RemoveAbility();
        }
    }
}