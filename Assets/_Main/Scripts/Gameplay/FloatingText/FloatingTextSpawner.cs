using System;
using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.FloatingScore
{
    public class FloatingTextSpawner : ManagedBehavior
    {
        [SerializeField] private FloatingTextBehaviour prefab;

        private FloatingScoreFactory _factory;

        private void Awake()
        {
            _factory = new FloatingScoreFactory(prefab);
            SetEventBus();
        }

        private void SpawnPoints(Vector2 position, int score, bool isDouble)
        {
            var temp = _factory.Get();
            temp.SetValues(new FloatingTextValues
            {
                Position = position,
                Text = $"+{score}",
                Color = isDouble ? Color.yellow : Color.white,
            });
        }

        private void SpawnAbility(Vector2 position, AbilityType ability)
        {
            var temp = _factory.Get();
            var newColor = ability switch
            {
                AbilityType.SuperShield => Color.cyan,
                AbilityType.SlowMotion => Color.green,
                AbilityType.Health => Color.red,
                AbilityType.DoublePoints => Color.yellow,
                _ => Color.clear
            };
            
            temp.SetValues(new FloatingTextValues
            {
                Position = position,
                Text = ability.ToString(),
                Color = newColor,
            });
        }

        #region Event Bus

        private void SetEventBus()
        {
            var eventManager = GameManager.Instance.EventManager;
            eventManager.Subscribe<FloatingTextEvents.Points>(EventBus_FloatingText_Points);
            eventManager.Subscribe<FloatingTextEvents.Ability>(EventBus_FloatingText_Ability);
        }

        private void EventBus_FloatingText_Ability(FloatingTextEvents.Ability input)
        {
            SpawnAbility(input.Position, input.AbilityType);
        }

        private void EventBus_FloatingText_Points(FloatingTextEvents.Points input)
        {
            SpawnPoints(input.Position, input.Score, input.IsDouble);
        }

        #endregion
    }
}