using System.Collections;
using _Main.Scripts.FyingObject;
using _Main.Scripts.Gameplay.FlyingObject;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorSpawner : ManagedBehavior, IUpdatable
    {
        [Header("Components")]
        [SerializeField] private ProjectileSpawnLocationController spawnLocation;
        [SerializeField] private MeteorView meteorPrefab;
        [SerializeField] private ProjectileSpawnDataSo projectileSpawnDataSo;
        [Space]
        [Header("Testing")] 
        [SerializeField] private bool doesSpawn;
        
        private readonly MeteorTravelledDistanceTracker _travelledDistanceTracker = new MeteorTravelledDistanceTracker();
        private ProjectileSpawnValues spawnValues;
        private MeteorFactory _meteorFactory;
        private bool _canSpawn;
        private bool _isSpawningRing;
        private bool _isFirstSpawn;
        private ulong _firstSpawnTimerId;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        
        private void Awake()
        {
            _meteorFactory = new MeteorFactory(meteorPrefab);
            spawnValues = new ProjectileSpawnValues(projectileSpawnDataSo);
            
            SetEventBus();
        }
        public void ManagedUpdate()
        {
            if (_travelledDistanceTracker.HasMeteor 
                && _canSpawn 
                && !_isSpawningRing)
            {
                if (_travelledDistanceTracker.GetTravelledDistanceRatio() <= spawnValues.GetMaxTravelDistance())
                {
                    SpawnSingleMeteor(GameValues.MaxMeteorSpeed);
                }
            }
        }

        #region Spawn

        private void SpawnSingleMeteor(float meteorSpeed)
        {
            if(_canSpawn == false) return;
            
            var position = spawnLocation.GetPositionByAngle(spawnLocation.GetSpawnAngle(), spawnLocation.GetSpawnRadius());
            var finalSpeed = meteorSpeed * spawnValues.GetMovementMultiplier();
            CreateMeteor(finalSpeed, position);
        }

        private void SpawnRingMeteor(float meteorSpeed)
        {
            if(_isSpawningRing) return;
            
            StartCoroutine(CreateRingMeteor(meteorSpeed));
        }

        private IEnumerator CreateRingMeteor(float meteorSpeed)
        {
            _isSpawningRing = true;
            
            yield return new WaitUntil(()=> _meteorFactory.ActiveMeteorCount == 0);
            
            var currAngle = 0f;
            var amountToSpawn = MeteorRingValues.MeteorAmount;
            var ringsToUse = MeteorRingValues.RingsAmount;
            var angleOffset = 360f / amountToSpawn;
            var startAngleOffset = angleOffset/2;
            var startOffset = 0f;
            var speedMultiplier = 2f;

            for (int a = 0; a < MeteorRingValues.WavesAmount; a++)
            {
                for (int i = 0; i < MeteorRingValues.RingsAmount; i++)
                {
                    if(_canSpawn == false) yield break; 
                
                    for (int j = 0; j < amountToSpawn; j++)
                    {
                        yield return new WaitForSeconds(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));

                        CreateMeteor(meteorSpeed * speedMultiplier, spawnLocation.GetPositionByAngle(currAngle, spawnLocation.GetSpawnRadius()), 
                            GetRingMeteorValue(amountToSpawn, ringsToUse));
                        currAngle += angleOffset;
                        currAngle = Mathf.Repeat(currAngle, 360f);
                    }
                
                    startOffset += startAngleOffset;
                    startOffset = Mathf.Repeat(startOffset, 360f);
                    currAngle = startOffset;
                    
                    yield return new WaitForSeconds(MeteorRingValues.DelayBetweenRings);
                }
                
                
                yield return new WaitForSeconds(MeteorRingValues.DelayBetweenWaves);
            }
            
            
            yield return new WaitUntil(()=> _meteorFactory.ActiveMeteorCount == 0);
            yield return new WaitForSeconds(GameTimeValues.MeteorSpawnDelayAfterRing);
            
            _isSpawningRing = false;
        }
        
        private float GetRingMeteorValue(float amountToSpawn, float ringsToUse)
        {
            var temp1 = 1f - (amountToSpawn * 0.1f);
            var temp2 = temp1 * (ringsToUse * 0.1f);
            
            return temp2 * 1.5f;
        }


        #endregion

        #region Create

        // ReSharper disable Unity.PerformanceAnalysis
        
        private void CreateMeteor(float meteorSpeed, Vector2 spawnPosition, float value = 1)
        {
            if(doesSpawn == false) return;
            
            var tempMeteor = _meteorFactory.SpawnMeteor();
            var cog = spawnLocation.GetCenterOfGravity();
                
            //Set Direction and Rotation towards COG
            Vector2 direction = cog - spawnPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var tempRot = Quaternion.AngleAxis(angle, Vector3.forward);
            tempMeteor.SetValues(new MeteorValuesData
            {
                MovementSpeed = meteorSpeed,
                Rotation = tempRot,
                Position = spawnPosition,
                Direction = direction.normalized,
                Value = value
            });
            tempMeteor.OnDeflection += Meteor_OnDeflectionHandler;
            tempMeteor.OnEarthCollision += Meteor_OnEarthCollisionHandler;
            _travelledDistanceTracker.SetMeteor(tempMeteor, cog);
        }

        private void RecycleAll()
        {
            _meteorFactory.RecycleAll();
        }

        #endregion

        #region Handlers

        private void Meteor_OnDeflectionHandler(MeteorCollisionData data)
        {
            data.Meteor.OnDeflection = null;
            data.Meteor.OnEarthCollision = null;
            
            GameManager.Instance.EventManager.Publish
            (
                new MeteorDeflected
                {
                    Position = data.Position,
                    Rotation = data.Rotation,
                    Direction = data.Direction,
                    Value = data.Value
                }
            );
            
            data.Meteor.ForceRecycle();
        }

        private void Meteor_OnEarthCollisionHandler(MeteorCollisionData data)
        {
            data.Meteor.OnDeflection = null;
            data.Meteor.OnEarthCollision = null;
            
            GameManager.Instance.EventManager.Publish
            (
                new MeteorCollision
                {
                    Position = data.Position,
                    Rotation = data.Rotation,
                    Direction = data.Direction
                }
            );
            
            data.Meteor.ForceRecycle();
        }

        #endregion

        #region EventBus

        private void SetEventBus()
        {
            var eventManager = GameManager.Instance.EventManager;
            eventManager.Subscribe<GameStart>(EnventBus_GameStart);
            eventManager.Subscribe<UpdateLevel>(EnventBus_UpdateLevel);
            eventManager.Subscribe<EnableMeteorSpawn>(EnventBus_EnableMeteorSpawn);
            eventManager.Subscribe<SpawnRingMeteor>(EnventBus_SpawnRingMeteor);
            eventManager.Subscribe<RecycleAllMeteors>(EnventBus_RecycleAllMeteors);
            eventManager.Subscribe<GameModeEnable>(EventBus_GameModeEnable);
        }

        private void EventBus_GameModeEnable(GameModeEnable input)
        {
            if (input.IsEnabled)
            {
                
            }
            else
            {
                TimerManager.Remove(_firstSpawnTimerId);
                RecycleAll();
            }
        }

        private void EnventBus_UpdateLevel(UpdateLevel input)
        {
            spawnValues.SetIndex(input.CurrentLevel);
        }
        
        private void EnventBus_EnableMeteorSpawn(EnableMeteorSpawn input)
        {
            _canSpawn = input.CanSpawn;
            if (_isFirstSpawn)
            {
                _firstSpawnTimerId = TimerManager.Add(new TimerData
                {
                    Time = 1f,
                    OnEndAction = ()=> SpawnSingleMeteor(GameValues.MaxMeteorSpeed)
                }, SelfUpdateGroup);
            }
        }
        
        private void EnventBus_SpawnRingMeteor(SpawnRingMeteor input)
        {
            SpawnRingMeteor(GameValues.MaxMeteorSpeed);
        }

        private void EnventBus_RecycleAllMeteors(RecycleAllMeteors input)
        {
            RecycleAll();
        }
        
        private void EnventBus_GameStart(GameStart input)
        {
            _isFirstSpawn = true;
            _travelledDistanceTracker.ClearValues();
        }

        #endregion
        
        #region Gizmos

        private void OnDrawGizmos()
        {
            if(spawnLocation == null) return;
            
            var cog = spawnLocation.GetCenterOfGravity();
            float temp = cog.x + spawnLocation.GetSpawnRadius();

            foreach (var t in projectileSpawnDataSo.SpawnData)
            {
                var multiplier = t.TravelDistance;
                Gizmos.color = new Color(multiplier, .5f, multiplier/2f, 1);
                Gizmos.DrawWireSphere(cog, multiplier * temp);
            }
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

        public void ClearValues()
        {
            _meteor = null;
            _targetPosition = Vector2.zero;
            _totalDistance = float.PositiveInfinity;
        }

        public float GetTravelledDistanceRatio()
        {
            var currentDistance = Vector2.Distance(_meteor.Position, _targetPosition);
            
            return currentDistance/_totalDistance;
        }
    }
}