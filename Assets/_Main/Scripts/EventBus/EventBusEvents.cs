using _Main.Scripts.Contracts.Interfaces;
using MeteorMadness.Contracts.Interfaces;
using UnityEngine;

namespace MeteorMadness.Contracts
{
    
    public struct EarthEvents
    {
        public struct Restart { }
        public struct RestartFinished { }
        public struct ShakeStart { }
        public struct Heal { }
        public struct EnableDamage { }
        public struct DisableDamage { }
        public struct DestructionStart {}
        public struct DestructionFinished {}
        public struct Death {}

        public struct PreSlice
        {
        }
    }

    public struct ShieldEvents
    {
        public struct RequestEnableShieldType
        {
            public ShieldType Type;
        }
        public struct RequestDisableShieldType
        {
            public ShieldType Type;
        }
        public struct NotifyShieldTypeEnabled
        {
            public ShieldType Type;
        }
        public struct NotifyShieldTypeDisabled
        {
            public ShieldType Type;
        }
        
        public struct NotifyMovement
        {
            public int Direction;
        }
        
        public struct Enable { }
        public struct Disable { }
    }

    public struct GameScreenEvents
    {
        public struct EnableScreen
        {
            public ScreenType ScreenType;
            public EventRequestType RequestType;
        }
        
        public struct DisableScreen
        {
            public ScreenType ScreenType;
            public EventRequestType RequestType;
        }

        public struct LastScreen
        {
        }
    }

    public struct GameModeEvents
    {
        
        public struct SetEnableUI
        {
            public bool IsEnable;
        }

        public struct SetPause
        {
            public bool IsPaused;
        }
        
        public struct SetEnablePause
        {
            public bool CanPause;
        }
    }

    public struct ProjectileEvents
    {
        public struct Collision
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector2 Direction;
            public ProjectileType Type;
        }

        public struct Deflected
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector2 Direction;
            public float Value;
            public ProjectileType Type;
        }
    }
    
    public struct MeteorEvents
    {
        public struct RingActive
        {
            public bool IsActive;
        }
    }

    public struct ParticleEvents
    {
        public struct Spawn
        {
            public IParticleData ParticleData; 
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 MoveDirection;
        }
    }

    public struct CameraEvents
    {
        public struct Transport
        {
            public ICameraTransportData Data;
        }

        public struct Shake
        {
            public ShakeData ShakeData;
        }

        public struct TransportStarted
        {
            public CameraTransportType Type;
        }

        public struct TransportFinished
        {
            public CameraTransportType Type;
        }
        
        public struct ShakeFinished { }
        public struct GrayscaleEnable { }
        public struct GrayscaleDisable { }
    }

    public struct InputsEvents
    {
        public struct SetEnable
        {
            public bool IsEnable;
        }
        
        public struct SetUIEnable
        {
            public bool IsEnable;
        }
        
        public struct ShakeUI
        {
            public float HealthRatio;
        }
    }

    public struct AbilitiesEvents
    {
        public struct SetCanUse
        {
            public bool CanUse;
        }

        public struct Add
        {
            public AbilityType AbilityType;
            public Vector2 Position;
        }
        
        public struct NotifyIsActive
        {
            public AbilityType AbilityType;
            public bool IsActive;
        }
        
        public struct Enable { }
        public struct Disable { }
        public struct Trigger { }
    }

    public struct FloatingTextEvents
    {
        public struct Spawn
        {
            public FloatingTextValues Data;
        }
    }

    public struct MultiPageUIEvents
    {
        public struct Create
        {
            public IMultiPageData Data;
            public ulong CreateId;
        }
        
        public struct Finished
        {
            public ulong CreateId;
        }
    }

    public struct CometSpawnEvents
    {
        public struct Enable { }
        public struct Disable { }
        public struct Pause { }
        public struct Resume { }
    }

    public struct ProjectileSpawnerEvents
    {
        public struct Enable {}
        public struct Disable {}
        public struct Clear {}
        public struct RestartValues {}

        public struct SetLevel
        {
            public int Level;
        }

        // Request a spawn of the given batch type
        public struct RequestSpawn 
        {
            public BatchType BatchType;
        }
        
        // When a batch is created
        public struct BatchCreated 
        {
            public BatchType BatchType;
        }
        
        // When a projectile is spawned
        public struct ProjectileSpawned 
        {
            public BatchType BatchType;
        }
        
        // When a batch is fully spawned
        public struct BatchSpawned 
        {
            public BatchType BatchType;
        }
        
        // When the projectiles from a batch are destroyed
        public struct BatchFinished 
        {
            public BatchType BatchType;
        }
        
        // When a batch is considered deflected
        public struct BatchDeflected { }

        public struct ProjectileReachedTarget
        {
            public bool IsLastFromBatch;
        }

        public struct SetEnableAbilitySpawn
        {
            public bool IsEnable;
        }
    }
}