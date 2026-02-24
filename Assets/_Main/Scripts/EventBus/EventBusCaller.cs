using System;
using _Main.Scripts.Contracts.Events;
using _Main.Scripts.Contracts.Interfaces;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.EventBus
{
    public class EventBusCaller
    {
        public static void Publish<T>(T eventData) where T : struct
        {
            EventBusDebugEvents.TriggerOnEntryAdded(new EventBusDebugEntry
            {
                EventName = eventData.ToString(),
                ActionType = EventBusActionType.Publish
            });
            
            EventBusManager.Instance.Publish(eventData);
        }
        
        public static void Subscribe<T>(Action<T> listener) where T : struct
        {
            
            EventBusDebugEvents.TriggerOnEntryAdded(new EventBusDebugEntry
            {
                EventName = typeof(T).ToString(),
                ActionType = EventBusActionType.Subscribe
            });
            
            EventBusManager.Instance.Subscribe(listener);
        }

        public static void Unsubscribe<T>(Action<T> listener) where T : struct
        {
            EventBusDebugEvents.TriggerOnEntryAdded(new EventBusDebugEntry
            {
                EventName = typeof(T).ToString(),
                ActionType = EventBusActionType.Unsubscribe
            });
            
            EventBusManager.Instance.Unsubscribe(listener);
        }
    }

    #region Template

    public static class TemplateEventCaller
    {
        
    }

    public static class TemplatEventSubscriber
    {
        
    }
    
    public static class TemplateEventUnSubscriber
    {
        
    }

    #endregion
    
    #region GameMode
    
    public static class GameModeEventCaller
    {

        public static void SetPause(bool isPaused) 
            => EventBusCaller.Publish(new GameModeEvents.SetPause{IsPaused = isPaused});

        public static void SetEnablePause(bool isEnable) 
            => EventBusCaller.Publish(new GameModeEvents.SetEnablePause{CanPause = isEnable});

        public static void SetEnableUI(bool isEnable) 
            => EventBusCaller.Publish(new GameModeEvents.SetEnableUI{IsEnable = isEnable});
    }

    public static class GameModeEventSubscriber
    {
        public static void SetPause(Action<GameModeEvents.SetPause> action) 
            => EventBusCaller.Subscribe(action);

        public static void SetEnablePause(Action<GameModeEvents.SetEnablePause> action) 
            => EventBusCaller.Subscribe(action);

        public static void SetEnableUI(Action<GameModeEvents.SetEnableUI> action) 
            => EventBusCaller.Subscribe(action);
    }
    
    public static class GameModeEventUnSubscriber
    {
        public static void SetEnablePause(Action<GameModeEvents.SetEnablePause> action) => EventBusCaller.Unsubscribe(action);
        public static void SetEnableUI(Action<GameModeEvents.SetEnableUI> action) => EventBusCaller.Unsubscribe(action);
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
            => EventBusCaller.Publish(new ShieldEvents.RequestEnableShieldType{Type = type});
        public static void RequestDisableShieldType(ShieldType type) 
            => EventBusCaller.Publish(new ShieldEvents.RequestDisableShieldType{Type = type});
        public static void NotifyShieldTypeEnabled(ShieldType type) 
            => EventBusCaller.Publish(new ShieldEvents.NotifyShieldTypeEnabled{Type = type});
        public static void NotifyShieldTypeDisabled(ShieldType type) 
            => EventBusCaller.Publish(new ShieldEvents.NotifyShieldTypeDisabled{Type = type});
        public static void Enable() 
            => EventBusCaller.Publish(new ShieldEvents.Enable());
        public static void Disable() 
            => EventBusCaller.Publish(new ShieldEvents.Disable());
        public static void NotifyMovement(int value) 
            => EventBusCaller.Publish(new ShieldEvents.NotifyMovement{Direction = value});
    }
    
    public static class ShieldEventSubscriber
    {
        public static void RequestEnableShieldType(Action<ShieldEvents.RequestEnableShieldType> action) 
            => EventBusCaller.Subscribe(action);

        public static void RequestDisableShieldType(Action<ShieldEvents.RequestDisableShieldType> action) 
            => EventBusCaller.Subscribe(action);

        public static void NotifyShieldTypeEnabled(Action<ShieldEvents.NotifyShieldTypeEnabled> action) 
            => EventBusCaller.Subscribe(action);

        public static void NotifyShieldTypeDisabled(Action<ShieldEvents.NotifyShieldTypeDisabled> action) 
            => EventBusCaller.Subscribe(action);

        public static void Enable(Action<ShieldEvents.Enable> action) 
            => EventBusCaller.Subscribe(action);

        public static void Disable(Action<ShieldEvents.Disable> action) 
            => EventBusCaller.Subscribe(action);

        public static void NotifyMovement(Action<ShieldEvents.NotifyMovement> action) 
            => EventBusCaller.Subscribe(action);
    }
    
    public static class ShieldEventUnSubscriber
    {
        public static void RequestEnableShieldType(Action<ShieldEvents.RequestEnableShieldType> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void RequestDisableShieldType(Action<ShieldEvents.RequestDisableShieldType> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void NotifyShieldTypeEnabled(Action<ShieldEvents.NotifyShieldTypeEnabled> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void NotifyShieldTypeDisabled(Action<ShieldEvents.NotifyShieldTypeDisabled> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void Enable(Action<ShieldEvents.Enable> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void Disable(Action<ShieldEvents.Disable> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void NotifyMovement(Action<ShieldEvents.NotifyMovement> action) 
            => EventBusCaller.Unsubscribe(action);
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
    }
    
    public static class ProjectileEventSubscriber
    {
        public static void Collision(Action<ProjectileEvents.Collision> action) => EventBusCaller.Subscribe(action);
        public static void Deflected(Action<ProjectileEvents.Deflected> action) => EventBusCaller.Subscribe(action);
    }
    
    public static class ProjectileEventUnSubscriber
    {
        public static void Collision(Action<ProjectileEvents.Collision> action) => EventBusCaller.Unsubscribe(action);
        public static void Deflected(Action<ProjectileEvents.Deflected> action) => EventBusCaller.Unsubscribe(action);
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
        public static void Transport(ICameraTransportData data)
            => EventBusCaller.Publish(new CameraEvents.Transport{Data = data});
        public static void Shake(ShakeData shake) 
            => EventBusCaller.Publish(new CameraEvents.Shake{ShakeData = shake});
        public static void NotifyTransportStarted(CameraTransportType type) 
            => EventBusCaller.Publish(new CameraEvents.TransportStarted{Type = type});
        public static void NotifyTransportFinished(CameraTransportType type) 
            => EventBusCaller.Publish(new CameraEvents.TransportFinished{Type = type});
        public static void NotifyShakeFinished() 
            => EventBusCaller.Publish(new CameraEvents.ShakeFinished());
        public static void EnableGrayscale() 
            => EventBusCaller.Publish(new CameraEvents.GrayscaleEnable());
        public static void DisableGrayscale() 
            => EventBusCaller.Publish(new CameraEvents.GrayscaleDisable());
    }
    
    public static class CameraEventSubscriber
    {
        public static void Transport(Action<CameraEvents.Transport> action) 
            => EventBusCaller.Subscribe(action);
        public static void Shake(Action<CameraEvents.Shake> action) 
            => EventBusCaller.Subscribe(action);
        public static void NotifyTransportStarted(Action<CameraEvents.TransportStarted> action) 
            => EventBusCaller.Subscribe(action);
        public static void NotifyTransportFinished(Action<CameraEvents.TransportFinished> action) 
            => EventBusCaller.Subscribe(action);
        public static void EnableGrayscale(Action<CameraEvents.GrayscaleEnable> action) 
            => EventBusCaller.Subscribe(action);
        public static void DisableGrayscale(Action<CameraEvents.GrayscaleDisable> action) 
            => EventBusCaller.Subscribe(action);
    }
    
    public static class CameraEventUnSubscriber
    {
        public static void Transport(Action<CameraEvents.Transport> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void Shake(Action<CameraEvents.Shake> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void NotifyTransportStarted(Action<CameraEvents.TransportStarted> action) 
            => EventBusCaller.Unsubscribe(action);
        public static void NotifyTransportFinished(Action<CameraEvents.TransportFinished> action) 
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
        public static void SetEnable(bool enable) => EventBusCaller.Publish(new InputsEvents.SetEnable{IsEnable = enable});
        public static void SetUIEnable(bool enable) => EventBusCaller.Publish(new InputsEvents.SetUIEnable{IsEnable = enable});
        public static void ShakeUI(float healthRatio) => EventBusCaller.Publish(new InputsEvents.ShakeUI{HealthRatio = healthRatio});
    }
    
    
    public static class InputsEventSubscriber
    {
        public static void SetEnable(Action<InputsEvents.SetEnable> action) => EventBusCaller.Subscribe(action);
        public static void SetUIEnable(Action<InputsEvents.SetUIEnable> action) => EventBusCaller.Subscribe(action);
        public static void ShakeUI(Action<InputsEvents.ShakeUI> action) => EventBusCaller.Subscribe(action);
    }
    
    public static class InputsEventUnSubscriber
    {
        public static void SetEnable(Action<InputsEvents.SetEnable> action) => EventBusCaller.Unsubscribe(action);
        public static void SetUIEnable(Action<InputsEvents.SetUIEnable> action) => EventBusCaller.Unsubscribe(action);
        public static void ShakeUI(Action<InputsEvents.ShakeUI> action) => EventBusCaller.Unsubscribe(action);
    }
    

    #endregion
    
    #region Abilities
    
    public static class AbilitiesEventCaller
    {
        public static void SetCanUse(bool canUse) => EventBusCaller.Publish(new AbilitiesEvents.SetCanUse{CanUse = canUse});

        public static void Add(AbilityAddData data)
        {
            EventBusCaller.Publish(new AbilitiesEvents.Add
            {
                AbilityType = data.AbilityType,
                Position = data.Position
            });
        }
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
        public static void Trigger() => EventBusCaller.Publish(new AbilitiesEvents.Trigger());
    }
    public static class AbilitiesEventSubscriber
    {
        public static void SetCanUse(Action<AbilitiesEvents.SetCanUse> action) => EventBusCaller.Subscribe(action);
        public static void Add(Action<AbilitiesEvents.Add> action) => EventBusCaller.Subscribe(action);
        public static void NotifyIsActive(Action<AbilitiesEvents.NotifyIsActive> action) => EventBusCaller.Subscribe(action);
        public static void Enable(Action<AbilitiesEvents.Enable> action) => EventBusCaller.Subscribe(action);
        public static void Disable(Action<AbilitiesEvents.Disable> action) => EventBusCaller.Subscribe(action);
        public static void Trigger(Action<AbilitiesEvents.Trigger> action) => EventBusCaller.Subscribe(action);
    }
    public static class AbilitiesEventUnSubscriber
    {
        public static void SetCanUse(Action<AbilitiesEvents.SetCanUse> action) => EventBusCaller.Unsubscribe(action);
        public static void Add(Action<AbilitiesEvents.Add> action) => EventBusCaller.Unsubscribe(action);
        public static void NotifyIsActive(Action<AbilitiesEvents.NotifyIsActive> action) => EventBusCaller.Unsubscribe(action);
        public static void Enable(Action<AbilitiesEvents.Enable> action) => EventBusCaller.Unsubscribe(action);
        public static void Disable(Action<AbilitiesEvents.Disable> action) => EventBusCaller.Unsubscribe(action);
        public static void Trigger(Action<AbilitiesEvents.Trigger> action) => EventBusCaller.Unsubscribe(action);
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

    #region Comet Spawner

    public static class CometSpawnEventCaller
    {
        public static void Enable() => EventBusCaller.Publish(new CometSpawnEvents.Enable());
        public static void Disable() => EventBusCaller.Publish(new CometSpawnEvents.Disable());
        public static void Pause() => EventBusCaller.Publish(new CometSpawnEvents.Pause());
        public static void Resume() => EventBusCaller.Publish(new CometSpawnEvents.Resume());
    }

    public static class CometSpawnEventSubscriber
    {
        public static void Enable(Action<CometSpawnEvents.Enable> action) => EventBusCaller.Subscribe(action);
        public static void Disable(Action<CometSpawnEvents.Disable> action) => EventBusCaller.Subscribe(action);
        public static void Pause(Action<CometSpawnEvents.Pause> action) => EventBusCaller.Subscribe(action);
        public static void Resume(Action<CometSpawnEvents.Resume> action) => EventBusCaller.Subscribe(action);
        
    }
    
    public static class CometSpawnEventUnSubscriber
    {
        public static void Enable(Action<CometSpawnEvents.Enable> action) => EventBusCaller.Unsubscribe(action);
        public static void Disable(Action<CometSpawnEvents.Disable> action) => EventBusCaller.Unsubscribe(action);
        public static void Pause(Action<CometSpawnEvents.Pause> action) => EventBusCaller.Unsubscribe(action);
        public static void Resume(Action<CometSpawnEvents.Resume> action) => EventBusCaller.Unsubscribe(action);
    }

    #endregion

    #region ProjectileSpawner

    public abstract class ProjectileSpawner
    {
        public sealed class Publish
        {
            public static void Enable() => EventBusCaller.Publish(new ProjectileSpawnerEvents.Enable());
            public static void Disable() => EventBusCaller.Publish(new ProjectileSpawnerEvents.Disable());
            public static void Clear() => EventBusCaller.Publish(new ProjectileSpawnerEvents.Clear());
            public static void RestartValues() => EventBusCaller.Publish(new ProjectileSpawnerEvents.RestartValues());
            public static void SetLevel(int level) => EventBusCaller.Publish(new ProjectileSpawnerEvents.SetLevel{Level = level});
            public static void RequestSpawn(BatchType batchType) 
                => EventBusCaller.Publish(new ProjectileSpawnerEvents.RequestSpawn{BatchType = batchType});
            public static void BatchCreated(BatchType batchType) 
                => EventBusCaller.Publish(new ProjectileSpawnerEvents.BatchCreated{BatchType = batchType});
            public static void ProjectileSpawned(BatchType batchType) 
                => EventBusCaller.Publish(new ProjectileSpawnerEvents.ProjectileSpawned{BatchType = batchType});
            public static void BatchSpawned(BatchType batchType) 
                => EventBusCaller.Publish(new ProjectileSpawnerEvents.BatchSpawned{BatchType = batchType});
            public static void BatchFinished(BatchType batchType) 
                => EventBusCaller.Publish(new ProjectileSpawnerEvents.BatchFinished{BatchType = batchType});
            public static void BatchDeflected() => EventBusCaller.Publish(new ProjectileSpawnerEvents.BatchDeflected());
            public static void ProjectileReachedTarget(bool isLastFromBatch) 
                => EventBusCaller.Publish(new ProjectileSpawnerEvents.ProjectileReachedTarget{IsLastFromBatch = isLastFromBatch});
            public static void SetEnableAbilitySpawn(bool isEnable) 
                => EventBusCaller.Publish(new ProjectileSpawnerEvents.SetEnableAbilitySpawn{IsEnable = isEnable});
        }
        
        public sealed class Subscribe
        {
            public static void Enable(Action<ProjectileSpawnerEvents.Enable> action) => EventBusCaller.Subscribe(action);
            public static void Disable(Action<ProjectileSpawnerEvents.Disable> action) => EventBusCaller.Subscribe(action);
            public static void Clear(Action<ProjectileSpawnerEvents.Clear> action) => EventBusCaller.Subscribe(action);
            public static void RestartValues(Action<ProjectileSpawnerEvents.RestartValues> action) => EventBusCaller.Subscribe(action);
            public static void SetLevel(Action<ProjectileSpawnerEvents.SetLevel> action) => EventBusCaller.Subscribe(action);
            
            public static void RequestSpawn(Action<ProjectileSpawnerEvents.RequestSpawn> action) => EventBusCaller.Subscribe(action);
            public static void BatchCreated(Action<ProjectileSpawnerEvents.BatchCreated> action) => EventBusCaller.Subscribe(action);
            public static void ProjectileSpawned(Action<ProjectileSpawnerEvents.ProjectileSpawned> action) => EventBusCaller.Subscribe(action);
            public static void BatchSpawned(Action<ProjectileSpawnerEvents.BatchSpawned> action) => EventBusCaller.Subscribe(action);
            public static void BatchFinished(Action<ProjectileSpawnerEvents.BatchFinished> action) => EventBusCaller.Subscribe(action);
            public static void BatchDeflected(Action<ProjectileSpawnerEvents.BatchDeflected> action) => EventBusCaller.Subscribe(action);
            public static void ProjectileReachedTarget(Action<ProjectileSpawnerEvents.ProjectileReachedTarget> action) => EventBusCaller.Subscribe(action);
            public static void SetEnableAbilitySpawn(Action<ProjectileSpawnerEvents.SetEnableAbilitySpawn> action) => EventBusCaller.Subscribe(action);
        }
        
        public sealed class Unsubscribe
        {
            public static void Enable(Action<ProjectileSpawnerEvents.Enable> action) => EventBusCaller.Unsubscribe(action);
            public static void Disable(Action<ProjectileSpawnerEvents.Disable> action) => EventBusCaller.Unsubscribe(action);
            public static void Clear(Action<ProjectileSpawnerEvents.Clear> action) => EventBusCaller.Unsubscribe(action);
            public static void RestartValues(Action<ProjectileSpawnerEvents.RestartValues> action) => EventBusCaller.Unsubscribe(action);
            public static void SetLevel(Action<ProjectileSpawnerEvents.SetLevel> action) => EventBusCaller.Unsubscribe(action);
            
            public static void RequestSpawn(Action<ProjectileSpawnerEvents.RequestSpawn> action) => EventBusCaller.Unsubscribe(action);
            public static void BatchCreated(Action<ProjectileSpawnerEvents.BatchCreated> action) => EventBusCaller.Unsubscribe(action);
            public static void ProjectileSpawned(Action<ProjectileSpawnerEvents.ProjectileSpawned> action) => EventBusCaller.Unsubscribe(action);
            public static void BatchSpawned(Action<ProjectileSpawnerEvents.BatchSpawned> action) => EventBusCaller.Unsubscribe(action);
            public static void BatchFinished(Action<ProjectileSpawnerEvents.BatchFinished> action) => EventBusCaller.Unsubscribe(action);
            public static void BatchDeflected(Action<ProjectileSpawnerEvents.BatchDeflected> action) => EventBusCaller.Unsubscribe(action);
            public static void ProjectileReachedTarget(Action<ProjectileSpawnerEvents.ProjectileReachedTarget> action) => EventBusCaller.Unsubscribe(action);
            public static void EnableAbilitySpawn(Action<ProjectileSpawnerEvents.SetEnableAbilitySpawn> action) => EventBusCaller.Unsubscribe(action);
        }
    }

    #endregion
    
}