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
        private readonly Timer _spawnTimer = new Timer();
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;


        private void Awake()
        {
            _spawnTimer.OnEnd += SpawnTimer_OnEndHandler;
        }

        private void Start()
        {
            StartCoroutine(Test());
        }

        public void ManagedUpdate()
        {

        }
        
        private void SpawnTimer_OnEndHandler()
        {
            Debug.Log("Ability Sent");
            
            GameManager.Instance.EventManager.Publish(new AddAbility{AbilityType = GetAbilityToAdd()});
            
            StartCoroutine(Test());
        }

        private AbilityType GetAbilityToAdd()
        {
            return (AbilityType)UnityEngine.Random.Range(1, Enum.GetValues(typeof(AbilityType)).Length-1);
        }

        private IEnumerator Test()
        {
            _spawnTimer.Set(spawnDelay);
            
            while (_spawnTimer.GetHasEnded == false)
            {
                _spawnTimer.Run(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
                Debug.Log($"Timer: {_spawnTimer.CurrentTime}, Ratio: {_spawnTimer.CurrentRatio}");
                
                yield return null;
            }
        }
    }
}