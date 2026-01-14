using System;
using System.Collections;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Core.FlyingObject;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.Meteor;
using MeteorMadness.GlobalValues.Tools;
using MeteorMadness.Managers.GameConfig;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Projectile
{
    internal class MeteorFactory : MonoBehaviour
    {
        #region Components
        
        #region Spawner

        private sealed class Spawner
        {
            private readonly GenericPool<MeteorView> _pool;

            public int ActiveMeteorCount { get; private set; }

            public Spawner(MeteorView meteorPrefab, int startCapacity = 3)
            {
                _pool = new GenericPool<MeteorView>(meteorPrefab, startCapacity, 10, "Meteor");
            }

            public IMeteor Spawn()
            {
                var meteor = _pool.Get();

                ActiveMeteorCount++;
                meteor.OnRecycle += OnRecycleHandler;
                meteor.SetEnableMovement(false);
                
                return meteor;
            }

            private void OnRecycleHandler(FlyingObjectView<MeteorData> input)
            {
                input.OnRecycle -= OnRecycleHandler;
                _pool.Release((MeteorView)input);
                ActiveMeteorCount--;
            }

            public void RecycleAll()
            {
                _pool.RecycleAll();
            }
        }

        #endregion
        
        #endregion
        
        [Header("Components")]
        [SerializeField] private ProjectileSpawnSettings spawnSettings;
        [SerializeField] private MeteorView meteorPrefab;
        [Header("Debug")] 
        [SerializeField] private bool doesDebug;
        
        private Spawner _spawner;
        private bool _isSpawningRing;
        
        private void Awake()
        {
            _spawner = new Spawner(meteorPrefab, 5);
            SetEventBus();
        }

        #region Spawn

        private float GetProjectileSpeed()
        {
            return GameConfigManager.Instance.GetGameplayData().ProjectileData.MaxProjectileSpeed;
        }

        private IMeteor CreateMeteor(ProjectileSpawnData data)
        {
            var finalSpeed = GetProjectileSpeed() * data.MovementMultiplier;
            var meteor = _spawner.Spawn();
            
            // Set Direction and Rotation towards the Center of Gravity
            
            float angle = Mathf.Atan2(data.Direction.y, data.Direction.x) * Mathf.Rad2Deg;
            var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            meteor.SetValues(new MeteorData
            {
                MovementSpeed = finalSpeed,
                Rotation = rotation,
                Position = data.Position,
                Direction = data.Direction.normalized,
                Value = data.Value
            });

            meteor.OnDeflection += Meteor_OnDeflectionHandler;
            meteor.OnEarthCollision += Meteor_OnCollisionHandler;
            
            if (meteor is IDebugMeteor debug)
            {
                debug.DebugEnable = doesDebug;
            }
            
            return meteor;
        }

        private void SpawnSingle(ProjectileSpawnData data)
        {
            var meteor = CreateMeteor(data);
            
            if (meteor is IProjectile projectile)
            {
                ProjectileEventCaller.Add((projectile));
            }
            else
            {
                Debug.LogWarning($"Projectile type {meteor} does not implement {nameof(IProjectile)}");
                meteor.Recycle();
            }
        }

        private void SpawnRing()
        {
            if(_isSpawningRing) return;

            StartCoroutine(CreateRing());
        }

        private IEnumerator CreateRing()
        {
            MeteorEventCaller.RingActive(true);
            var projectileData = GameConfigManager.Instance.GetGameplayData().ProjectileData;
            var ringValues = projectileData.MeteorRingData;

            _isSpawningRing = true;
            
            yield return new WaitUntil(()=> _spawner.ActiveMeteorCount == 0);
            
            var currAngle = 0f;
            var amountToSpawn = ringValues.MeteorAmount;
            var angleOffset = 360f / amountToSpawn;
            var startAngleOffset = angleOffset/2;
            var startOffset = 0f;
            var speedMultiplier = 2f;
            var valuePerMeteor = GetRingMeteorValue(amountToSpawn, ringValues.RingsAmount);

            ProjectileSpawnData spawnData = new()
            {
                MovementMultiplier = speedMultiplier
            };

            for (int a = 0; a < ringValues.WavesAmount; a++)
            {
                for (int i = 0; i < ringValues.RingsAmount; i++)
                {
                    for (int j = 0; j < amountToSpawn; j++)
                    {
                        yield return new WaitForSeconds(0.1f);
                        
                        var finalValue = j % 2 == 0 ? valuePerMeteor : 0;

                        spawnData.Position = spawnSettings.GetPositionByAngle(currAngle);
                        spawnData.Value = finalValue;
                        spawnData.Direction = (spawnSettings.GetCenterOfGravity() - spawnData.Direction).normalized;
                        
                       CreateMeteor(spawnData).SetEnableMovement(true);
                        
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
            
            yield return new WaitUntil(()=> _spawner.ActiveMeteorCount == 0);
            
            yield return new WaitForSeconds(projectileData.MeteorSpawnDelayAfterRing);
            
            MeteorEventCaller.RingActive(false);
            AbilitiesEventCaller.RunTimer();
            _isSpawningRing = false;
        }
        
        private float GetRingMeteorValue(int countPerWave, int waves)
        {
            var totalMeteor = (countPerWave * waves);
            var finalScoreValue = GetRingTargetScore() / totalMeteor;
            return finalScoreValue;
        }
        
        private float GetRingTargetScore() => GameParameters.GameplayValues.BaseMeteorValue * 30;
        
        private void RecycleAll()
        {
            _spawner.RecycleAll();
        }
        
        #endregion
        
        #region Meteor Handlers

        private void Meteor_OnCollisionHandler(IProjectile input1, MeteorCollisionData data)
        {
            if (input1 is IMeteor meteor)
            {
                meteor.OnDeflection -= Meteor_OnDeflectionHandler;
                meteor.OnEarthCollision -= Meteor_OnCollisionHandler;
            }
            
            ProjectileEventCaller.Collision(new CollisionData
            {
                Position = data.Position,
                Rotation = data.Rotation,
                Direction = data.Direction,
                Type = ProjectileType.Meteor
            });
        }

        private void Meteor_OnDeflectionHandler(IProjectile input1, MeteorCollisionData data)
        {
            if (input1 is IMeteor meteor)
            {
                meteor.OnDeflection -= Meteor_OnDeflectionHandler;
                meteor.OnEarthCollision -= Meteor_OnCollisionHandler;
            }
            
            ProjectileEventCaller.Deflected(new DeflectData
            {
                Position = data.Position,
                Rotation = data.Rotation,
                Direction = data.Direction,
                Value = data.Value,
                Type = ProjectileType.Meteor
            });
        }

        #endregion

        #region Event Bus

        private void SetEventBus()
        {
            MeteorEventSubscriber.SpawnRing(EnventBus_Meteor_SpawnRing);
            //
            ProjectileEventSubscriber.DisableSpawn(EventBus_Projectile_DisableSpawn);
            ProjectileEventSubscriber.Spawn(EventBus_Projectile_Spawn);
        }
        
        #region Meteor

        private void EnventBus_Meteor_SpawnRing(MeteorEvents.SpawnRing input)
        {
            SpawnRing();
        }

        #endregion
        
        #region Projectiles

        private void EventBus_Projectile_Spawn(ProjectileEvents.Spawn input)
        {
            if (input.ProjectileType == ProjectileType.Meteor)
            {
                SpawnSingle(new ProjectileSpawnData
                {
                    Position = input.Position,
                    Direction = input.Direction,
                    MovementMultiplier = input.MovementMultiplier
                });
            }
        }
        
        private void EventBus_Projectile_DisableSpawn(ProjectileEvents.DisableSpawn input)
        {
            RecycleAll();
        }

        #endregion

        #endregion
    }
}