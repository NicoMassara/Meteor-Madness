using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts
{
    public class AbilityAdder : ManagedBehavior, IUpdatable
    {
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Inputs;
        public void ManagedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                AddAbility(AbilityType.SuperShield);
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                AddAbility(AbilityType.SlowMotion);
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                AddAbility(AbilityType.Health);
            }
        }

        private void AddAbility(AbilityType abilityType)
        {
            GameManager.Instance.EventManager.Publish(new AbilitiesEvents.Add{AbilityType = abilityType});
        }
    }
}