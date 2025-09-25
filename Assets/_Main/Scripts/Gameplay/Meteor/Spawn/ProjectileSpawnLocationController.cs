using System;
using _Main.Scripts.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class ProjectileSpawnLocationController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform centerOfGravity;
        [Header("Values")] 
        [Range(22f,100f)]
        [SerializeField] private float spawnRadius;
        [Range(1, 180f)] 
        [SerializeField] private int minSpawnProximity = 30;
        [Range(0, 180)] 
        [SerializeField] private int maxSpawnProximity = 180;

        private float _lastAngle;

        private void Awake()
        {
            GameManager.Instance.EventManager.Subscribe<GameStart>(EventBus_OnGameStart);
        }

        #region Create Spawn Angle

        // ReSharper disable Unity.PerformanceAnalysis
        public float GetSpawnAngle()
        {
            float proximityRange = (float)minSpawnProximity;
            float angle = 0f;

            angle =  GetValidAngle(_lastAngle,proximityRange);
            
            _lastAngle = angle; 
            return angle;
        }
        
        private float GetValidAngle(float lastAngle, float proximityRange)
        {
            float angle;
            int safety = 0;
            do
            {
                angle = Random.Range(0f, 359f);
                safety++;
                if (safety > 1000) // seguridad anti-bucle infinito
                {
                    Debug.LogWarning("No se encontró un ángulo válido!");
                    angle = lastAngle + (maxSpawnProximity - (proximityRange - 1));
                    break;
                }
            }
            while (!IsAngleValid(angle,lastAngle,proximityRange));
            
            return angle;
        }
        
        private bool IsAngleValid(float angle, float lastAngle, float proximityRange)
        {
            if (lastAngle < 0) return true; // primera vez siempre válido

            // Diferencia directa
            float diff = Mathf.DeltaAngle(lastAngle, angle);

            // Zona prohibida alrededor del último
            if (Mathf.Abs(diff) < proximityRange) return false;

            // Zona prohibida alrededor del opuesto
            float oppDiff = Mathf.DeltaAngle(lastAngle + maxSpawnProximity, angle);
            if (Mathf.Abs(oppDiff) < proximityRange) return false;

            return true;
        }

        #endregion

        #region Get Position

        public Vector2 GetPositionByAngle(float angle, float radius)
        {
            float radians = angle * Mathf.Deg2Rad;
            
            //Point in Radius
            Vector2 point = new Vector2(MathF.Cos(radians), Mathf.Sin(radians)) * radius;
            return point;
        }
        

        #endregion

        public Vector2 GetCenterOfGravity()
        {
            return centerOfGravity.position;
        }
        
        private void RestartValues()
        {
            _lastAngle = 0;
        }

        #region Event Bus

        private void EventBus_OnGameStart(GameStart input)
        {
            RestartValues();
        }
        #endregion
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(centerOfGravity.position, spawnRadius);
        }
    }
}