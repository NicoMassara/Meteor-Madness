using System.Collections;
using _Main.Scripts.Gameplay.Projectile;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorSpawner : ManagedBehavior, IUpdatable
    {
        [Header("Components")]
        [SerializeField] private ProjectileSpawnSettings spawnSettings;
        [SerializeField] private MeteorView meteorPrefab;
        
        private MeteorFactory _meteorFactory;
        private bool _isSpawningRing;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        
        private void Awake()
        {
            _meteorFactory = new MeteorFactory(meteorPrefab);
            
            SetEventBus();
        }
        
        public void ManagedUpdate() { }

        #region Spawn

        private void SpawnSingleMeteor(Vector2 spawnPosition, Vector2 direction, float movementMultiplier)
        {
            var finalSpeed = GameParameters.GameplayValues.MaxMeteorSpeed * movementMultiplier;
            var tempMeteor = _meteorFactory.SpawnMeteor();
                
            //Set Direction and Rotation towards COG
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var tempRot = Quaternion.AngleAxis(angle, Vector3.forward);
            tempMeteor.SetValues(new MeteorValuesData
            {
                MovementSpeed = finalSpeed,
                Rotation = tempRot,
                Position = spawnPosition,
                Direction = direction.normalized,
                Value = 1f
            });
            tempMeteor.OnDeflection += Meteor_OnDeflectionHandler;
            tempMeteor.OnEarthCollision += Meteor_OnEarthCollisionHandler;
            
            GameManager.Instance.EventManager.Publish(new ProjectileEvents.Add{Projectile = tempMeteor});
        }

        private void SpawnRingMeteor(float meteorSpeed)
        {
            if(_isSpawningRing) return;
            
            StartCoroutine(CreateRingMeteor(meteorSpeed));
        }

        private IEnumerator CreateRingMeteor(float meteorSpeed)
        {
            GameManager.Instance.EventManager.Publish(new MeteorEvents.RingActive{IsActive = true});

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
                    for (int j = 0; j < amountToSpawn; j++)
                    {
                        yield return new WaitForSeconds(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));

                        CreateMeteor(meteorSpeed * speedMultiplier, spawnSettings.GetPositionByAngle(currAngle, spawnSettings.GetSpawnRadius()), 
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
            
            yield return new WaitForSeconds(GameParameters.TimeValues.MeteorSpawnDelayAfterRing);
            
            GameManager.Instance.EventManager.Publish(new MeteorEvents.RingActive{IsActive = false});
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
            var tempMeteor = _meteorFactory.SpawnMeteor();
            var cog = spawnSettings.GetCenterOfGravity();
                
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
            tempMeteor.EnableMovement = true;
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
                new MeteorEvents.Deflected
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
                new MeteorEvents.Collision
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
            eventManager.Subscribe<MeteorEvents.SpawnRing>(EnventBus_Meteor_SpawnRing);
            eventManager.Subscribe<MeteorEvents.RecycleAll>(EnventBus_Meteor_RecycleAll);
            eventManager.Subscribe<GameModeEvents.Disable>(EventBus_GameMode_Disable);
            eventManager.Subscribe<ProjectileEvents.Spawn>(EventBus_Projectile_Spawn);
        }

        private void EventBus_Projectile_Spawn(ProjectileEvents.Spawn input)
        {
            if (input.ProjectileType == ProjectileType.Meteor)
            {
                SpawnSingleMeteor(input.Position, input.Direction, input.MovementMultiplier);
            }
        }

        private void EventBus_GameMode_Disable(GameModeEvents.Disable input)
        {
            RecycleAll();
        }
        
        private void EnventBus_Meteor_SpawnRing(MeteorEvents.SpawnRing input)
        {
            SpawnRingMeteor(GameParameters.GameplayValues.MaxMeteorSpeed);
        }

        private void EnventBus_Meteor_RecycleAll(MeteorEvents.RecycleAll input)
        {
            RecycleAll();
        }

        #endregion
    }
}