using System;
using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Managers;
using UnityEngine;

namespace _Main.Scripts
{
    public class InputTester : MonoBehaviour
    {
        [SerializeField] private AbilityType abilityToAdd;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                AddAbility();
            }
        }

        private void Kill()
        {
            for (int i = 0; i < 10; i++)
            {
                GameManager.Instance.EventManager.Publish(new MeteorEvents.Collision());
            }
        }
        
        private void AddAbility()
        {
            GameManager.Instance.EventManager.Publish(new AbilitiesEvents.Add{AbilityType = abilityToAdd});
        }
    }
}