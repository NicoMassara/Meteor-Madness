using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.So
{
    [CreateAssetMenu(fileName = "So_ShieldRotation_Default", menuName = "Shield Rotation/Rotation Data", order = 0)]
    public class RotatorDataSo : ScriptableObject, IShieldRotatorData
    {
        [SerializeField] private SpeederDataSo speederData;
        [SerializeField] private AutomaticInputDataSo automaticData;
        [SerializeField] private InputRotationDataSo inputRotationData;
        [SerializeField] private TargetFinderDataSo targetFinderData;
        [SerializeField] private TargetDetectorDataSo detectorData;

        public IAutomaticSpeederData SpeederData => speederData;
        public IAutomaticInputData AutomaticInputData => automaticData;
        public IInputRotationData InputRotationData => inputRotationData;
        public ITargetFinderData TargetFinderData => targetFinderData;
        public IDetectorData DetectorData => detectorData;
    }
}