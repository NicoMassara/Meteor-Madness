using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Particles;
using _Main.Scripts.Shaker;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    
    public struct EarthEvents
    {
        public struct Initialize { }
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
        public struct Initialize { };
        public struct SetEnable
        {
            public bool IsEnabled;
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
        public struct SetEnable
        {
            public bool IsEnabled;
        }

        public struct Initialize { };
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
        }

        public struct Deflected
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector2 Direction;
            public float Value;
        }
    
        public struct EnableSpawn
        {
            public bool CanSpawn;
        }
    
        public struct SpawnRing { }
    
        public struct RecycleAll{}
    }

    public struct ParticleEvents
    {
        public struct Spawn
        {
            public ParticleDataSo ParticleData; 
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 MoveDirection;
        }
    }

    public struct CameraEvents
    {
        public struct Shake
        {
            public ShakeDataSo ShakeData;
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
        }

        public struct EnableSpawner
        {
            public bool IsEnable;
        }

        public struct SetStorageFull
        {
            public bool IsFull;
        }
    }
}