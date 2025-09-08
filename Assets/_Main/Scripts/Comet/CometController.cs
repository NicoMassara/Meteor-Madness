using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Comet
{
    public class CometController : MonoBehaviour
    {
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
        [SerializeField] private CometFactory cometFactory; 
        [SerializeField] private Camera playerCamera;
        
        private readonly Timer _spawnTimer = new Timer();
        private bool _isBottomSpawn;

        private void Start()
        {
            _spawnTimer.Set(firstSpawnDelay);
            _spawnTimer.OnEnd += Timer_OnEndHandler;
        }

        private void Update()
        {
            _spawnTimer.Run();
        }
        
        private void Timer_OnEndHandler()
        {
            SpawnComet();
            _spawnTimer.Set(spawnDelay);
        }

        private void SpawnComet()
        {
            cometFactory.SpawnComet(movementSpeed, GetSpawnPosition(), GetTargetPosition());
            _isBottomSpawn = !_isBottomSpawn;
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
    }
}