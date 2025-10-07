using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects.GameConfig
{
    [CreateAssetMenu(fileName = "SO_EarthTimeData_Name", menuName = "Scriptable Objects/Game Config/Earth/Time Data", order = 0)]
    public class EarthTimeDataSo : ScriptableObject, IEarthTime, IEarthDestruction, IEarthSlice, IEarthRestart
    {
        [Header("Destruction")]
        [Range(0,2f)]
        [SerializeField] private float startTriggerDestructionTime = 0f;
        [Range(0,2f)]
        [SerializeField] private float endTriggerDestructionTime = 0.5f;
        [Range(0,2f)]
        [SerializeField] private float startShake = 1.5f;
        [Range(0,2f)]
        [SerializeField] private float deathShakeDuration = 2f;
        [Range(0,2f)]
        [SerializeField] private float showEarthDestruction = 1f;
        [Range(0,2f)]
        [SerializeField] private float startRotatingAfterDeath = 1f;
        [Space]
        [Header("Slice")]
        [Range(0,1f)]
        [SerializeField] private float startSlice = 0.05f;
        [Range(0,1f)]
        [SerializeField] private float moveSlices = 0.05f;
        [Range(0,2f)]
        [SerializeField] private float returnToNormalTime = 0.5f;
        [Range(0,1f)]
        [SerializeField] private float returnSlices = 0.1f;
        [Space]
        [Header("Restart")]
        [Range(0,1f)]
        [SerializeField] private float restartZRotation = 0.25f;
        [Range(0,1f)]
        [SerializeField] private float restartYRotation = 0.25f;
        [Range(0,1f)]
        [SerializeField] private float timeBeforeRotateZ = 0.25f;
        [Range(0,1f)]
        [SerializeField] private float timeBeforeRotateY = 0.25f;
        [Range(0,1f)]
        [SerializeField] private float restartHealth = 0.75f;
        [Range(0,1f)]
        [SerializeField] private float finishRestart = 0.25f;
        
        
        public IEarthDestruction Destruction => this;
        public IEarthSlice Slice => this;
        public IEarthRestart Restart => this;
        
        public float StartTriggerDestructionTime => startTriggerDestructionTime;

        public float EndTriggerDestructionTime => endTriggerDestructionTime;
        

        public float StartShake => startShake;

        public float DeathShakeDuration => deathShakeDuration;

        public float ShowEarthDestruction => showEarthDestruction;

        public float StartRotatingAfterDeath => startRotatingAfterDeath;

        public float StartSlice => startSlice;

        public float MoveSlices => moveSlices;

        public float ReturnToNormalTime => returnToNormalTime;

        public float ReturnSlices => returnSlices;

        public float RestartZRotation => restartZRotation;

        public float RestartYRotation => restartYRotation;

        public float TimeBeforeRotateZ => timeBeforeRotateZ;

        public float TimeBeforeRotateY => timeBeforeRotateY;

        public float RestartHealth => restartHealth;

        public float FinishRestart => finishRestart;
    }
}