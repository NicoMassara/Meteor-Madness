using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects.AbilityTime
{
    [CreateAssetMenu(fileName = "SO_AbilityTimeData_[ABILITYNAME]_Default", menuName = "Scriptable Objects/Game Config/Ability/Time Data", order = 0)]
    public class AbilityTimeDataSo : ScriptableObject, IAbilityTimeData
    {
        [Range(1,15)]
        [SerializeField] private float activeTime;
        [Space]
        [Header("Zooming")]
        [Range(0,1f)]
        [SerializeField] private float zoomOut = 0.75f;
        [Range(0,1f)]
        [SerializeField] private float zoomIn = 0.75f;
        [Space]
        [Header("Action Time")]
        [Range(0,1f)]
        [SerializeField] private float startAction = 0.25f;
        [Range(0,1f)]
        [SerializeField] private float stopAction = 0.25f;
        [Header("Time Scale values")]
        [Range(0,1f)]
        [SerializeField] private float slowDown = 0.25f;
        [Range(0,1f)]
        [SerializeField] private float speedUp = 0.25f;
        
        public float ActiveTime => activeTime;
        //
        public float ZoomOut => zoomOut;
        public float ZoomIn => zoomIn;
        //
        public float StartAction => startAction;
        public float StopAction => stopAction;
        //
        public float SlowDown => slowDown;
        public float SpeedUp => speedUp;
    }
}