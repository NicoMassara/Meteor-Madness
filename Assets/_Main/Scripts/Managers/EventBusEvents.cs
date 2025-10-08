using _Main.Scripts.Interfaces;
using _Main.Scripts.ScriptableObjects;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    
    public struct EarthEvents
    {
        public struct Restart { }
        public struct RestartFinished { }
        public struct ShakeStart { }
        public struct Heal { }
        public struct SetEnableDamage
        {
            public bool DamageEnable;
        }
        
        public struct DestructionStart {}
        public struct DestructionFinished {}
    }

    public struct ShieldEvents
    {
        public struct SetEnable
        {
            public bool IsEnabled;
        }
        
        public struct SetGold
        {
            public bool IsActive;
        }
        
        public struct SetAutomatic
        {
            public bool IsActive;
        }
        public struct EnableSuperShield { }
        public struct EnableNormalShield { }
    }

    public struct GameScreenEvents
    {
        public struct MainMenuEnable { }
        public struct GameModeEnable { }
        
        public struct SetGameScreen
        {
            public int Index;
        }
    }

    public struct GameModeEvents
    {
        public struct Initialize { };

        public struct Disable { };

        public struct Start { };
        public struct Finish { };
        public struct Restart { };

        public struct UpdateLevel
        {
            public int CurrentLevel;
        };
    
        public struct SetPause
        {
            public bool IsPaused;
        }
    }

    public struct MeteorEvents
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
    
        public struct EnableSpawn
        {
            public bool CanSpawn;
        }

        public struct SpawnRing {}
        public struct RingActive
        {
            public bool IsActive;
        }
    
        public struct RecycleAll{}
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

        public struct ZoomIn { }
        public struct ZoomOut { }
    }

    public struct InputsEvents
    {
        public struct SetEnable
        {
            public bool IsEnable;
        }
    }

    public struct AbilitiesEvents
    {
        public struct SetEnable
        {
            public bool IsEnable;
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
        
        public struct SetActive
        {
            public AbilityType AbilityType;
            public bool IsActive;
        }
    }

    public struct ProjectileEvents
    {
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
        };
    }

    public struct FloatingTextEvents
    {
        public struct Spawn
        {
            public FloatingTextValues Data;
        }
    }
}