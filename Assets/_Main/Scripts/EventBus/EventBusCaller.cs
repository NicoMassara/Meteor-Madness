using System;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;

namespace _Main.Scripts.EventBus
{
    public class EventBusCaller
    {
        public static void Publish<T>(T eventData) where T : struct
        {
            EventBusManager.Instance.Publish(eventData);
        }
        
        public static void Subscribe<T>(Action<T> listener) where T : struct
        {
            EventBusManager.Instance.Subscribe(listener);
        }

        public static void Unsubscribe<T>(Action<T> listener) where T : struct
        {
            EventBusManager.Instance.Unsubscribe(listener);
        }
    }
    
    #region GameMode
    
    public static class GameModeEventCaller
    {
        public static void InitializeValues()
        {
            EventBusCaller.Publish(new GameModeEvents.InitializeValues());
        }
        
        public static void SetPause(bool isPaused)
        {
            EventBusCaller.Publish(new GameModeEvents.SetPause{IsPaused = isPaused});
        }

        public static void SetEnablePause(bool isEnable)
        {
            EventBusCaller.Publish(new GameModeEvents.SetEnablePause{CanPause = isEnable});
        }

    }

    public static class GameModeEventSubscriber
    {
        public static void InitializeValues(Action<GameModeEvents.InitializeValues> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void SetPause(Action<GameModeEvents.SetPause> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void SetEnablePause(Action<GameModeEvents.SetEnablePause> action)
        {
            EventBusCaller.Subscribe(action);
        }
    }
    
    public static class GameModeEventUnSubscriber
    {
        public static void InitializeValues(Action<GameModeEvents.InitializeValues> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void SetPause(Action<GameModeEvents.SetPause> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void SetEnablePause(Action<GameModeEvents.SetEnablePause> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
    }
    
    
    #endregion
    
    #region Earth

    public static class EarthEventCaller
    {
        public static void Restart()
        {
            EventBusCaller.Publish(new EarthEvents.Restart());
        }
        
        public static void RestartFinished()
        {
            EventBusCaller.Publish(new EarthEvents.RestartFinished());
        }
        
        public static void ShakeStart()
        {
            EventBusCaller.Publish(new EarthEvents.ShakeStart());
        }
        
        public static void Heal()
        {
            EventBusCaller.Publish(new EarthEvents.Heal());
        }
        
        public static void EnableDamage()
        {
            EventBusCaller.Publish(new EarthEvents.EnableDamage());
        }
        
        public static void DisableDamage()
        {
            EventBusCaller.Publish(new EarthEvents.DisableDamage());
        }
        
        public static void DestructionStart()
        {
            EventBusCaller.Publish(new EarthEvents.DestructionStart());
        }
        
        public static void DestructionFinished()
        {
            EventBusCaller.Publish(new EarthEvents.DestructionFinished());
        }
        
        public static void Death()
        {
            EventBusCaller.Publish(new EarthEvents.Death());
        }
        
        public static void PreSlice()
        {
            EventBusCaller.Publish(new EarthEvents.PreSlice());
        }
    }
    
    public static class EarthEventSubscriber
    {
        public static void Restart(Action<EarthEvents.Restart> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void RestartFinished(Action<EarthEvents.RestartFinished> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void ShakeStart(Action<EarthEvents.ShakeStart> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void Heal(Action<EarthEvents.Heal> action)
        {
            EventBusCaller.Subscribe(action);
        }

        public static void EnableDamage(Action<EarthEvents.EnableDamage> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void DisableDamage(Action<EarthEvents.DisableDamage> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void DestructionStart(Action<EarthEvents.DestructionStart> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void DestructionFinished(Action<EarthEvents.DestructionFinished> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void Death(Action<EarthEvents.Death> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void PreSlice(Action<EarthEvents.PreSlice> action)
        {
            EventBusCaller.Subscribe(action);
        }
    }
    
    public static class EarthEventUnSubscriber
    {
        public static void Restart(Action<EarthEvents.Restart> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void RestartFinished(Action<EarthEvents.RestartFinished> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void ShakeStart(Action<EarthEvents.ShakeStart> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void Heal(Action<EarthEvents.Heal> action)
        {
            EventBusCaller.Unsubscribe(action);
        }

        public static void EnableDamage(Action<EarthEvents.EnableDamage> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void DisableDamage(Action<EarthEvents.DisableDamage> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void DestructionStart(Action<EarthEvents.DestructionStart> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void DestructionFinished(Action<EarthEvents.DestructionFinished> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void Death(Action<EarthEvents.Death> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void PreSlice(Action<EarthEvents.PreSlice> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
    }
    
    #endregion
    
    #region Shield
    
    public static class ShieldEventCaller
    {
        public static void RequestEnableShieldType(ShieldType type)
        {
            EventBusCaller.Publish(new ShieldEvents.RequestEnableShieldType{Type = type});
        }
        
        public static void RequestDisableShieldType(ShieldType type)
        {
            EventBusCaller.Publish(new ShieldEvents.RequestDisableShieldType{Type = type});
        }
        
        public static void NotifyShieldTypeEnabled(ShieldType type)
        {
            EventBusCaller.Publish(new ShieldEvents.NotifyShieldTypeEnabled{Type = type});
        }
        
        public static void NotifyShieldTypeDisabled(ShieldType type)
        {
            EventBusCaller.Publish(new ShieldEvents.NotifyShieldTypeDisabled{Type = type});
        }
        
        public static void Enable()
        {
            EventBusCaller.Publish(new ShieldEvents.Enable());
        }
        public static void Disable()
        {
            EventBusCaller.Publish(new ShieldEvents.Disable());
        }

        public static void NotifyMovement(int value)
        {
            EventBusCaller.Publish(new ShieldEvents.NotifyMovement{Direction = value});
        }
    }
    
    public static class ShieldEventSubscriber
    {
        public static void RequestEnableShieldType(Action<ShieldEvents.RequestEnableShieldType> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void RequestDisableShieldType(Action<ShieldEvents.RequestDisableShieldType> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void NotifyShieldTypeEnabled(Action<ShieldEvents.NotifyShieldTypeEnabled> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void NotifyShieldTypeDisabled(Action<ShieldEvents.NotifyShieldTypeDisabled> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void Enable(Action<ShieldEvents.Enable> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void Disable(Action<ShieldEvents.Disable> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void NotifyMovement(Action<ShieldEvents.NotifyMovement> action)
        {
            EventBusCaller.Subscribe(action);
        }
    }
    
    public static class ShieldEventUnSubscriber
    {
        public static void RequestEnableShieldType(Action<ShieldEvents.RequestEnableShieldType> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void RequestDisableShieldType(Action<ShieldEvents.RequestDisableShieldType> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void NotifyShieldTypeEnabled(Action<ShieldEvents.NotifyShieldTypeEnabled> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void NotifyShieldTypeDisabled(Action<ShieldEvents.NotifyShieldTypeDisabled> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void Enable(Action<ShieldEvents.Enable> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void Disable(Action<ShieldEvents.Disable> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void NotifyMovement(Action<ShieldEvents.NotifyMovement> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
    }
    
    #endregion
    
    #region GameScreen

    public static class GameScreenEventCaller
    {
        public static void EnableScreen(ScreenType currentScreen,EventRequestType requestType)
        {
            EventBusCaller.Publish(new GameScreenEvents.EnableScreen
            {
                ScreenType = currentScreen,
                RequestType = requestType
            });
        }
        
        public static void DisableScreen(ScreenType currentScreen,EventRequestType requestType)
        {
            EventBusCaller.Publish(new GameScreenEvents.DisableScreen
            {
                ScreenType = currentScreen,
                RequestType = requestType
            });
        }

        public static void LoadLastScreen()
        {
            EventBusCaller.Publish(new GameScreenEvents.LastScreen());
        }

    }
    
    public static class GameScreenEventSubscriber
    {

        public static void EnableScreen(Action<GameScreenEvents.EnableScreen> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void DisableScreen(Action<GameScreenEvents.DisableScreen> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void GoToLastScreen(Action<GameScreenEvents.LastScreen> action)
        {
            EventBusCaller.Subscribe(action);
        }
    }
    
    public static class GameScreenEventUnSubscriber
    {
        public static void EnableScreen(Action<GameScreenEvents.EnableScreen> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void DisableScreen(Action<GameScreenEvents.DisableScreen> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void GoToLastScreen(Action<GameScreenEvents.LastScreen> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
    }
    
    #endregion
    
    #region Projectile
    
    public static class ProjectileEventCaller
    {
        public static void Collision(CollisionData data)
        {
            EventBusCaller.Publish(new ProjectileEvents.Collision
            {
                Position = data.Position,
                Rotation = data.Rotation,
                Direction = data.Direction,
                Type = data.Type
            });
        }
        
        public static void Deflected(DeflectData data)
        {
            EventBusCaller.Publish(new ProjectileEvents.Deflected
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
            EventBusCaller.Publish(new ProjectileEvents.Add{Projectile = projectile});
        }

        public static void RequestSpawn(ProjectileType projectileType)
        {
            EventBusCaller.Publish(new ProjectileEvents.RequestSpawn
            {
                ProjectileType = projectileType, 
                RequestType = EventRequestType.Requested
            });
        }
        
        public static void GrantSpawn(ProjectileType projectileType)
        {
            EventBusCaller.Publish(new ProjectileEvents.RequestSpawn
            {
                ProjectileType = projectileType, 
                RequestType = EventRequestType.Granted
            });
        }

        public static void Spawn(ProjectileSpawnData data)
        {
            EventBusCaller.Publish(new ProjectileEvents.Spawn
            {
                ProjectileType = data.ProjectileType,
                Position = data.Position,
                Direction = data.Direction,
                MovementMultiplier = data.MovementMultiplier
            });
        }

        public static void ClearQueue()
        {
            EventBusCaller.Publish(new ProjectileEvents.ClearQueue());
        }
        
        public static void DisableSpawn()
        {
            EventBusCaller.Publish(new ProjectileEvents.DisableSpawn());
        }
        
        public static void EnableSpawn()
        {
            EventBusCaller.Publish(new ProjectileEvents.EnableSpawn());
        }
        
        public static void UpdateLevel(int level)
        {
            EventBusCaller.Publish(new ProjectileEvents.UpdateLevel{Level = level});
        }
    }
    
    public static class ProjectileEventSubscriber
    {
        public static void Collision(Action<ProjectileEvents.Collision> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void Deflected(Action<ProjectileEvents.Deflected> action)
        {
            EventBusCaller.Subscribe(action);
        }

        public static void Add(Action<ProjectileEvents.Add> action)
        {
            EventBusCaller.Subscribe(action);
        }

        public static void RequestSpawn(Action<ProjectileEvents.RequestSpawn> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void GrantSpawn(Action<ProjectileEvents.RequestSpawn> action)
        {
            EventBusCaller.Subscribe(action);
        }

        public static void Spawn(Action<ProjectileEvents.Spawn> action)
        {
            EventBusCaller.Subscribe(action);
        }

        public static void ClearQueue(Action<ProjectileEvents.ClearQueue> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void DisableSpawn(Action<ProjectileEvents.DisableSpawn> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void EnableSpawn(Action<ProjectileEvents.EnableSpawn> action)
        {
            EventBusCaller.Subscribe(action);
        }
        
        public static void UpdateLevel(Action<ProjectileEvents.UpdateLevel> action)
        {
            EventBusCaller.Subscribe(action);
        }
    }
    
    public static class ProjectileEventUnSubscriber
    {
        public static void Collision(Action<ProjectileEvents.Collision> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void Deflected(Action<ProjectileEvents.Deflected> action)
        {
            EventBusCaller.Unsubscribe(action);
        }

        public static void Add(Action<ProjectileEvents.Add> action)
        {
            EventBusCaller.Unsubscribe(action);
        }

        public static void RequestSpawn(Action<ProjectileEvents.RequestSpawn> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void GrantSpawn(Action<ProjectileEvents.RequestSpawn> action)
        {
            EventBusCaller.Unsubscribe(action);
        }

        public static void Spawn(Action<ProjectileEvents.Spawn> action)
        {
            EventBusCaller.Unsubscribe(action);
        }

        public static void ClearQueue(Action<ProjectileEvents.ClearQueue> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void DisableSpawn(Action<ProjectileEvents.DisableSpawn> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void EnableSpawn(Action<ProjectileEvents.EnableSpawn> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
        
        public static void UpdateLevel(Action<ProjectileEvents.UpdateLevel> action)
        {
            EventBusCaller.Unsubscribe(action);
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
            EventBusCaller.Publish(new MeteorEvents.SpawnRing());
        }

        public static void RingActive(bool isActive)
        {
            EventBusCaller.Publish(new MeteorEvents.RingActive{IsActive = isActive});
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
            EventBusCaller.Subscribe(action);
        }

        public static void RingActive(Action<MeteorEvents.RingActive> action)
        {
            EventBusCaller.Subscribe(action);
        }
    }
    
    public static class MeteorEventUnSubscriber
    {
        public static void GrantSpawnSingle(Action<ProjectileEvents.RequestSpawn> action)
        {
            ProjectileEventUnSubscriber.GrantSpawn(action);
        }
        
        public static void RequestSpawnSingle(Action<ProjectileEvents.RequestSpawn> action)
        {
            ProjectileEventUnSubscriber.RequestSpawn(action);
        }
        
        public static void SpawnRing(Action<MeteorEvents.SpawnRing> action)
        {
            EventBusCaller.Unsubscribe(action);
        }

        public static void RingActive(Action<MeteorEvents.RingActive> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
    }
    
    #endregion
    
    #region Particle

    public static class ParticleEventCaller
    {
        public static void Spawn(ParticleSpawnData data)
        {
            EventBusCaller.Publish(new ParticleEvents.Spawn
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
            EventBusCaller.Subscribe(action);
        }
    }
    
    public static class ParticleEventUnSubscribe
    {
        public static void Spawn(Action<ParticleEvents.Spawn> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
    }
    

    
    #endregion
    
    #region Camera
    
    public static class CameraEventCaller
    {
        public static void ZoomIn(float timeToZoom = 0.5f) 
            => EventBusCaller.Publish(new CameraEvents.ZoomIn{TimeToZoom = timeToZoom});
        public static void ZoomOut(float timeToZoom = 0.5f) 
            => EventBusCaller.Publish(new CameraEvents.ZoomOut{TimeToZoom = timeToZoom});
        public static void Shake(IShakeData shake) 
            => EventBusCaller.Publish(new CameraEvents.Shake{ShakeData = shake});
        public static void LookCenter(float timeToLook = 0.5f) 
            => EventBusCaller.Publish(new CameraEvents.LookCenter{TimeToLook = timeToLook});
        public static void LookRight(float timeToLook = 0.5f) 
            => EventBusCaller.Publish(new CameraEvents.LookRight{TimeToLook = timeToLook});
        public static void LookLeft(float timeToLook = 0.5f) 
            => EventBusCaller.Publish(new CameraEvents.LookLeft{TimeToLook = timeToLook});
        public static void LookUp(float timeToLook = 0.5f) 
            => EventBusCaller.Publish(new CameraEvents.LookUp{TimeToLook = timeToLook});
        public static void LookDown(float timeToLook = 0.5f) 
            => EventBusCaller.Publish(new CameraEvents.LookDown{TimeToLook = timeToLook});
        public static void NotifyZoomFinished() 
            => EventBusCaller.Publish(new CameraEvents.ZoomFinished());
        public static void NotifyLookFinished() 
            => EventBusCaller.Publish(new CameraEvents.LookFinished());
        public static void NotifyShakeFinished() 
            => EventBusCaller.Publish(new CameraEvents.ShakeFinished());
        public static void EnableGrayscale() 
            => EventBusCaller.Publish(new CameraEvents.GrayscaleEnable());
        public static void DisableGrayscale() 
            => EventBusCaller.Publish(new CameraEvents.GrayscaleDisable());
    }
    
    public static class CameraEventSubscriber
    {
        public static void ZoomIn(Action<CameraEvents.ZoomIn> action) 
            => EventBusCaller.Subscribe(action);
        public static void ZoomOut(Action<CameraEvents.ZoomOut> action) 
            => EventBusCaller.Subscribe(action);
        public static void Shake(Action<CameraEvents.Shake> action) 
            => EventBusCaller.Subscribe(action);
        public static void LookCenter(Action<CameraEvents.LookCenter> action) 
            => EventBusCaller.Subscribe(action);
        public static void LookRight(Action<CameraEvents.LookRight> action) 
            => EventBusCaller.Subscribe(action);
        public static void LookLeft(Action<CameraEvents.LookLeft> action) 
            => EventBusCaller.Subscribe(action);
        public static void LookUp(Action<CameraEvents.LookUp> action) 
            => EventBusCaller.Subscribe(action);
        public static void LookDown(Action<CameraEvents.LookDown> action) 
            => EventBusCaller.Subscribe(action);
        public static void NotifyZoomFinished(Action<CameraEvents.ZoomFinished> action) 
            => EventBusCaller.Subscribe(action);
        public static void NotifyLookFinished(Action<CameraEvents.LookFinished> action) 
            => EventBusCaller.Subscribe(action);
        public static void NotifyShakeFinished(Action<CameraEvents.ShakeFinished> action) 
            => EventBusCaller.Subscribe(action);
        public static void EnableGrayscale(Action<CameraEvents.GrayscaleEnable> action) 
            => EventBusCaller.Subscribe(action);
        public static void DisableGrayscale(Action<CameraEvents.GrayscaleDisable> action) 
            => EventBusCaller.Subscribe(action);
    }
    
    public static class CameraEventUnSubscriber
    {
        public static void ZoomIn(Action<CameraEvents.ZoomIn> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void ZoomOut(Action<CameraEvents.ZoomOut> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void Shake(Action<CameraEvents.Shake> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void LookCenter(Action<CameraEvents.LookCenter> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void LookRight(Action<CameraEvents.LookRight> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void LookLeft(Action<CameraEvents.LookLeft> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void LookUp(Action<CameraEvents.LookUp> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void LookDown(Action<CameraEvents.LookDown> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void NotifyZoomFinished(Action<CameraEvents.ZoomFinished> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void NotifyLookFinished(Action<CameraEvents.LookFinished> action)
            => EventBusCaller.Unsubscribe(action);
        public static void NotifyShakeFinished(Action<CameraEvents.ShakeFinished> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void EnableGrayscale(Action<CameraEvents.GrayscaleEnable> action)
            => EventBusCaller.Unsubscribe(action);
        public static void DisableGrayscale(Action<CameraEvents.GrayscaleDisable> action) 
            => EventBusCaller.Unsubscribe(action);
    }

    #endregion
    
    #region Inputs
    

    public static class InputsEventCaller
    {
        public static void SetEnable(bool enable)
        {
            EventBusCaller.Publish(new InputsEvents.SetEnable{IsEnable = enable});
        }

        public static void SetUIEnable(bool enable)
        {
            EventBusCaller.Publish(new InputsEvents.SetUIEnable{IsEnable = enable});
        }
    }
    
    
    public static class InputsEventSubscriber
    {
        public static void SetEnable(Action<InputsEvents.SetEnable> action)
        {
            EventBusCaller.Subscribe(action);
        }

        public static void SetUIEnable(Action<InputsEvents.SetUIEnable> action)
        {
            EventBusCaller.Subscribe(action);
        }
    }
    
    public static class InputsEventUnSubscriber
    {
        public static void SetEnable(Action<InputsEvents.SetEnable> action)
        {
            EventBusCaller.Unsubscribe(action);
        }

        public static void SetUIEnable(Action<InputsEvents.SetUIEnable> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
    }
    

    #endregion
    
    #region Abilities
    
    public static class AbilitiesEventCaller
    {
        public static void SetCanUse(bool canUse) => EventBusCaller.Publish(new AbilitiesEvents.SetCanUse{CanUse = canUse});
        public static void EnableUI() => EventBusCaller.Publish(new AbilitiesEvents.EnableUI());
        public static void DisableUI() => EventBusCaller.Publish(new AbilitiesEvents.DisableUI());

        public static void Add(AbilityAddData data)
        {
            EventBusCaller.Publish(new AbilitiesEvents.Add
            {
                AbilityType = data.AbilityType,
                Position = data.Position
            });
        }
        public static void SetStorageFull(bool isFull) => EventBusCaller.Publish(new AbilitiesEvents.SetStorageFull{IsFull = isFull});
        public static void NotifyIsActive(AbilityType type, bool isActive)
        {
            EventBusCaller.Publish(new AbilitiesEvents.NotifyIsActive
            {
                AbilityType = type,
                IsActive = isActive,
            });
        }

        public static void Enable() => EventBusCaller.Publish(new AbilitiesEvents.Enable());

        public static void Disable() => EventBusCaller.Publish(new AbilitiesEvents.Disable());

        public static void RunTimer() => EventBusCaller.Publish(new AbilitiesEvents.RunTimer());

        public static void GrantSpawn()
        {
            EventBusCaller.Publish(new ProjectileEvents.RequestSpawn
            {
                ProjectileType = ProjectileType.AbilitySphere, 
                RequestType = EventRequestType.Granted
            });
        }
        
        public static void RequestSpawn()
        {
            EventBusCaller.Publish(new ProjectileEvents.RequestSpawn
            {
                ProjectileType = ProjectileType.AbilitySphere, 
                RequestType = EventRequestType.Requested
            });
        }

        public static void SetNextSpawn(AbilityType type) => EventBusCaller.Publish(new AbilitiesEvents.SetNextSpawn{AbilityType = type});
        public static void UiInitialized() => EventBusCaller.Publish(new AbilitiesEvents.UIInitialized());
    }
    public static class AbilitiesEventSubscriber
    {
        public static void SetCanUse(Action<AbilitiesEvents.SetCanUse> action) => EventBusCaller.Subscribe(action);
        public static void EnableUI(Action<AbilitiesEvents.EnableUI> action) => EventBusCaller.Subscribe(action);
        public static void DisableUI(Action<AbilitiesEvents.DisableUI> action) => EventBusCaller.Subscribe(action);
        public static void Add(Action<AbilitiesEvents.Add> action) => EventBusCaller.Subscribe(action);
        public static void SetStorageFull(Action<AbilitiesEvents.SetStorageFull> action) => EventBusCaller.Subscribe(action);
        public static void NotifyIsActive(Action<AbilitiesEvents.NotifyIsActive> action) => EventBusCaller.Subscribe(action);
        public static void Enable(Action<AbilitiesEvents.Enable> action) => EventBusCaller.Subscribe(action);
        public static void Disable(Action<AbilitiesEvents.Disable> action) => EventBusCaller.Subscribe(action);
        public static void RunTimer(Action<AbilitiesEvents.RunTimer> action) => EventBusCaller.Subscribe(action);
        public static void GrantSpawn(Action<ProjectileEvents.RequestSpawn> action) => EventBusCaller.Subscribe(action);
        public static void RequestSpawn(Action<ProjectileEvents.RequestSpawn> action) => EventBusCaller.Subscribe(action);
        public static void SetNextSpawn(Action<AbilitiesEvents.SetNextSpawn> action) => EventBusCaller.Subscribe(action);
        public static void UiInitialized(Action<AbilitiesEvents.UIInitialized> action) => EventBusCaller.Subscribe(action);
    }
    public static class AbilitiesEventUnSubscriber
    {
        public static void SetCanUse(Action<AbilitiesEvents.SetCanUse> action) => EventBusCaller.Unsubscribe(action);
        public static void EnableUI(Action<AbilitiesEvents.EnableUI> action) => EventBusCaller.Unsubscribe(action);
        public static void DisableUI(Action<AbilitiesEvents.DisableUI> action) => EventBusCaller.Unsubscribe(action);
        public static void Add(Action<AbilitiesEvents.Add> action) => EventBusCaller.Unsubscribe(action);
        public static void SetStorageFull(Action<AbilitiesEvents.SetStorageFull> action) => EventBusCaller.Unsubscribe(action);
        public static void NotifyIsActive(Action<AbilitiesEvents.NotifyIsActive> action) => EventBusCaller.Unsubscribe(action);
        public static void Enable(Action<AbilitiesEvents.Enable> action) => EventBusCaller.Unsubscribe(action);
        public static void Disable(Action<AbilitiesEvents.Disable> action) => EventBusCaller.Unsubscribe(action);
        public static void RunTimer(Action<AbilitiesEvents.RunTimer> action) => EventBusCaller.Unsubscribe(action);
        public static void GrantSpawn(Action<ProjectileEvents.RequestSpawn> action) => EventBusCaller.Unsubscribe(action);
        public static void RequestSpawn(Action<ProjectileEvents.RequestSpawn> action) => EventBusCaller.Unsubscribe(action);
        public static void SetNextSpawn(Action<AbilitiesEvents.SetNextSpawn> action) => EventBusCaller.Unsubscribe(action);
        public static void UiInitialized(Action<AbilitiesEvents.UIInitialized> action) => EventBusCaller.Unsubscribe(action);
    }

    #endregion

    #region Abilities UI

    public static class AbilitiesUIEventCaller
    {
        public static void Add(int index) 
            => EventBusCaller.Publish(new AbilitiesUIEvents.Add{AbilityIndex = index});
        
        public static void Initialize()
            => EventBusCaller.Publish(new AbilitiesUIEvents.Initialize());

        public static void SelectAbility()
            => EventBusCaller.Publish(new AbilitiesUIEvents.SelectAbility());

        public static void Restart()
            => EventBusCaller.Publish(new AbilitiesUIEvents.Restart());

        public static void EnableUI()
            => EventBusCaller.Publish(new AbilitiesUIEvents.EnableUI());

        public static void DisableUI()
            => EventBusCaller.Publish(new AbilitiesUIEvents.DisableUI());
    }
    
    public static class AbilitiesUISubscriber
    {
        public static void Add(Action<AbilitiesUIEvents.Add> action)
            => EventBusCaller.Subscribe(action);

        public static void Initialize(Action<AbilitiesUIEvents.Initialize> action)
            => EventBusCaller.Subscribe(action);

        public static void SelectAbility(Action<AbilitiesUIEvents.SelectAbility> action)
            => EventBusCaller.Subscribe(action);

        public static void Restart(Action<AbilitiesUIEvents.Restart> action)
            => EventBusCaller.Subscribe(action);

        public static void EnableUI(Action<AbilitiesUIEvents.EnableUI> action)
            => EventBusCaller.Subscribe(action);

        public static void DisableUI(Action<AbilitiesUIEvents.DisableUI> action)
            => EventBusCaller.Subscribe(action);
    }

    public static class AbilitiesUIUnSubscriber
    {
        public static void Add(Action<AbilitiesUIEvents.Add> action)
            => EventBusCaller.Unsubscribe(action);

        public static void Initialize(Action<AbilitiesUIEvents.Initialize> action)
            => EventBusCaller.Unsubscribe(action);

        public static void SelectAbility(Action<AbilitiesUIEvents.SelectAbility> action)
            => EventBusCaller.Unsubscribe(action);

        public static void Restart(Action<AbilitiesUIEvents.Restart> action)
            => EventBusCaller.Unsubscribe(action);

        public static void EnableUI(Action<AbilitiesUIEvents.EnableUI> action)
            => EventBusCaller.Unsubscribe(action);

        public static void DisableUI(Action<AbilitiesUIEvents.DisableUI> action)
            => EventBusCaller.Unsubscribe(action);
    }

    #endregion
    
    #region Floatin Text
    

    public static class FloatingTextEventCaller
    {
        public static void Spawn(FloatingTextValues data)
        {
            EventBusCaller.Publish(new FloatingTextEvents.Spawn { Data = data });
        }
    }
    
    public static class FloatingTextEventSubscriber
    {
        public static void Spawn(Action<FloatingTextEvents.Spawn> action)
        {
            EventBusCaller.Subscribe(action);
        }
    }
    
    public static class FloatingTextEventUnSubscriber
    {
        public static void Spawn(Action<FloatingTextEvents.Spawn> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
    }
    
    #endregion
    
    #region MultiPage
    

    public static class MultiPageUIEventCaller
    {
        public static void Create(IMultiPageData data, ulong createId)
        {
            EventBusCaller.Publish(new MultiPageUIEvents.Create{Data = data, CreateId = createId});
        }

        public static void Finished(ulong createId)
        {
            EventBusCaller.Publish(new MultiPageUIEvents.Finished{CreateId = createId});
        }
    }
    
    public static class MultiPageUIEventSubscriber
    {
        public static void Create(Action<MultiPageUIEvents.Create> action)
        {
            EventBusCaller.Subscribe(action);
        }

        public static void Finished(Action<MultiPageUIEvents.Finished> action)
        {
            EventBusCaller.Subscribe(action);
        }
    }
    
    public static class MultiPageUIEventUnSubscriber
    {
        public static void Create(Action<MultiPageUIEvents.Create> action)
        {
            EventBusCaller.Unsubscribe(action);
        }

        public static void Finished(Action<MultiPageUIEvents.Finished> action)
        {
            EventBusCaller.Unsubscribe(action);
        }
    }
    
    #endregion
    
}