using System;
using System.Collections;
using _Main.Scripts.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorSpawner : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MeteorView meteorPrefab;
        [SerializeField] private Transform centerOfGravity;
        [Header("Spawn Rings")] 
        [Range(22f,100f)]
        [SerializeField] private float spawnRadius;
        [Range(1.1f, 2.5f)] 
        [SerializeField] private float[] ringOffset;
        [Range(2, 360f)] 
        [SerializeField] private int ringMeteorSpawnAmount;
        
        private MeteorFactory _meteorFactory;
        private float[] _spawnAngleArr;


        private void Awake()
        {
            //Add events to EventManager
            var eventManager = GameManager.Instance.EventManager;
            eventManager.Subscribe<SpawnSingleMeteor>(EnventBus_SpawnSingleMeteor);
            eventManager.Subscribe<SpawnRingMeteor>(EnventBus_SpawnRingMeteor);
            eventManager.Subscribe<RecycleAllMeteors>(EnventBus_RecycleAllMeteors);
        }

        private void Start()
        {
            _meteorFactory = new MeteorFactory(meteorPrefab);
        }

        #region Spawn Single Meteor

        public void SpawnSingleMeteor(float meteorSpeed)
        {
            _spawnAngleArr = CreateSpawnAngle();
            StartCoroutine(SpawnSingleMeteorCoroutine(meteorSpeed));
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator SpawnSingleMeteorCoroutine(float meteorSpeed)
        {
            for (int i = 5 - 1; i >= 0; i--)
            {
                var position = GetPositionInRadius(i);
                var finalSpeed = Random.Range(meteorSpeed*0.95f, meteorSpeed*1.05f);
                CreateMeteor(finalSpeed, position);
                
                yield return new WaitForSeconds(GameTimeValues.MeteorDelayBetweenSpawn);
            }
            
            /*for (int i = 0; i < 5; i++)
            {
                CreateMeteor(meteorSpeed, i);

                yield return new WaitForSeconds(_delayBetweenSpawns);
            }*/
        }
        
        private float[] CreateSpawnAngle()
        {
            float[] angleArr = new float[5];
            float proximityRange = 45 / 2f;

            for (int i = 0; i < angleArr.Length; i++)
            {
                float newAngle;
                bool valid;

                do
                {
                    newAngle = Random.Range(0f, 360f);
                    valid = true;

                    for (int j = 0; j < i; j++)
                    {
                        float checkAngle = angleArr[j];

                        // Creamos el rango de exclusión
                        float lowerBound = (checkAngle - proximityRange + 360f) % 360f;
                        float upperBound = (checkAngle + proximityRange) % 360f;

                        // Verificar si newAngle cae dentro del rango
                        if (lowerBound < upperBound)
                        {
                            if (newAngle >= lowerBound && newAngle <= upperBound)
                            {
                                valid = false;
                                break;
                            }
                        }
                        else // el rango envuelve 0°
                        {
                            if (newAngle >= lowerBound || newAngle <= upperBound)
                            {
                                valid = false;
                                break;
                            }
                        }
                    }

                } while (!valid);

                angleArr[i] = newAngle;
            }

            return angleArr;
        }

        #endregion

        #region Spawn Ring Meteor

        public void SpawnRingMeteor(float meteorSpeed)
        {
            var currAngle = 0f;
            var amountToSpawn = (float)ringMeteorSpawnAmount;
            var angleOffset = 360f / amountToSpawn;
            var startAngleOffset = angleOffset/2;
            var startOffset = 0f;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < amountToSpawn; j++)
                {
                    CreateMeteor(meteorSpeed, GetPositionByAngle(currAngle, i));
                    currAngle += angleOffset;
                    currAngle = Mathf.Repeat(currAngle, 360f);
                }
                startOffset += startAngleOffset;
                startOffset = Mathf.Repeat(startOffset, 360f);
                currAngle = startOffset;
            }
        }

        #endregion
        
        // ReSharper disable Unity.PerformanceAnalysis
        
        private void CreateMeteor(float meteorSpeed, Vector2 spawnPosition)
        {
            var tempMeteor = _meteorFactory.SpawnMeteor();
                
            //Set Direction and Rotation towards COG
            Vector2 direction = (Vector2)centerOfGravity.position - spawnPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var tempRot = Quaternion.AngleAxis(angle, Vector3.forward);
            tempMeteor.SetValues(meteorSpeed, tempRot, spawnPosition,direction.normalized);
            tempMeteor.OnDeflection += Meteor_OnDeflectionHandler;
            tempMeteor.OnEarthCollision += Meteor_OnEarthCollisionHandler;
        }

        public void RecycleAll()
        {
            _meteorFactory.RecycleAll();
        }

        private Vector2 GetPositionByAngle(float angle, int ringIndex)
        {
            float radians = angle * Mathf.Deg2Rad;
            
            //Point in Radius
            Vector2 point = new Vector2(MathF.Cos(radians), Mathf.Sin(radians)) * GetRingOffset(ringIndex);
            return point;
        }

        private Vector2 GetPositionInRadius(int ringIndex)
        {
            return GetPositionByAngle(_spawnAngleArr[ringIndex], ringIndex);
        }

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

        #region Handlers

        private void Meteor_OnDeflectionHandler(MeteorView view, 
            Vector3 position, Quaternion rotation, Vector2 direction)
        {
            view.OnDeflection = null;
            view.OnEarthCollision = null;
            
            GameManager.Instance.EventManager.Publish
            (
                new MeteorDeflected
                {
                    Position = position,
                    Rotation = rotation,
                    Direction = direction
                }
            );
            
            view.ForceRecycle();
        }

        private void Meteor_OnEarthCollisionHandler(MeteorView view, 
            Vector3 position, Quaternion rotation, Vector2 direction)
        {
            view.OnDeflection = null;
            view.OnEarthCollision = null;
            
            GameManager.Instance.EventManager.Publish
            (
                new MeteorCollision
                {
                    Position = position,
                    Rotation = rotation,
                    Direction = direction
                }
            );
            
            view.ForceRecycle();
        }

        #endregion

        #region EventBus

        private void EnventBus_SpawnSingleMeteor(SpawnSingleMeteor input)
        {
            SpawnSingleMeteor(input.Speed);
        }
        
        private void EnventBus_SpawnRingMeteor(SpawnRingMeteor input)
        {
            SpawnRingMeteor(input.Speed);
        }

        private void EnventBus_RecycleAllMeteors(RecycleAllMeteors input)
        {
            RecycleAll();
        }

        #endregion

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