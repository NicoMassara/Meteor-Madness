using _Main.Scripts.Particles;
using _Main.Scripts.Shaker;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    #region Earth
    public struct EarthRestart { }
    public struct EarthShake { }
    
    public struct EarthStartDestruction { }
    public struct EarthEndDestruction { }

    #endregion

    #region Shield

    public struct ShieldEnable
    {
        public bool IsEnabled;
    }

    #endregion

    #region GameMode

    public struct GameFinished { };
    public struct GameRestart { };

    public struct UpdateLevel
    {
        public int CurrentLevel;
    };

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
    
    #region Meteor



    public struct EnableMeteorSpawn
    {
        public bool CanSpawn;
    }
    
    public struct SpawnRingMeteor { }
    
    public struct RecycleAllMeteors{}
    

    #endregion

    #region Camera

    public struct CameraShake
    {
        public ShakeDataSo ShakeData;
    }

    public struct CameraZoomIn { }
    public struct CameraZoomOut { }

    #endregion
}