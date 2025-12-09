using _Main.Scripts.FlyingObject;
using _Main.Scripts.Managers;
using UnityEngine;
using NicolasMassara.CustomTimerManager;
using NicolasMassara.CustomUpdateManager;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Comet
{
    public class CometFactory : ManagedBehavior
    {
        [SerializeField] private CometView cometPrefab;
        [Header("Values")]
        [Range(1,100f)]
        [SerializeField] private float movementSpeed = 15;
        [SerializeField] private float spawnOffset;
        [Header("Components")]
        [SerializeField] private Camera playerCamera;
        
        private GenericPool<CometView> _pool;
        private bool _isBottomSpawn;
        private TimerManager.GeneratedId _spawnTimerId;

        private void Awake()
        {
            BootEvents.OnSubSystemRequestInitialize += Initialize;
        }

        private void Initialize()
        {            
            BootEvents.OnSubSystemRequestInitialize -= Initialize;
            //
            _pool = new GenericPool<CometView>(cometPrefab, 1, 5);
            
            SetTimer(GameConfigManager.Instance.GetGameplayData().GameTimeData.FirstCometSpawnDelay);
            
            BootEvents.SubSystemInitialized();
        }

        private void SetTimer(float spawnDelay)
        {
            _spawnTimerId = TimerManager.Add(new TimerData(spawnDelay, Timer_OnEndHandler, UpdateFrequency.EveryFrame));
        }

        private void Timer_OnEndHandler()
        {
            if(playerCamera == null) return;
            if (GameManager.Instance.IsPaused == false)
            {
                SpawnComet(GetSpawnPosition(), GetTargetPosition());
            }
            
            var spawnDelay = GameConfigManager.Instance.GetGameplayData().GameTimeData.CometSpawnDelay;
            var spawnDelayRange = Random.Range(spawnDelay*0.75f, spawnDelay*1.25f);
            
            SetTimer(spawnDelayRange);
        }

        private Vector2 GetSpawnPosition()
        {
            var screenX = Random.Range(0, Screen.width);
            var screenY = _isBottomSpawn ? 0 : Screen.height;
            var tempPosition = new Vector3(screenX, screenY, -10f);
            var worldPosition = playerCamera.ScreenToWorldPoint(tempPosition);
            return new Vector2(worldPosition.x,  _isBottomSpawn ? -spawnOffset : worldPosition.y + spawnOffset );
        }

        private Vector2 GetTargetPosition()
        {
            var screenX = Random.Range(0, Screen.width);
            var screenY = _isBottomSpawn ? Screen.height : 0;
            var tempPosition = new Vector3(screenX, screenY, -10f);
            var worldPosition = playerCamera.ScreenToWorldPoint(tempPosition);
            return new Vector2(worldPosition.x,  worldPosition.y);
        }

        private void SpawnComet(Vector2 spawnPosition, Vector2 targetPosition)
        {
            _isBottomSpawn = !_isBottomSpawn;
            Vector2 direction = targetPosition - spawnPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var tempRot = Quaternion.AngleAxis(angle, Vector3.forward);
            var tempComet = _pool.Get();
            tempComet.SetValues(new FlyingObjectValues
            {
                MovementSpeed = movementSpeed,
                Rotation = tempRot,
                Position = spawnPosition,
                Direction = direction,
            });
            tempComet.OnRecycle += Comet_OnRecycleHandler;
        }

        private void Comet_OnRecycleHandler(CometView item)
        {
            item.OnRecycle -= Comet_OnRecycleHandler;
            _pool.Release(item);
        }
    }
}