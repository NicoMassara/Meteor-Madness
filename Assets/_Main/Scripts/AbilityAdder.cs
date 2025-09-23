using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
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
                GameManager.Instance.EventManager.Publish(new AddAbility{AbilityType = AbilityType.SuperShield});
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                GameManager.Instance.EventManager.Publish(new AddAbility{AbilityType = AbilityType.SlowMotion});
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                GameManager.Instance.EventManager.Publish(new AddAbility{AbilityType = AbilityType.Health});
            }
        }
    }
}