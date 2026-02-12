using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.So
{
    [CreateAssetMenu(fileName = "So_ShieldRotation_TargetDetector_Default", menuName = "Shield Rotation/TargetDetector Data", order = 0)]
    public class TargetDetectorDataSo : ScriptableObject, IDetectorData
    {
        [SerializeField] private LayerMask targetLayerMask;
        [SerializeField] private float checkRadius;
        public LayerMask TargetLayerMask => targetLayerMask;
        public float CheckRadius => checkRadius;
    }
}