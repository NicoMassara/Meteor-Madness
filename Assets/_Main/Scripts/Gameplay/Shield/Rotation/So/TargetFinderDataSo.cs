using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.So
{
    [CreateAssetMenu(fileName = "So_ShieldRotation_TargetFinder_Default", menuName = "Shield Rotation/TargetFinder Data", order = 0)]
    public class TargetFinderDataSo : ScriptableObject, ITargetFinderData
    {
        [SerializeField] private RotationData rotationData;
        public IRotationData RotationData => rotationData;
    }
}