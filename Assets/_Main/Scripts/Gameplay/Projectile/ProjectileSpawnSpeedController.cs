using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile
{
    public class ProjectileSpawnSpeedController : ManagedBehavior
    {
        [SerializeField] private ProjectileSpawnDataSo projectileSpawnDataSo;
        private ProjectileSpawnValues _spawnValues;
        
        private void Awake()
        {
            _spawnValues = new ProjectileSpawnValues(projectileSpawnDataSo);
            
            SetEventBus();
        }


        public float GetMaxTravelDistance()
        {
            return _spawnValues.GetMaxTravelDistance();
        }

        public float GetMovementMultiplier()
        {
            return _spawnValues.GetMovementMultiplier();
        }

        public MeteorSpawnData[] GetSpawnData()
        {
            return projectileSpawnDataSo.SpawnData;
        }
        
        #region EventBus

        private void SetEventBus()
        {
            var eventManager = GameManager.Instance.EventManager;
            eventManager.Subscribe<GameModeEvents.UpdateLevel>(EnventBus_GameMode_UpdateLevel);
        }

        private void EnventBus_GameMode_UpdateLevel(GameModeEvents.UpdateLevel input)
        {
            _spawnValues.SetIndex(input.CurrentLevel);
        }

        #endregion
    }
}