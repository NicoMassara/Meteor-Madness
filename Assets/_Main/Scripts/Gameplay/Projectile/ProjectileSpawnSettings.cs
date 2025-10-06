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
        
        private ProjectileTravelController _speed;
        private ProjectileAngleController _location;
        private ProjectileSlotController _slot;

        private float multiplier = 1;
        private const int SlotAmount = GameParameters.GameplayValues.AngleSlots;

        private void Awake()
        {
            var projectileData = GameConfigManager.Instance.GetGameplayData().ProjectileData;
            var travelData = projectileData.GetTravelData();
            var slotData = projectileData.GetSlotData();
            
            _speed = new ProjectileTravelController(travelData.speed, travelData.speed);
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
            _speed.SetLevel(input.CurrentLevel);
            _slot.SetLevel(input.CurrentLevel);
        }

        private void EventBus_GameMode_Start(GameModeEvents.Start input)
        {
            _location.RestartValues();
            _speed.SetLevel(0);
            _slot.SetLevel(0);
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (centerOfGravity != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(centerOfGravity.position, spawnRadius);

            }
            
            if (_location != null)
            {
                for (int i = 0; i < SlotAmount; i++)
                {
                    var angle = _location.GetAngleBySlot(i);
                    Gizmos.DrawLine(centerOfGravity.position, GetPositionByAngle(angle));
                }
            }
            
            /*if (_speed != null && _location != null)
            {
                var cog = GetCenterOfGravity();
                float temp = cog.x + GetSpawnRadius();

                foreach (var t in _speed.GetSpawnData())
                {
                    var multiplier = t.TravelDistance;
                    Gizmos.color = new Color(multiplier, .5f, multiplier/2f, 1);
                    Gizmos.DrawWireSphere(cog, multiplier * temp);
                }
            }*/
        }

        #endregion
    }
}