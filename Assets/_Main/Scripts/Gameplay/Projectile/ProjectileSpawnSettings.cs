using _Main.Scripts.Gameplay.Projectile.Utilities;
using _Main.Scripts.Managers;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile
{
    public class ProjectileSpawnSettings : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform centerOfGravity;

        [Header("Values")] 
        [Range(22f, 100f)]
        [SerializeField] private float spawnRadius = 26;

        [Header("Gizmos")] 
        [SerializeField] private bool showSpawnRadius;
        [SerializeField] private bool showAngleSlots;
        [SerializeField] private bool showTravelDistance;
        
        
        private ProjectileTravelController _speed;
        private ProjectileAngleController _location;
        private ProjectileSlotController _slot;

        private float multiplier = 1;
        private const int SlotAmount = GameParameters.GameplayValues.AngleSlots;

        private void Awake()
        {
            var gameplayData = GameConfigManager.Instance.GetGameplayData();
            var projectileData = gameplayData.ProjectileData;

            var slotData = projectileData.GetSlotData();
            
            _speed = new ProjectileTravelController(
                projectileData.GetSpeedMultiplier,projectileData.GetTravelRatio);
            _slot = new ProjectileSlotController(slotData.minSlot, slotData.maxSlot);
            _location = new ProjectileAngleController(SlotAmount);
            
            SetEventBus();
        }

        
        public Vector2 GetPositionByAngle(float currAngle)
        {
           return _location.GetPositionByAngle(currAngle, spawnRadius);
        }

        public Vector2 GetSpawnPosition()
        {
            return GetPositionByAngle(GetSpawnAngle());
        }

        private float GetSpawnAngle()
        {
            return _location.GetSpawnAngle(_slot.GetMinSlotDistance(), _slot.GetMaxSlotDistance());
        }

        public float GetMaxTravelDistance()
        {
            return _speed.GetMaxTravelDistance() * multiplier;
        }

        public float GetMovementMultiplier()
        {
            return _speed.GetMovementMultiplier();
        }

        public Vector2 GetCenterOfGravity()
        {
            return centerOfGravity.transform.position;
        }

        public void SetMultiplier(float value = 1)
        {
            value = Mathf.Clamp(value, 0.01f, 10);
            multiplier = value;
        }

        #region Event Bus

        private void SetEventBus()
        {
            var eventManager = GameManager.Instance.EventManager;
            eventManager.Subscribe<GameModeEvents.Start>(EventBus_GameMode_Start);
            eventManager.Subscribe<GameModeEvents.UpdateLevel>(EventBus_GameMode_UpdateLevel);
        }

        private void EventBus_GameMode_UpdateLevel(GameModeEvents.UpdateLevel input)
        {
            _speed.SetLevelIndex(input.CurrentLevel);
            _slot.SetLevel(input.CurrentLevel);
        }

        private void EventBus_GameMode_Start(GameModeEvents.Start input)
        {
            _location.RestartValues();
            var gameplayData = GameConfigManager.Instance.GetGameplayData();
            _speed.SetLevelAmount(gameplayData.LevelAmount);
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (centerOfGravity != null && showSpawnRadius)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(centerOfGravity.position, spawnRadius);
            }
            
            if (showAngleSlots)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < SlotAmount; i++)
                {
                    var angle = AngleCalculations.GetAngleBySlot(i,SlotAmount);
                    Gizmos.DrawLine(centerOfGravity.position, AngleCalculations.GetPositionByAngle(angle, spawnRadius));
                }
            }
            
            if (showTravelDistance)
            {
                var cog = GetCenterOfGravity();
                float temp = cog.x + spawnRadius;

                var gameplayData = FindObjectOfType<GameConfigManager>().GetGameplayData();
                var levelAmount = gameplayData.LevelAmount;
                var projectileData = gameplayData.ProjectileData;

                for (int i = 0; i < levelAmount; i++)
                {
                    var t = i / (float)(levelAmount - 1);
                    var curveValue = projectileData.GetTravelRatio(t);
                    var currentRadius = curveValue * temp;
                    Gizmos.color = Color.Lerp(Color.green, Color.red, t);
                    Gizmos.DrawWireSphere(cog, currentRadius);
                }
            }
        }

        #endregion
    }
}