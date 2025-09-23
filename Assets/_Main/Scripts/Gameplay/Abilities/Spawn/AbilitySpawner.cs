using System;
using System.Collections;
using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilities.Spawn
{
    public class AbilitySpawner : ManagedBehavior, IUpdatable
    {
        [Range(5, 15f)] 
        [SerializeField] private float spawnDelay = 5f;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;

        private void Start()
        {
            SetEventBus();
            
            SetTimer();
        }

        public void ManagedUpdate()
        {

        }
        
        private void SendAbility()
        {
            Debug.Log("Ability Sent");
            
            GameManager.Instance.EventManager.Publish(new AddAbility{AbilityType = GetAbilityToAdd()});
            
            SetTimer();
        }

        private void SetTimer()
        {
            TimerManager.AddTimer(new TimerData
            {
                Time = spawnDelay,
                OnEndAction = SendAbility
            }, SelfUpdateGroup);
        }

        private AbilityType GetAbilityToAdd()
        {
            return (AbilityType)UnityEngine.Random.Range(1, Enum.GetValues(typeof(AbilityType)).Length-1);
        }

        #region EventBus

        private void SetEventBus()
        {
            GameManager.Instance.EventManager.Subscribe<GameFinished>(EventBus_OnGameFinished);
        }

        private void EventBus_OnGameFinished(GameFinished input)
        {
            
        }

        #endregion
    }
}