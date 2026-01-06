using UnityEngine;

namespace _Main.Scripts.ShieldRotation.MovementCorrection
{
    public interface IMovementCorrectionData
    {
        public int CorrectionSlotDistance { get; }
        public float MaxDistance { get; }
    }

    [CreateAssetMenu(fileName = "So_ShieldRotation_Correction_Default", menuName = "Shield Rotation/Correction Data", order = 0)]
    public class MovementCorrectionDataSo : ScriptableObject, IMovementCorrectionData
    {
        [Min(0)]
        [SerializeField] private int correctionSlotDistance;
        [Min(0)]
        [SerializeField] private float maxDistance;

        public int CorrectionSlotDistance => correctionSlotDistance;

        public float MaxDistance => maxDistance;
    }
}