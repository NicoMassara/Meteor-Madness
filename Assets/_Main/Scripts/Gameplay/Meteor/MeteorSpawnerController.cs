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
        private readonly MeteorTravelledDistanceTracker _travelledDistanceTracker = new MeteorTravelledDistanceTracker();
        private MeteorLocationSpawnController _locationSpawn;
        private MeteorFactory _meteorFactory;
        private bool _canSpawn;
        private bool _isSpawningRing;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;

        private void Awake()
        {
            _locationSpawn = GetComponent<MeteorLocationSpawnController>();
            
            //Add events to EventManager
            var eventManager = GameManager.Instance.EventManager;
            eventManager.Subscribe<GameRestart>(EnventBus_GameRestart);
            eventManager.Subscribe<UpdateLevel>(EnventBus_UpdateLevel);
            eventManager.Subscribe<EnableMeteorSpawn>(EnventBus_EnableMeteorSpawn);
            eventManager.Subscribe<SpawnRingMeteor>(EnventBus_SpawnRingMeteor);
            eventManager.Subscribe<RecycleAllMeteors>(EnventBus_RecycleAllMeteors);
        }

        private void Start()
        {
            _meteorFactory = new MeteorFactory(meteorPrefab);
            _spawnTimer.OnEnd += SpawnTimer_OnEndHandler;
        }

        public void ManagedUpdate()
        {
            if (_canSpawn && !_isSpawningRing)
            {
                _spawnTimer.Run(CustomTime.GetChannel(SelfUpdateGroup).DeltaTime);
            }

            if (_travelledDistanceTracker.HasMeteor && _canSpawn && !_isSpawningRing)
            {
                if (_travelledDistanceTracker.GetTravelledDistanceRatio() <= _spawnSettings.GetMaxTravelDistance())
                {
                    SpawnSingleMeteor(GameValues.MaxMeteorSpeed);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnSingleMeteor(GameValues.MaxMeteorSpeed);
            }
        }

        #region Spawn

        private void SpawnSingleMeteor(float meteorSpeed)
        {
            var position = _locationSpawn.GetPositionByAngle(_locationSpawn.GetSpawnAngle());
            var finalSpeed = meteorSpeed * _spawnSettings.GetMovementMultiplier();
            CreateMeteor(finalSpeed, position);
        }

        private void SpawnRingMeteor(float meteorSpeed)
        {
            StartCoroutine(CreateRingMeteor(meteorSpeed));
        }

        private IEnumerator CreateRingMeteor(float meteorSpeed)
        {
            _isSpawningRing = true;
            
            var currAngle = 0f;
            var amountToSpawn = (float)_locationSpawn.RingMeteorSpawnAmount;
            var ringsToUse = _locationSpawn.RingsAmount;
            var angleOffset = 360f / amountToSpawn;
            var startAngleOffset = angleOffset/2;
            var startOffset = 0f;

            for (int i = 0; i < ringsToUse; i++)
            {
                if(_canSpawn == false) yield break; 
                
                for (int j = 0; j < amountToSpawn; j++)
                {
                    CreateMeteor(meteorSpeed, _locationSpawn.GetPositionByAngle(currAngle));
                    currAngle += angleOffset;
                    currAngle = Mathf.Repeat(currAngle, 360f);
                }
                
                startOffset += startAngleOffset;
                startOffset = Mathf.Repeat(startOffset, 360f);
                currAngle = startOffset;
                
                yield return new WaitForSeconds(GameTimeValues.RingMeteorDelayBetweenSpawn);
            }
            
            _isSpawningRing = false;
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
            _travelledDistanceTracker.SetMeteor(tempMeteor, centerOfGravity.position);
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
            SpawnSingleMeteor(GameValues.MaxMeteorSpeed);
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
        
        private void EnventBus_GameRestart(GameRestart input)
        {
            _spawnTimer.Set(1f);
            _locationSpawn.RestartValues();
        }

        #endregion
    }

    public class MeteorTravelledDistanceTracker
    {
        private IMeteor _meteor;
        private Vector2 _targetPosition;
        private float _totalDistance;
        
        public bool HasMeteor => _meteor != null;

        public void SetMeteor(IMeteor meteor, Vector2 targetPosition)
        {
            _meteor = meteor;
            _targetPosition = targetPosition;
            _totalDistance = Vector2.Distance(_meteor.Position, targetPosition);
        }

        public float GetTravelledDistanceRatio()
        {
            var currentDistance = Vector2.Distance(_meteor.Position, _targetPosition);
            
            return currentDistance/_totalDistance;
        }
    }

    public class MeteorSpawnSettings
    {
        private int _currentIndex;

        private readonly MeteorSpawnData[] _spawnData = new[]
        {
            new MeteorSpawnData()
            {
                SpeedMultiplier = 0.50f,
                TravelDistance = 0.11f
            },
            new MeteorSpawnData()
            {
                SpeedMultiplier = 0.60f,
                TravelDistance = 0.15f
            },
            new MeteorSpawnData()
            {
                SpeedMultiplier = 0.70f,
                TravelDistance = 0.20f
            },
            new MeteorSpawnData()
            {
                SpeedMultiplier = 0.75f,
                TravelDistance = 0.25f
            },
            new MeteorSpawnData()
            {
                SpeedMultiplier = 0.80f,
                TravelDistance = 0.30f
            },
            new MeteorSpawnData()
            {
                SpeedMultiplier = 0.80f,
                TravelDistance = 0.45f
            },
            new MeteorSpawnData()
            {
                SpeedMultiplier = 0.90f,
                TravelDistance = 0.50f
            },
            new MeteorSpawnData()
            {
                SpeedMultiplier = 0.90f,
                TravelDistance = 0.55f
            },
            new MeteorSpawnData()
            {
                SpeedMultiplier = 0.95f,
                TravelDistance = 0.60f
            },
            new MeteorSpawnData()
            {
                SpeedMultiplier = 1f,
                TravelDistance = 0.70f
            }
        };

        public void SetIndex(int index)
        {
            _currentIndex = index;
            _currentIndex = Mathf.Clamp(_currentIndex, 0, _spawnData.Length - 1);
        }

        public float GetMovementMultiplier()
        {
            return _spawnData[_currentIndex].SpeedMultiplier;
        }

        public float GetMaxTravelDistance()
        {
            return _spawnData[_currentIndex].TravelDistance;
        }
    }

    public class MeteorSpawnData
    {
        public float SpeedMultiplier;
        // The distance needs a Meteor to travel in order to spawn a new one, goes from 1 to 0. 
        public float TravelDistance; 
    }
}