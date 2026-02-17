using _Main.Scripts.EventBus;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projecitle.Spawner
{
    internal class ProjectileSpawnerSetup : MonoBehaviour
    {
        [SerializeField] private BatchTypeData data;
        
        private ProjectileSpawnerController.IProjectileSpawnerController _controller;
        private ProjectileSpawnerView.IProjectileSpawnerView _view;

        private void Awake()
        {
            var motor = new ProjectileSpawnerMotor(data);
            _controller = new ProjectileSpawnerController(motor);
            
            _view = GetComponentInChildren<ProjectileSpawnerView.IProjectileSpawnerView>();
            motor.Subscribe((IObserver)_view);
            
            
            BootEvents.OnSubSystemRequestInitialize += Initialize;
            
            SetEventBus();
        }

        private void Initialize()
        {
            BootEvents.OnSubSystemRequestInitialize -= Initialize;
            _controller.InitializeSpawner();
            BootEvents.SubSystemInitialized();
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
            ProjectileEventSubscriber.SetSpawnType(EventBus_Projectile_SetSpawnTyp);
            
        }

        private void EventBus_Projectile_SetSpawnTyp(ProjectileEvents.SetSpawnType input)
        {
            _controller.ChangeBatchType(input.BatchType);
        }


        private void EventBus_Projectile_Collision(ProjectileEvents.Collision input)
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