using System;
using _Main.Scripts.EventBus;
using _Main.Scripts.Gameplay.Projectile.SO;
using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projecitle.Spawner
{
    internal class ProjectileSpawnerSetup : MonoBehaviour
    {
        [SerializeField] private ProjectileSpawnDataSo spawnData;
        [SerializeField] private ProjectileRingDataSo ringData;
        private ProjectileSpawnerController.IProjectileSpawnerController _controller;
        private ProjectileSpawnerView.IProjectileSpawnerView _view;

        private void Awake()
        {
            if (spawnData == null)
            {
                Debug.LogError($"Spawn Data So not selected.");
                return;
            }

            var motor = new ProjectileSpawnerMotor(spawnData,ringData);
            _controller = new ProjectileSpawnerController(motor);
            
            _view = GetComponentInChildren<ProjectileSpawnerView.IProjectileSpawnerView>();
            motor.Subscribe((IObserver)_view);
            
            SetEventBus();
        }

        private void Start()
        {
            SetViewHandlers();
        }

        private void SetViewHandlers()
        {
            _view.OnBatchSpawned += () => _controller.NotifyBatchSpawned();
            _view.OnProjectileReachedTargetRatio += () => _controller.NotifyProjectileHasReachedTargetRatio();
        }


        private void SetEventBus()
        {
            ProjectileEventSubscriber.EnableSpawn(EventBus_Projectile_Enable);
            ProjectileEventSubscriber.DisableSpawn(EventBus_Projectile_Disable);
            ProjectileEventSubscriber.UpdateLevel(EventBus_Projectile_UpdateLevel);
            ProjectileEventSubscriber.Deflected(EventBus_Projectile_Deflected);
            ProjectileEventSubscriber.Collision(EventBus_Projectile_Collision);
        }

        private void EventBus_Projectile_Collision(ProjectileEvents.Collision obj)
        {
            _controller.NotifyProjectileCollision();
        }

        private void EventBus_Projectile_Deflected(ProjectileEvents.Deflected input)
        {
            _controller.NotifyProjectileDeflected();
        }

        private void EventBus_Projectile_UpdateLevel(ProjectileEvents.UpdateLevel input)
        {
            _controller.UpdateLevel(input.Level);
        }

        private void EventBus_Projectile_Enable(ProjectileEvents.EnableSpawn input)
        {
            _controller.EnableSpawn();
        }
        
        private void EventBus_Projectile_Disable(ProjectileEvents.DisableSpawn input)
        {
            _controller.DisableSpawn(input.DoesClearProjectiles);
        }
    }
}