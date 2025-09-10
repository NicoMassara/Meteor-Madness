using _Main.Scripts.Particles;
using _Main.Scripts.Shaker;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    #region Earth

    public struct EarthDamaged
    {
        public int CurrentHealth;
    }

    public struct EarthHealed
    {
        public int CurrentHealth;
    }

    public struct EarthDeath { }
    public struct EarthDestruction { }
    public struct EarthRestart { }
    public struct EarthShake { }

    #endregion

    #region Shield

    public struct ShieldEnable
    {
        public bool IsEnabled;
    }

    #endregion

    #region GameMode

    public struct GameFinished { }

    #endregion

    #region Meteor

    public struct MeteorCollision
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }

    public struct MeteorDeflected
    {
        public Vector3 Position;
    }

    #endregion

    #region Spawner

    public struct SpawnParticle
    {
        public ParticleDataSo ParticleData; 
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 MoveDirection;
    }

    public struct SpawnSingleMeteor
    {
        public float Speed;
    }
    
    public struct SpawnRingMeteor
    {
        public float Speed;
    }
    
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