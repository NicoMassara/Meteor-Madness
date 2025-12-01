using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Managers
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
        
        public struct Enable { }
        public struct Disable { }
    }

    public struct GameScreenEvents
    {
        public struct SetScreen
        {
            public ScreenType ScreenType;
            public bool IsEnable;
        }
        
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
        public struct InitializeValues { }
    
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
        
        public struct Add
        {
            public IProjectile Projectile;
        }

        public struct RequestSpawn
        {
            public ProjectileType ProjectileType;
            public EventRequestType RequestType;
        }

        public struct Spawn
        {
            public ProjectileType ProjectileType;
            public Vector2 Position;
            public Vector2 Direction;
            public float MovementMultiplier;
        }
        
        public struct UpdateLevel
        {
            public int Level;
        }
        
        public struct ClearQueue {}
        public struct DisableSpawn {}
        public struct EnableSpawn {}
        
    }
    
    public struct MeteorEvents
    {
        public struct SpawnRing {}
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
        public struct Shake
        {
            public IShakeData ShakeData;
        }

        public struct ZoomIn
        {
            public float TimeToZoom;
        }
        public struct ZoomOut
        {
            public float TimeToZoom;
        }
        
        public struct LookCenter
        {
            public float TimeToLook;
        }
        public struct LookRight
        {
            public float TimeToLook;
        }
        public struct LookLeft
        {
            public float TimeToLook;
        }
        
        public struct LookUp
        {
            public float TimeToLook;
        }
        
        public struct LookDown
        {
            public float TimeToLook;
        }

        public struct ZoomFinished { }
        public struct LookFinished { }
        public struct ShakeFinished { }
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

        public struct SetStorageFull
        {
            public bool IsFull;
        }
        
        public struct NotifyIsActive
        {
            public AbilityType AbilityType;
            public bool IsActive;
        }
        
        public struct SetNextSpawn
        {
            public AbilityType AbilityType;
        }
        public struct EnableUI { }
        public struct DisableUI { }
        
        public struct Enable { }
        public struct Disable { }
        public struct RunTimer { }
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
}