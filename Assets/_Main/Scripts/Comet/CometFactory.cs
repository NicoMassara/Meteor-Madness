using _Main.Scripts.Particles;
using UnityEngine;

namespace _Main.Scripts.Comet
{
    public class CometFactory : MonoBehaviour
    {
        [SerializeField] private CometMotor cometPrefab;
        private GenericPool<CometMotor> _pool;
        
        private void Start()
        {
            _pool = new GenericPool<CometMotor>(cometPrefab);
        }

        public void SpawnComet(float movementSpeed, Vector2 spawnPosition, Vector2 targetPosition)
        {
            Vector2 direction = targetPosition - spawnPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var tempRot = Quaternion.AngleAxis(angle, Vector3.forward);
            var tempComet = _pool.Get();
            tempComet.SetValues(movementSpeed, tempRot, spawnPosition);
            tempComet.OnRecycle += Comet_OnRecycleHandler;
        }

        private void Comet_OnRecycleHandler(CometMotor item)
        {
            item.OnRecycle -= Comet_OnRecycleHandler;
            _pool.Release(item);
        }
    }
}