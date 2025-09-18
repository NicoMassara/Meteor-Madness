using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorLocationSpawnController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform centerOfGravity;
        [Header("Values")] 
        [Range(1, 180f)] 
        [SerializeField] private int minSpawnProximity = 30;
        [Range(0, 180)] 
        [SerializeField] private int maxSpawnProximity = 180;
        [Header("Ring Meteor Value")]
        [Range(2,20)]
        [SerializeField] private int ringSpawnAmount;
        [Range(1, 10)] 
        [SerializeField] private int ringsAmount;

        private float _lasAngle;
        private bool _isFirstSpawn;
        public int RingMeteorSpawnAmount => ringSpawnAmount;
        public int RingsAmount => ringsAmount;

        public void RestartValues()
        {
            _lasAngle = 0;
            _isFirstSpawn = true;
        }

        #region Create Spawn Angle

        // ReSharper disable Unity.PerformanceAnalysis
        public float GetSpawnAngle()
        {
            float proximityRange = (float)minSpawnProximity;
            float angle = 0f;

            if (_isFirstSpawn)
            {
                _isFirstSpawn = false;
                angle = Random.Range(0f, 359f);
            }
            else
            {
                angle =  GetValidAngle(_lasAngle,proximityRange);
                float diferenciaAbs = Mathf.Abs(Mathf.DeltaAngle(_lasAngle, angle));
            }
            
            _lasAngle = angle; 
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

        public Vector2 GetPositionByAngle(float angle, float spawnRadius)
        {
            float radians = angle * Mathf.Deg2Rad;
            
            //Point in Radius
            Vector2 point = new Vector2(MathF.Cos(radians), Mathf.Sin(radians)) * spawnRadius;
            return point;
        }
        

        #endregion
    }
}