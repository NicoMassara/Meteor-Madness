using System;
using System.Collections;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Managers.UpdateManager.Interfaces;
using _Main.Scripts.MyCustoms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Gameplay.Meteor
{
    [RequireComponent(typeof(MeteorLocationSpawnController))]
    public class MeteorSpawnerController : ManagedBehavior, IUpdatable
    {
        [Header("Components")]
        [SerializeField] private MeteorView meteorPrefab;
        [SerializeField] private Transform centerOfGravity;

        private readonly MeteorSpawnSettings _spawnSettings = new MeteorSpawnSettings();
        private readonly Timer _spawnTimer = new Timer();
        private MeteorLocationSpawnController _locationSpawn;
        private MeteorFactory _meteorFactory;
        private bool _canSpawn;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;

        private void Awake()
        {
            _locationSpawn = GetComponent<MeteorLocationSpawnController>();
            
            //Add events to EventManager
            var eventManager = GameManager.Instance.EventManager;
            eventManager.Subscribe<UpdateLevel>(EnventBus_UpdateLevel);
            eventManager.Subscribe<EnableMeteorSpawn>(EnventBus_EnableMeteorSpawn);
            eventManager.Subscribe<SpawnRingMeteor>(EnventBus_SpawnRingMeteor);
            eventManager.Subscribe<RecycleAllMeteors>(EnventBus_RecycleAllMeteors);
        }

        private void Start()
        {
            _meteorFactory = new MeteorFactory(meteorPrefab);
            _spawnTimer.OnEnd += SpawnTimer_OnEndHandler;
            _spawnTimer.Set(1f);
        }

        public void ManagedUpdate()
        {
            if (_canSpawn)
            {
                _spawnTimer.Run(CustomTime.GetChannel(SelfUpdateGroup).DeltaTime);
            }
        }

        #region Spawn

        public void SpawnSingleMeteor(float meteorSpeed)
        {
            _locationSpawn.CreateSpawnAngleArray();
            
            for (int i = _locationSpawn.RingsToUse - 1; i >= 0; i--)
            {
                var position = _locationSpawn.GetPositionInRadius(i);
                var finalSpeed = Random.Range(meteorSpeed*0.95f, meteorSpeed*1.05f);
                CreateMeteor(finalSpeed, position);
            }
        }
        
        public void SpawnRingMeteor(float meteorSpeed)
        {
            var currAngle = 0f;
            var amountToSpawn = (float)_locationSpawn.RingMeteorSpawnAmount;
            var angleOffset = 360f / amountToSpawn;
            var startAngleOffset = angleOffset/2;
            var startOffset = 0f;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < amountToSpawn; j++)
                {
                    CreateMeteor(meteorSpeed, _locationSpawn.GetPositionByAngle(currAngle, i));
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
        
        private void SpawnTimer_OnEndHandler()
        {
            var finalSpeed = GameValues.MaxMeteorSpeed * _spawnSettings.GetMultiplier();
            SpawnSingleMeteor(finalSpeed);
            /*var finalSpeed = GameValues.MaxMeteorSpeed;
            SpawnSingleMeteor(finalSpeed);*/
            _spawnTimer.Set(GameTimeValues.MeteorSpawnDelay);
        }

        #endregion

        #region EventBus
        
        private void EnventBus_UpdateLevel(UpdateLevel input)
        {
            _spawnSettings.SetIndex(input.CurrentLevel);
        }
        
        private void EnventBus_EnableMeteorSpawn(EnableMeteorSpawn input)
        {
            _canSpawn = input.CanSpawn;
        }
        
        private void EnventBus_SpawnRingMeteor(SpawnRingMeteor input)
        {
            SpawnRingMeteor(GameValues.MaxMeteorSpeed);
        }

        private void EnventBus_RecycleAllMeteors(RecycleAllMeteors input)
        {
            RecycleAll();
        }

        #endregion
    }

    public class MeteorSpawnSettings
    {
        private int _currentIndex;
        
        private readonly float[] _spawnMultiplier = new float[]
        {
            0.50f,
            0.60f,
            0.70f,
            0.75f,
            0.80f,
            0.80f,
            0.90f,
            0.90f,
            0.95f,
            1
        };

        public void SetIndex(int index)
        {
            _currentIndex = index;
            _currentIndex = Mathf.Clamp(_currentIndex, 0, _spawnMultiplier.Length - 1);
        }

        public float GetMultiplier()
        {
            return _spawnMultiplier[_currentIndex];
        }
    }
}