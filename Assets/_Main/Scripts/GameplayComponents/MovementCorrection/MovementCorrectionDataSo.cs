using UnityEngine;

namespace _Main.Scripts.GameplayComponents.MovementCorrection
{
    public interface IMovementCorrectionData
    {
        public int CorrectionSlotDistance { get; }
        public float MaxDistance { get; }
    }

    [CreateAssetMenu(fileName = "So_MovementCorrection_Default", menuName = "Movement/Correction Data", order = 0)]
    public class MovementCorrectionDataSo : ScriptableObject
    {
        
    }
}