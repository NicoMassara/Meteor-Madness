using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;

namespace _Main.Scripts
{
    public static class GameEventCaller
    {
        public static void Publish<T>(T eventData) where T : struct
        {
            GameManager.Instance.EventManager.Publish(eventData);
        }
        
        public static void Subscribe<T>(Action<T> listener) where T : struct
        {
            GameManager.Instance.EventManager.Subscribe(listener);
        }

        public static void Unsubscribe<T>(Action<T> listener) where T : struct
        {
            GameManager.Instance.EventManager.Unsubscribe(listener);
        }
    }

    public static class EarthEventCaller
    {
        public static void Restart()
        {
            GameEventCaller.Publish(new EarthEvents.Restart());
        }
        
        public static void RestartFinished()
        {
            GameEventCaller.Publish(new EarthEvents.RestartFinished());
        }
        
        public static void ShakeStart()
        {
            GameEventCaller.Publish(new EarthEvents.ShakeStart());
        }
        
        public static void Heal()
        {
            GameEventCaller.Publish(new EarthEvents.Heal());
        }

        public static void SetEnableDamage(bool enable)
        {
            GameEventCaller.Publish(new EarthEvents.SetEnableDamage{DamageEnable = enable});
        }
        
        public static void DestructionStart()
        {
            GameEventCaller.Publish(new EarthEvents.DestructionStart());
        }
        
        public static void DestructionFinished()
        {
            GameEventCaller.Publish(new EarthEvents.DestructionFinished());
        }
    }
    public static class ShieldEventCaller
    {
        public static void SetEnableShield(bool isEnabled)
        {
            GameEventCaller.Publish(new ShieldEvents.SetEnable{IsEnabled = isEnabled});
        }
        public static void SetGold(bool isActive)
        {
            GameEventCaller.Publish(new ShieldEvents.SetGold{IsActive = isActive});
        }
        public static void SetAutomatic(bool isActive)
        {
            GameEventCaller.Publish(new ShieldEvents.SetAutomatic{IsActive = isActive});
        }

        public static void EnableSuperShield()
        {
            GameEventCaller.Publish(new ShieldEvents.EnableSuperShield());
        }
        
        public static void EnableNormalShield()
        {
            GameEventCaller.Publish(new ShieldEvents.EnableNormalShield());
        }

    }
    public static class GameScreenEventCaller
    {
        public static void SetGameScreen(ScreenType type, bool isEnable = false)
        {
            GameEventCaller.Publish(new GameScreenEvents.SetGameScreen
            {
                ScreenType = type,
                IsEnable = isEnable
            });
        }
    }
    public static class GameModeEventCaller
    {
        public static void InitializeValues()
        {
            GameEventCaller.Publish(new GameModeEvents.InitializeValues());
        }
        
        public static void Disable()
        {
            GameEventCaller.Publish(new GameModeEvents.Disable());
        }
        
        public static void Start()
        {
            GameEventCaller.Publish(new GameModeEvents.Start());
        }
        
        public static void Finish()
        {
            GameEventCaller.Publish(new GameModeEvents.Finish());
        }
        
        public static void Restart()
        {
            GameEventCaller.Publish(new GameModeEvents.Restart());
        }

        public static void UpdateLevel(int currentLevel)
        {
            GameEventCaller.Publish(new GameModeEvents.UpdateLevel{CurrentLevel = currentLevel});
        }
        
        public static void SetPause(bool isPaused)
        {
            GameEventCaller.Publish(new GameModeEvents.SetPause{IsPaused = isPaused});
        }
        
    }
    public static class ProjectileEventCaller
    {
        public static void Collision(CollisionData data)
        {
            GameEventCaller.Publish(new ProjectileEvents.Collision
            {
                Position = data.Position,
                Rotation = data.Rotation,
                Direction = data.Direction,
                Type = data.Type
            });
        }
        
        public static void Deflected(DeflectData data)
        {
            GameEventCaller.Publish(new ProjectileEvents.Deflected
            {
                Position = data.Position,
                Rotation = data.Rotation,
                Direction = data.Direction,
                Value = data.Value,
                Type = data.Type
            });
        }

        public static void Add(IProjectile projectile)
        {
            GameEventCaller.Publish(new ProjectileEvents.Add{Projectile = projectile});
        }

