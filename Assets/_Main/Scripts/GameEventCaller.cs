using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using UnityEngine;

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

    #region GameMode
    
    public static class GameModeEventCaller
    {
        public static void InitializeValues()
        {
            GameEventCaller.Publish(new GameModeEvents.InitializeValues());
        }
        
        public static void SetPause(bool isPaused)
        {
            GameEventCaller.Publish(new GameModeEvents.SetPause{IsPaused = isPaused});
        }

        public static void SetEnablePause(bool isEnable)
        {
            GameEventCaller.Publish(new GameModeEvents.SetEnablePause{CanPause = isEnable});
        }

    }

    public static class GameModeEventSubscriber
    {
        public static void InitializeValues(Action<GameModeEvents.InitializeValues> action)
        {
            GameEventCaller.Subscribe<GameModeEvents.InitializeValues>(action);
        }
        
        public static void SetPause(Action<GameModeEvents.InitializeValues> action)
        {
            GameEventCaller.Subscribe<GameModeEvents.InitializeValues>(action);
        }
        
        public static void SetEnablePause(Action<GameModeEvents.SetEnablePause> action)
        {
            GameEventCaller.Subscribe(action);
        }
    }
    
    public static class GameModeEventUnSubscriber
    {
        public static void InitializeValues(Action<GameModeEvents.InitializeValues> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void SetPause(Action<GameModeEvents.InitializeValues> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void SetEnablePause(Action<GameModeEvents.SetEnablePause> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
    }
    
    
    #endregion
    
    #region Earth

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
        
        public static void SetToDefault()
        {
            GameEventCaller.Publish(new EarthEvents.SetToDefault());
        }
        
        public static void DestructionFinished()
        {
            GameEventCaller.Publish(new EarthEvents.DestructionFinished());
        }
        
        public static void Death()
        {
            GameEventCaller.Publish(new EarthEvents.Death());
        }
    }
    
    public static class EarthEventSubscriber
    {
        public static void Restart(Action<EarthEvents.Restart> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void RestartFinished(Action<EarthEvents.RestartFinished> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void ShakeStart(Action<EarthEvents.ShakeStart> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void Heal(Action<EarthEvents.Heal> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void SetEnableDamage(Action<EarthEvents.SetEnableDamage> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void DestructionStart(Action<EarthEvents.DestructionStart> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void SetToDefault(Action<EarthEvents.SetToDefault> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void DestructionFinished(Action<EarthEvents.DestructionFinished> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void Death(Action<EarthEvents.Death> action)
        {
            GameEventCaller.Subscribe(action);
        }
    }
    
    public static class EarthEventUnSubscriber
    {
        public static void Restart(Action<EarthEvents.Restart> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void RestartFinished(Action<EarthEvents.RestartFinished> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void ShakeStart(Action<EarthEvents.ShakeStart> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void Heal(Action<EarthEvents.Heal> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void SetEnableDamage(Action<EarthEvents.SetEnableDamage> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void DestructionStart(Action<EarthEvents.DestructionStart> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void SetToDefault(Action<EarthEvents.SetToDefault> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void DestructionFinished(Action<EarthEvents.DestructionFinished> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void Death(Action<EarthEvents.Death> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
    }
    
    #endregion
    
    #region Shield
    

    public static class ShieldEventCaller
    {
        public static void SetGold(bool isActive)
        {
            GameEventCaller.Publish(new ShieldEvents.SetGold{IsActive = isActive});
        }

        public static void SetSlow(bool isActive)
        {
            GameEventCaller.Publish(new ShieldEvents.SetSlow{IsActive = isActive});
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
        
        public static void RestartPosition()
        {
            GameEventCaller.Publish(new ShieldEvents.RestartPosition());
        }
        
        public static void Enable()
        {
            GameEventCaller.Publish(new ShieldEvents.Enable());
        }
        
        public static void Disable()
        {
            GameEventCaller.Publish(new ShieldEvents.Disable());
        }
    }
    
    public static class ShieldEventSubscriber
    {
        public static void SetGold(Action<ShieldEvents.SetGold> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void SetSlow(Action<ShieldEvents.SetSlow> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void SetAutomatic(Action<ShieldEvents.SetAutomatic> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void EnableSuperShield(Action<ShieldEvents.EnableSuperShield> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void EnableNormalShield(Action<ShieldEvents.EnableNormalShield> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void RestartPosition(Action<ShieldEvents.RestartPosition> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void Enable(Action<ShieldEvents.Enable> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void Disable(Action<ShieldEvents.Disable> action)
        {
            GameEventCaller.Subscribe(action);
        }
    }
    
    public static class ShieldEventUnSubscriber
    {
        public static void SetGold(Action<ShieldEvents.SetGold> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void SetSlow(Action<ShieldEvents.SetSlow> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void SetAutomatic(Action<ShieldEvents.SetAutomatic> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void EnableSuperShield(Action<ShieldEvents.EnableSuperShield> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void EnableNormalShield(Action<ShieldEvents.EnableNormalShield> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void RestartPosition(Action<ShieldEvents.RestartPosition> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void Enable(Action<ShieldEvents.Enable> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void Disable(Action<ShieldEvents.Disable> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
    }
    
    #endregion
    
    #region GameScreen

    public static class GameScreenEventCaller
    {
        public static void SetGameScreen(ScreenType type, bool isEnable = false)
        {
            GameEventCaller.Publish(new GameScreenEvents.SetScreen
            {
                ScreenType = type,
                IsEnable = isEnable
            });
        }

        public static void EnableScreen(ScreenType type, EventRequestType requestType)
        {
            GameEventCaller.Publish(new GameScreenEvents.EnableScreen
            {
                ScreenType = type,
                RequestType = requestType
            });
        }
        
        public static void DisableScreen(ScreenType type, EventRequestType requestType)
        {
            GameEventCaller.Publish(new GameScreenEvents.DisableScreen
            {
                ScreenType = type,
                RequestType = requestType
            });
        }

        public static void LoadLastScreen()
        {
            GameEventCaller.Publish(new GameScreenEvents.LastScreen());
        }

    }
    
    public static class GameScreenEventSubscriber
    {
        public static void SetGameScreen(Action<GameScreenEvents.SetScreen> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void EnableScreen(Action<GameScreenEvents.EnableScreen> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void DisableScreen(Action<GameScreenEvents.DisableScreen> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void GoToLastScreen(Action<GameScreenEvents.LastScreen> action)
        {
            GameEventCaller.Subscribe(action);
        }
    }
    
    public static class GameScreenEventUnSubscriber
    {
        public static void SetGameScreen(Action<GameScreenEvents.SetScreen> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void EnableScreen(Action<GameScreenEvents.EnableScreen> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void DisableScreen(Action<GameScreenEvents.DisableScreen> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void GoToLastScreen(Action<GameScreenEvents.LastScreen> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
    }
    
    #endregion
    
    #region Projectile
    
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
                RequestType = EventRequestType.Requested
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

        public static void ClearQueue()
        {
            GameEventCaller.Publish(new ProjectileEvents.ClearQueue());
        }
        
        public static void DisableSpawn()
        {
            GameEventCaller.Publish(new ProjectileEvents.DisableSpawn());
        }
        
        public static void EnableSpawn()
        {
            GameEventCaller.Publish(new ProjectileEvents.EnableSpawn());
        }
        
        public static void UpdateLevel(int level)
        {
            GameEventCaller.Publish(new ProjectileEvents.UpdateLevel{Level = level});
        }
    }
    
    public static class ProjectileEventSubscriber
    {
        public static void Collision(Action<ProjectileEvents.Collision> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void Deflected(Action<ProjectileEvents.Deflected> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void Add(Action<ProjectileEvents.Add> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void RequestSpawn(Action<ProjectileEvents.RequestSpawn> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void GrantSpawn(Action<ProjectileEvents.RequestSpawn> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void Spawn(Action<ProjectileEvents.Spawn> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void ClearQueue(Action<ProjectileEvents.ClearQueue> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void DisableSpawn(Action<ProjectileEvents.DisableSpawn> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void EnableSpawn(Action<ProjectileEvents.EnableSpawn> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void UpdateLevel(Action<ProjectileEvents.UpdateLevel> action)
        {
            GameEventCaller.Subscribe(action);
        }
    }
    
    public static class ProjectileEventUnSubscriber
    {
        public static void Collision(Action<ProjectileEvents.Collision> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void Deflected(Action<ProjectileEvents.Deflected> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void Add(Action<ProjectileEvents.Add> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void RequestSpawn(Action<ProjectileEvents.RequestSpawn> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void GrantSpawn(Action<ProjectileEvents.RequestSpawn> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void Spawn(Action<ProjectileEvents.Spawn> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void ClearQueue(Action<ProjectileEvents.ClearQueue> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void DisableSpawn(Action<ProjectileEvents.DisableSpawn> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void EnableSpawn(Action<ProjectileEvents.EnableSpawn> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void UpdateLevel(Action<ProjectileEvents.UpdateLevel> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
    }
    
    #endregion
    
    #region Meteor

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
    }
    
    public static class MeteorEventSubscriber
    {
        public static void GrantSpawnSingle(Action<ProjectileEvents.RequestSpawn> action)
        {
            ProjectileEventSubscriber.GrantSpawn(action);
        }
        
        public static void RequestSpawnSingle(Action<ProjectileEvents.RequestSpawn> action)
        {
            ProjectileEventSubscriber.RequestSpawn(action);
        }
        
        public static void SpawnRing(Action<MeteorEvents.SpawnRing> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void RingActive(Action<MeteorEvents.RingActive> action)
        {
            GameEventCaller.Subscribe(action);
        }
    }
    
    public static class MeteorEventUnSubscriber
    {
        public static void GrantSpawnSingle(Action<ProjectileEvents.RequestSpawn> action)
        {
            ProjectileEventSubscriber.GrantSpawn(action);
        }
        
        public static void RequestSpawnSingle(Action<ProjectileEvents.RequestSpawn> action)
        {
            ProjectileEventSubscriber.RequestSpawn(action);
        }
        
        public static void SpawnRing(Action<MeteorEvents.SpawnRing> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void RingActive(Action<MeteorEvents.RingActive> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
    }
    
    #endregion
    
    #region Particle
    

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
    
    
    public static class ParticleEventSubscriber
    {
        public static void Spawn(Action<ParticleEvents.Spawn> action)
        {
            GameEventCaller.Subscribe(action);
        }
    }
    
    public static class ParticleEventUnSubscribe
    {
        public static void Spawn(Action<ParticleEvents.Spawn> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
    }
    

    
    #endregion
    
    #region Camera
    

    public static class CameraEventCaller
    {
        public static void ZoomIn(float timeToZoom = 0.1f)
        {
            GameEventCaller.Publish(new CameraEvents.ZoomIn{TimeToZoom = timeToZoom});
        }
        
        public static void ZoomOut(float timeToZoom = 0.1f)
        {
            GameEventCaller.Publish(new CameraEvents.ZoomOut{TimeToZoom = timeToZoom});
        }

        public static void Shake(IShakeData shake)
        {
            GameEventCaller.Publish(new CameraEvents.Shake{ShakeData = shake});
        }

        public static void LookCenter(float timeToLook = 0.5f)
        {
            GameEventCaller.Publish(new CameraEvents.LookCenter{TimeToLook = timeToLook});
        }
        public static void LookRight(float timeToLook = 0.5f)
        {
            GameEventCaller.Publish(new CameraEvents.LookRight{TimeToLook = timeToLook});
        }
        public static void LookLeft(float timeToLook = 0.5f)
        {
            GameEventCaller.Publish(new CameraEvents.LookLeft{TimeToLook = timeToLook});
        }
        public static void LookUp(float timeToLook = 0.5f)
        {
            GameEventCaller.Publish(new CameraEvents.LookUp{TimeToLook = timeToLook});
        }
        public static void LookDown(float timeToLook = 0.5f)
        {
            GameEventCaller.Publish(new CameraEvents.LookDown{TimeToLook = timeToLook});
        }
    }
    
    public static class CameraEventSubscriber
    {
        public static void ZoomIn(Action<CameraEvents.ZoomIn> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void ZoomOut(Action<CameraEvents.ZoomOut> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void Shake(Action<CameraEvents.Shake> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void LookCenter(Action<CameraEvents.LookCenter> action)
        {
            GameEventCaller.Subscribe(action);
        }
        public static void LookRight(Action<CameraEvents.LookRight> action)
        {
            GameEventCaller.Subscribe(action);
        }
        public static void LookLeft(Action<CameraEvents.LookLeft> action)
        {
            GameEventCaller.Subscribe(action);
        }
        public static void LookUp(Action<CameraEvents.LookUp> action)
        {
            GameEventCaller.Subscribe(action);
        }
        public static void LookDown(Action<CameraEvents.LookDown> action)
        {
            GameEventCaller.Subscribe(action);
        }
    }
    
    public static class CameraEventUnSubscriber
    {
        public static void ZoomIn(Action<CameraEvents.ZoomIn> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void ZoomOut(Action<CameraEvents.ZoomOut> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void Shake(Action<CameraEvents.Shake> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void LookCenter(Action<CameraEvents.LookCenter> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        public static void LookRight(Action<CameraEvents.LookRight> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        public static void LookLeft(Action<CameraEvents.LookLeft> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        public static void LookUp(Action<CameraEvents.LookUp> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        public static void LookDown(Action<CameraEvents.LookDown> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
    }

    #endregion
    
    #region Inputs
    

    public static class InputsEventCaller
    {
        public static void SetEnable(bool enable)
        {
            GameEventCaller.Publish(new InputsEvents.SetEnable{IsEnable = enable});
        }

        public static void SetUIEnable(bool enable)
        {
            GameEventCaller.Publish(new InputsEvents.SetUIEnable{IsEnable = enable});
        }
    }
    
    
    public static class InputsEventSubscriber
    {
        public static void SetEnable(Action<InputsEvents.SetEnable> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void SetUIEnable(Action<InputsEvents.SetUIEnable> action)
        {
            GameEventCaller.Subscribe(action);
        }
    }
    
    public static class InputsEventUnSubscriber
    {
        public static void SetEnable(Action<InputsEvents.SetEnable> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void SetUIEnable(Action<InputsEvents.SetUIEnable> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
    }
    

    #endregion
    
    #region Abilities
    
    public static class AbilitiesEventCaller
    {
        public static void SetCanUse(bool canUse)
        {
            GameEventCaller.Publish(new AbilitiesEvents.SetCanUse{CanUse = canUse});
        }

        public static void SetEnableUI(bool isEnable)
        {
            GameEventCaller.Publish(new AbilitiesEvents.SetEnableUI{IsEnable = isEnable});
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
        
        public static void NotifyIsActive(AbilityType type, bool isActive)
        {
            GameEventCaller.Publish(new AbilitiesEvents.NotifyIsActive
            {
                AbilityType = type,
                IsActive = isActive,
            });
        }

        public static void Enable()
        {
            GameEventCaller.Publish(new AbilitiesEvents.Enable());
        }
        
        public static void Disable()
        {
            GameEventCaller.Publish(new AbilitiesEvents.Disable());
        }
        
        public static void RunTimer()
        {
            GameEventCaller.Publish(new AbilitiesEvents.RunTimer());
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
                RequestType = EventRequestType.Requested
            });
        }

        public static void SetNextSpawn(AbilityType type)
        {
            GameEventCaller.Publish(new AbilitiesEvents.SetNextSpawn{AbilityType = type});
        }
    }
    public static class AbilitiesEventSubscriber
    {
        public static void SetCanUse(Action<AbilitiesEvents.SetCanUse> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void SetEnableUI(Action<AbilitiesEvents.SetEnableUI> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void Add(Action<AbilitiesEvents.Add> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void SetStorageFull(Action<AbilitiesEvents.SetStorageFull> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void NotifyIsActive(Action<AbilitiesEvents.NotifyIsActive> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void Enable(Action<AbilitiesEvents.Enable> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void Disable(Action<AbilitiesEvents.Disable> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void RunTimer(Action<AbilitiesEvents.RunTimer> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void GrantSpawn(Action<ProjectileEvents.RequestSpawn> action)
        {
            GameEventCaller.Subscribe(action);
        }
        
        public static void RequestSpawn(Action<ProjectileEvents.RequestSpawn> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void SetNextSpawn(Action<AbilitiesEvents.SetNextSpawn> action)
        {
            GameEventCaller.Subscribe(action);
        }
    }
    public static class AbilitiesEventUnSubscriber
    {
        public static void SetCanUse(Action<AbilitiesEvents.SetCanUse> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void SetEnableUI(Action<AbilitiesEvents.SetEnableUI> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void Add(Action<AbilitiesEvents.Add> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void SetStorageFull(Action<AbilitiesEvents.SetStorageFull> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void NotifyIsActive(Action<AbilitiesEvents.NotifyIsActive> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void Enable(Action<AbilitiesEvents.Enable> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void Disable(Action<AbilitiesEvents.Disable> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void RunTimer(Action<AbilitiesEvents.RunTimer> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void GrantSpawn(Action<ProjectileEvents.RequestSpawn> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
        
        public static void RequestSpawn(Action<ProjectileEvents.RequestSpawn> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void SetNextSpawn(Action<AbilitiesEvents.SetNextSpawn> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
    }

    #endregion
    
    #region Floatin Text
    

    public static class FloatingTextEventCaller
    {
        public static void Spawn(FloatingTextValues data)
        {
            GameEventCaller.Publish(new FloatingTextEvents.Spawn { Data = data });
        }
    }
    
    public static class FloatingTextEventSubscriber
    {
        public static void Spawn(Action<FloatingTextEvents.Spawn> action)
        {
            GameEventCaller.Subscribe(action);
        }
    }
    
    public static class FloatingTextEventUnSubscriber
    {
        public static void Spawn(Action<FloatingTextEvents.Spawn> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
    }
    
    #endregion
    
    #region MultiPage
    

    public static class MultiPageUIEventCaller
    {
        public static void Create(IMultiPageData data, ulong createId)
        {
            GameEventCaller.Publish(new MultiPageUIEvents.Create{Data = data, CreateId = createId});
        }

        public static void Finished(ulong createId)
        {
            GameEventCaller.Publish(new MultiPageUIEvents.Finished{CreateId = createId});
        }
    }
    
    public static class MultiPageUIEventSubscriber
    {
        public static void Create(Action<MultiPageUIEvents.Create> action)
        {
            GameEventCaller.Subscribe(action);
        }

        public static void Finished(Action<MultiPageUIEvents.Finished> action)
        {
            GameEventCaller.Subscribe(action);
        }
    }
    
    public static class MultiPageUIEventUnSubscriber
    {
        public static void Create(Action<MultiPageUIEvents.Create> action)
        {
            GameEventCaller.Unsubscribe(action);
        }

        public static void Finished(Action<MultiPageUIEvents.Finished> action)
        {
            GameEventCaller.Unsubscribe(action);
        }
    }
    
    #endregion
    
    
}