﻿using System.Collections;
using _Main.Scripts.Gameplay.Projectile;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorSpawner : ManagedBehavior
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
        
        private float GetMovementSpeed()
        {
            return GameConfigManager.Instance.GetGameplayData().ProjectileData.MaxProjectileSpeed;
        }

        #region Spawn

        private void SpawnSingleMeteor(Vector2 spawnPosition, Vector2 direction, float movementMultiplier)
        {
            var finalSpeed = GetMovementSpeed() * movementMultiplier;
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
            var projectileData = GameConfigManager.Instance.GetGameplayData().ProjectileData;
            var ringValues = projectileData.MeteorRingData;

            _isSpawningRing = true;
            
            yield return new WaitUntil(()=> _meteorFactory.ActiveMeteorCount == 0);
            
            var currAngle = 0f;
            var amountToSpawn = ringValues.MeteorAmount;
            var angleOffset = 360f / amountToSpawn;
            var startAngleOffset = angleOffset/2;
            var startOffset = 0f;
            var speedMultiplier = 2f;

            for (int a = 0; a < ringValues.WavesAmount; a++)
            {
                for (int i = 0; i < ringValues.RingsAmount; i++)
                {
                    for (int j = 0; j < amountToSpawn; j++)
                    {
                        yield return new WaitForSeconds(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));

                        CreateMeteor(meteorSpeed * speedMultiplier, spawnSettings.GetPositionByAngle(currAngle), 
                            GetRingMeteorValue(amountToSpawn, ringValues.RingsAmount));
                        currAngle += angleOffset;
                        currAngle = Mathf.Repeat(currAngle, 360f);
                    }
                
                    startOffset += startAngleOffset;
                    startOffset = Mathf.Repeat(startOffset, 360f);
                    currAngle = startOffset;
                    
                    yield return new WaitForSeconds(ringValues.DelayBetweenRings);
                }
                
                yield return new WaitForSeconds(ringValues.DelayBetweenWaves);
            }
            
            
            yield return new WaitUntil(()=> _meteorFactory.ActiveMeteorCount == 0);
            
            yield return new WaitForSeconds(projectileData.MeteorSpawnDelayAfterRing);
            
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
                new ProjectileEvents.Deflected
                {
                    Position = data.Position,
                    Rotation = data.Rotation,
                    Direction = data.Direction,
                    Value = data.Value,
                    Type = ProjectileType.Meteor
                }
            );
            
            data.Meteor.Recycle();
        }

        private void Meteor_OnEarthCollisionHandler(MeteorCollisionData data)
        {
            data.Meteor.OnDeflection = null;
            data.Meteor.OnEarthCollision = null;
            
            GameManager.Instance.EventManager.Publish
            (
                new ProjectileEvents.Collision
                {
                    Position = data.Position,
                    Rotation = data.Rotation,
                    Direction = data.Direction,
                    Type = ProjectileType.AbilitySphere
                }
            );
            
            data.Meteor.Recycle();
        }

        #endregion

        #region EventBus

        private void SetEventBus()
        {
            GameEventCaller.Subscribe<MeteorEvents.SpawnRing>(EnventBus_Meteor_SpawnRing);
            GameEventCaller.Subscribe<MeteorEvents.RecycleAll>(EnventBus_Meteor_RecycleAll);
            GameEventCaller.Subscribe<GameModeEvents.Disable>(EventBus_GameMode_Disable);
            GameEventCaller.Subscribe<ProjectileEvents.Spawn>(EventBus_Projectile_Spawn);
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
            SpawnRingMeteor(GetMovementSpeed());
        }

        private void EnventBus_Meteor_RecycleAll(MeteorEvents.RecycleAll input)
        {
            RecycleAll();
        }

        #endregion
    }
}