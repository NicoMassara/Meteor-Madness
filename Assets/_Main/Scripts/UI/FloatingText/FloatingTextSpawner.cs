using MeteorMadness.Contracts;
using MeteorMadness.Managers;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.UI.FloatingText
{
    public class FloatingTextSpawner : ManagedBehavior
    {
        [SerializeField] private FloatingTextBehaviour prefab;

        private FloatingScoreFactory _factory;

        private void Awake()
        {
            _factory = new FloatingScoreFactory(prefab);
        }

        private void Start()
        {
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
            FloatingTextEventSubscriber.Spawn(EventBus_FloatingText_Spawn);
        }

        private void EventBus_FloatingText_Spawn(FloatingTextEvents.Spawn input)
        {
            Spawn(input.Data);
        }

        #endregion
    }
}