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

        private void Spawn(FloatingTextValues data)
        {
            var temp = _factory.Get();
            temp.SetValues(data);
        }

        #region Event Bus

        private void SetEventBus()
        {
            var eventManager = GameManager.Instance.EventManager;
            eventManager.Subscribe<FloatingTextEvents.Spawn>(EventBus_FloatingText_Spawn);
        }

        private void EventBus_FloatingText_Spawn(FloatingTextEvents.Spawn input)
        {
            Spawn(input.Data);
        }

        #endregion
    }
}