        public static void RequestSpawn(ProjectileType projectileType)
        {
            GameEventCaller.Publish(new ProjectileEvents.RequestSpawn
            {
                ProjectileType = projectileType, 
                RequestType = EventRequestType.Request
            });
        }
        
        public static void GrantSpawn(ProjectileType projectileType)
        {
            GameEventCaller.Publish(new ProjectileEvents.RequestSpawn
            {
                ProjectileType = projectileType, 
                RequestType = EventRequestType.Granted
            });
        }

        public static void Spawn(ProjectileSpawnData data)
        {
            GameEventCaller.Publish(new ProjectileEvents.Spawn
            {
                ProjectileType = data.ProjectileType,
                Position = data.Position,
                Direction = data.Direction,
                MovementMultiplier = data.MovementMultiplier
            });
        }
    }
    public static class MeteorEventCaller
    {
        public static void GrantSpawnSingle()
        {
            ProjectileEventCaller.GrantSpawn(ProjectileType.Meteor);
        }
        
        public static void RequestSpawnSingle()
        {
            ProjectileEventCaller.RequestSpawn(ProjectileType.Meteor);
        }
        
        public static void SpawnRing()
        {
            GameEventCaller.Publish(new MeteorEvents.SpawnRing());
        }

        public static void RingActive(bool isActive)
        {
            GameEventCaller.Publish(new MeteorEvents.RingActive{IsActive = isActive});
        }

        public static void RecycleAll()
        {
            GameEventCaller.Publish(new MeteorEvents.RecycleAll());
        }

        public static void EnableSpawn(bool canSpawn)
        {
            GameEventCaller.Publish(new MeteorEvents.EnableSpawn{CanSpawn = canSpawn});
        }
    }
    public static class ParticleEventCaller
    {
        public static void Spawn(ParticleSpawnData data)
        {
            GameEventCaller.Publish(new ParticleEvents.Spawn
            {
                ParticleData = data.ParticleData,
                Position = data.Position,
                Rotation = data.Rotation,
                MoveDirection = data.MoveDirection,
            });
        }
    }
    public static class CameraEventCaller
    {
        public static void ZoomIn()
        {
            GameEventCaller.Publish(new CameraEvents.ZoomIn());
        }
        
        public static void ZoomOut()
        {
            GameEventCaller.Publish(new CameraEvents.ZoomOut());
        }

        public static void Shake(IShakeData shake)
        {
            GameEventCaller.Publish(new CameraEvents.Shake{ShakeData = shake});
        }
    }
    public static class InputsEventCaller
    {
        public static void SetEnable(bool enable)
        {
            GameEventCaller.Publish(new InputsEvents.SetEnable{IsEnable = enable});
        }
    }
    public static class AbilitiesEventCaller
    {
        public static void SetCanUse(bool canUse)
        {
            GameEventCaller.Publish(new AbilitiesEvents.SetCanUse{CanUse = canUse});
        }

        public static void Add(AbilityAddData data)
        {
            GameEventCaller.Publish(new AbilitiesEvents.Add
            {
                AbilityType = data.AbilityType,
                Position = data.Position
            });
        }

        public static void SetStorageFull(bool isFull)
        {
            GameEventCaller.Publish(new AbilitiesEvents.SetStorageFull{IsFull = isFull});
        }
        
        public static void SetActive(SetActiveAbilityData data)
        {
            GameEventCaller.Publish(new AbilitiesEvents.SetActive
            {
                AbilityType = data.AbilityType,
                IsActive = data.IsActive,
            });
        }
        
        public static void GrantSpawn()
        {
            GameEventCaller.Publish(new ProjectileEvents.RequestSpawn
            {
                ProjectileType = ProjectileType.AbilitySphere, 
                RequestType = EventRequestType.Granted
            });
        }
        
        public static void RequestSpawn()
        {
            GameEventCaller.Publish(new ProjectileEvents.RequestSpawn
            {
                ProjectileType = ProjectileType.AbilitySphere, 
                RequestType = EventRequestType.Request
            });
        }
    }
    public static class FloatingTextEventCaller
    {
        public static void Spawn(FloatingTextValues data)
        {
            GameEventCaller.Publish(new FloatingTextEvents.Spawn { Data = data });
        }
    }
}