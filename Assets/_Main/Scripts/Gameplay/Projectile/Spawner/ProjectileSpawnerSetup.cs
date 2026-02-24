using System;
using _Main.Scripts.EventBus;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projecitle.Spawner
{
    internal class ProjectileSpawnerSetup : MonoBehaviour
    {
        [SerializeField] private BatchTypeData data;

        private IProjectileSpawnerView _view;
        private ProjectileSpawnerController _controller;

        private void Awake()
        {
            _view = GetComponent<IProjectileSpawnerView>();

            if (_view == null)
            {
                throw new Exception("View component not attached.");
            }

            var motor = new ProjectileSpawnerMotor(data);
            
            motor.Subscribe(_view);

            _controller = new ProjectileSpawnerController(motor);
            
            BootEvents.OnSubSystemRequestInitialize += Initialize;
            SetEventBus();
            SetViewHandlers();
        }

        private void Initialize()
        {
            BootEvents.OnSubSystemRequestInitialize -= Initialize;
            BootEvents.SubSystemInitialized();
            _controller.Initialize();
        }

        private void SetViewHandlers()
        {
            _view.OnProjectileReachedTarget += (isLast) =>
            {
                if(isLast)
                    _controller.NotifyLastProjectileReachedTarget();
                else
                    _controller.NotifyProjectileReachedTarget();
            };
        }
        
        #region Event Bus

        private void SetEventBus()
        {
            ProjectileSpawner.Subscribe.Enable(EventBus_ProjectileSpawner_Enable);
            ProjectileSpawner.Subscribe.Disable(EventBus_ProjectileSpawner_Disable);
            ProjectileSpawner.Subscribe.Clear(EventBus_ProjectileSpawner_Clear);
            ProjectileSpawner.Subscribe.RestartValues(EventBus_ProjectileSpawner_RestartValues);
            ProjectileSpawner.Subscribe.RequestSpawn(EventBus_ProjectileSpawner_RequestSpawn);
            ProjectileSpawner.Subscribe.SetLevel(EventBus_ProjectileSpawner_SetLevel);
            ProjectileSpawner.Subscribe.SetEnableAbilitySpawn(EventBus_ProjectileSpawner_SetEnableAbilitySpawn);
            ProjectileEventSubscriber.Collision(EventBus_Projectile_Collision);
            ProjectileEventSubscriber.Deflected(EventBus_Projectile_Deflected);
            ProjectileEventSubscriber.Deflected(EventBus_Projectile_Deflected);
        }



        #region Enable/Disable
        
        private void EventBus_ProjectileSpawner_SetEnableAbilitySpawn(ProjectileSpawnerEvents.SetEnableAbilitySpawn input)
        {
            _controller.SetEnableAbilitySpawn(input.IsEnable);
        }

        private void EventBus_ProjectileSpawner_RestartValues(ProjectileSpawnerEvents.RestartValues input)
        {
            _controller.RestartValues();
        }

        private void EventBus_ProjectileSpawner_Clear(ProjectileSpawnerEvents.Clear input)
        {
            _controller.Clear();
        }

        private void EventBus_ProjectileSpawner_Disable(ProjectileSpawnerEvents.Disable input)
        {
            _controller.Disable();
        }

        private void EventBus_ProjectileSpawner_Enable(ProjectileSpawnerEvents.Enable input)
        {
            _controller.Enable();
        }
        
        private void EventBus_ProjectileSpawner_SetLevel(ProjectileSpawnerEvents.SetLevel input)
        {
            _controller.UpdateLevel(input.Level);
        }
        
        private void EventBus_Projectile_Deflected(ProjectileEvents.Deflected input)
        {
            _controller.NotifyProjectileDeflected();
        }

        private void EventBus_Projectile_Collision(ProjectileEvents.Collision input)
        {
            _controller.NotifyProjectileDestroyed();
        }

        #endregion

        #region Spawned

        private void EventBus_ProjectileSpawner_RequestSpawn(ProjectileSpawnerEvents.RequestSpawn input)
        {
            _controller.CreateBatch(input.BatchType);
        }
        
        #endregion
        
        #endregion
    }
}