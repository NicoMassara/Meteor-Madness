using UnityEngine;

namespace _Main.Scripts.ShieldRotation.TargetSnapper
{
    public interface ITargetSnapperData
    {
        public float StartVelocity { get; }
    }
    
    [CreateAssetMenu(fileName = "So_ShieldRotation_Snapper_Default", menuName = "Shield Rotation/Snapper Data", order = 0)]
    public class TargetSnapperDataSo : ScriptableObject, ITargetSnapperData
    {
        [Min(1)] 
        [SerializeField] private float startVelocity = 200;
        public float StartVelocity => startVelocity;
    }
}