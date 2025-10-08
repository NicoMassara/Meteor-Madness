using System;
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

    public static class EventCallerCamera
    {
        public static void ZoomIn()
        {
            GameEventCaller.Publish(new CameraEvents.ZoomIn());
        }
        
        public static void ZoomOut()
        {
            GameEventCaller.Publish(new CameraEvents.ZoomIn());
        }
    }

    public static class EventCallerMeteor
    {
        public static void GrantSpawnSingle()
        {
            GameEventCaller.Publish(new ProjectileEvents.RequestSpawn
            {
                ProjectileType = ProjectileType.Meteor, RequestType = EventRequestType.Granted
            });
        }
        
        public static void RequestSpawnSingle()
        {
            GameEventCaller.Publish(new ProjectileEvents.RequestSpawn
            {
                ProjectileType = ProjectileType.Meteor, RequestType = EventRequestType.Request
            });
        }
        
        public static void SpawnRing()
        {
            GameEventCaller.Publish(new MeteorEvents.SpawnRing());
        }
    }
    public static class EventCallerAbility
    {
        public static void AddAbility(AbilityType abilityType)
        {
            GameEventCaller.Publish(new AbilitiesEvents.Add{AbilityType = abilityType});
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
    public static class EventCallerShield
    {
        public static void SetEnableShield(bool isEnabled)
        {
            GameEventCaller.Publish(new ShieldEvents.SetEnable{IsEnabled = isEnabled});
        }
    }
    public static class EventCallerGameValues
    {
        public static void SetDamageType(DamageTypes damageType)
        {
            GameConfigManager.Instance.SetDamage(damageType);
        }

        public static void SetCanPlay(bool canPlay)
        {
            GameManager.Instance.CanPlay = canPlay;
        }

        public static void UpdateLevel(int level)
        {
            GameEventCaller.Publish(new GameModeEvents.UpdateLevel{CurrentLevel = level});
        }
    }
    public static class EventCallerInputs
    {
        public static void SetEnable(bool enable)
        {
            GameEventCaller.Publish(new InputsEvents.SetEnable{IsEnable = enable});
        }
    }
    
    public static class EventCallerGameMode
    {
        public static void Start()
        {
            GameEventCaller.Publish(new GameModeEvents.Start());
        }

        public static void Finish()
        {
            GameEventCaller.Publish(new GameModeEvents.Finish());
        }
    }
}