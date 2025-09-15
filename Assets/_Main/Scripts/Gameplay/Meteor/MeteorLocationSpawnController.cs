using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorLocationSpawnController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform centerOfGravity;
        [Header("Spawn Rings")] 
        [Range(22f,100f)]
        [SerializeField] private float spawnRadius;
        [Range(1.1f, 2.5f)] 
        [SerializeField] private float[] ringOffset;
        [Range(2, 15)] 
        [SerializeField] private int ringMeteorSpawnAmount;
        [Range(1,5)]
        [SerializeField] private int ringsToUse = 5;
        
        public int RingsToUse => ringsToUse;
        public int RingMeteorSpawnAmount => ringMeteorSpawnAmount;
        
        private float[] _spawnAngleArr;

        #region Create Spawn Angle

        public void CreateSpawnAngleArray()
        {
            float[] angleArr = new float[ringsToUse];
            float proximityRange = 30 / 2f;


            for (int i = 0; i < angleArr.Length; i++)
            {
                if (i == 0)
                {
                    angleArr[i] = Random.Range(0f, 359f);
                }
                else
                {
                    var lastAngle = angleArr[i - 1];
                    angleArr[i] = GetValidAngle(lastAngle,proximityRange);
                }
            }

            _spawnAngleArr = angleArr;
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
                    angle = lastAngle + (180 - (proximityRange - 1));
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
            float oppDiff = Mathf.DeltaAngle(lastAngle + 180f, angle);
            if (Mathf.Abs(oppDiff) < proximityRange) return false;

            return true;
        }

        #endregion

        #region Get Position

        public Vector2 GetPositionByAngle(float angle, int ringIndex)
        {
            float radians = angle * Mathf.Deg2Rad;
            
            //Point in Radius
            Vector2 point = new Vector2(MathF.Cos(radians), Mathf.Sin(radians)) * GetRingOffset(ringIndex);
            return point;
        }

        public Vector2 GetPositionInRadius(int ringIndex)
        {
            return GetPositionByAngle(_spawnAngleArr[ringIndex], ringIndex);
        }

        #endregion


        private float GetRingOffset(int ringNumber)
        {
            float offset = ringNumber switch
            {
                0 => spawnRadius,
                1 => (spawnRadius * ringOffset[0]),
                2 => (spawnRadius * ringOffset[0]) * ringOffset[1],
                3 => ((spawnRadius * ringOffset[0]) * ringOffset[1]) * ringOffset[2],
                4 => (((spawnRadius * ringOffset[0]) * ringOffset[1]) * ringOffset[2]) * ringOffset[3],
                _ => -1
            };

            return offset;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(centerOfGravity.position, GetRingOffset(0));
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(centerOfGravity.position, GetRingOffset(1));
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(centerOfGravity.position, GetRingOffset(2));
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(centerOfGravity.position, GetRingOffset(3));
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(centerOfGravity.position, GetRingOffset(4));
        }
    }
}