using _Main.Scripts.Particles;
using UnityEngine;

namespace _Main.Scripts.Comet
{
    public class CometFactory : MonoBehaviour
    {
        [SerializeField] private CometView cometPrefab;
        [Header("Values")]
        [Range(1,100f)]
        [SerializeField] private float movementSpeed = 15;
        [SerializeField] private float spawnOffset;
        [Header("Spawn Delay")]
        [Range(1, 60f)]
        [SerializeField] private float spawnDelay;
        [Range(0,10)]
        [SerializeField] private float firstSpawnDelay;
        [Header("Components")]
        [SerializeField] private Camera playerCamera;
        
        private readonly Timer _spawnTimer = new Timer();
        private GenericPool<CometView> _pool;
        private bool _isBottomSpawn;

        private void Start()
        {
            _pool = new GenericPool<CometView>(cometPrefab);
            _spawnTimer.Set(firstSpawnDelay);
            _spawnTimer.OnEnd += Timer_OnEndHandler;
        }

        private void Update()
        {
            _spawnTimer.Run();
        }
        
        private void Timer_OnEndHandler()
        {
            SpawnComet(GetSpawnPosition(), GetTargetPosition());
            _spawnTimer.Set(spawnDelay);
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
            tempComet.SetValues(movementSpeed, tempRot, spawnPosition);
            tempComet.OnRecycle += Comet_OnRecycleHandler;
        }

        private void Comet_OnRecycleHandler(CometView item)
        {
            item.OnRecycle -= Comet_OnRecycleHandler;
            _pool.Release(item);
        }
    }
}