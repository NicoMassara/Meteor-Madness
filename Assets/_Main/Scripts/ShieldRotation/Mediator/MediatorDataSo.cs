using _Main.Scripts.ShieldRotation.AutomaticMovement;
using _Main.Scripts.ShieldRotation.Movement;
using _Main.Scripts.ShieldRotation.MovementCorrection;
using _Main.Scripts.ShieldRotation.RotationSpeeder;
using _Main.Scripts.ShieldRotation.TargetSnapper;
using UnityEngine;

namespace _Main.Scripts.ShieldRotation.Mediator
{
    public interface IMediatorData
    {
        public IMovementData MovementData { get; }
        public IAutomaticMovementData AutomaticData { get; }
        public IMovementCorrectionData CorrectionData { get; }
        public IRotationSpeederData SpeederData { get; }
        public ITargetSnapperData SnapperData { get; }
    }

    [CreateAssetMenu(fileName = "So_ShieldRotation_Mediator_Default", menuName = "Shield Rotation/Mediator Data", order = -1)]
    public class MediatorDataSo : ScriptableObject, IMediatorData
    {
        [SerializeField] private MovementDataSo movementData;
        [SerializeField] private AutomaticMovementDataSo automaticData;
        [SerializeField] private MovementCorrectionDataSo movementCorrectionData;
        [SerializeField] private RotationSpeederDataSo rotationSpeeder;
        [SerializeField] private TargetSnapperDataSo snapperData;

        public IMovementData MovementData => movementData;
        public IAutomaticMovementData AutomaticData => automaticData;
        public IMovementCorrectionData CorrectionData => movementCorrectionData;
        public IRotationSpeederData SpeederData => rotationSpeeder;
        public ITargetSnapperData SnapperData => snapperData;
    }
}