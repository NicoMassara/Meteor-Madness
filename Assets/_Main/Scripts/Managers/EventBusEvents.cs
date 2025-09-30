using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Particles;
using _Main.Scripts.Shaker;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    #region Earth
    public struct EarthRestart { }
    public struct EarthRestartFinish { }
    public struct EarthShake { }

    public struct HealEarth
    {
        public float HealAmount;
    }

    public struct EarthCanTakeDamage
    {
        public bool CanTakeDamage;
    }
    
    public struct EarthStartDestruction { }
    public struct EarthEndDestruction { }

    #endregion

    #region Shield

    public struct ShieldEnable
    {
        public bool IsEnabled;
    }

    public struct SetSuperShield { }
    public struct SetNormalShield { }


    #endregion

    #region GameScreen

    public struct MainMenuScreenEnable { }
    public struct GameModeScreenEnable { }

    public struct SetGameScreen
    {
        public int Index;
    }

    #endregion

    #region GameMode

    public struct GameModeEnable
    {
        public bool IsEnabled;
    }

    public struct GameStart { };
    public struct GameFinished { };
    public struct GameRestart { };

    public struct UpdateLevel
    {
        public int CurrentLevel;
    };
    
    public struct GamePause
    {
        public bool IsPaused;
    }
    
    #endregion

    #region Meteor

    public struct MeteorCollision
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector2 Direction;
    }

    public struct MeteorDeflected
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector2 Direction;
        public float Value;
    }
    
    public struct EnableMeteorSpawn
    {
        public bool CanSpawn;
    }
    
    public struct SpawnRingMeteor { }
    
    public struct RecycleAllMeteors{}

    #endregion

    #region Particle

    public struct SpawnParticle
    {
        public ParticleDataSo ParticleData; 
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 MoveDirection;
    }

    #endregion
    
    #region Camera

    public struct CameraShake
    {
        public ShakeDataSo ShakeData;
    }

    public struct CameraZoomIn { }
    public struct CameraZoomOut { }

    #endregion

    #region Inputs

    public struct SetEnableInputs
    {
        public bool IsEnable;
    }

    #endregion

    #region Abilities

    public struct SetEnableAbility
    {
        public bool IsEnable;
    }

    public struct AddAbility
    {
        public AbilityType AbilityType;
    }

    public struct EnableSpawner
    {
        public bool IsEnable;
    }

    public struct SetAbilityStorageFull
    {
        public bool IsFull;
    }

    #endregion
}