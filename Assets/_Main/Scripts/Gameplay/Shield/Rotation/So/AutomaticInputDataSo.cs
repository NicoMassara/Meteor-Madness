using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.So
{
    [CreateAssetMenu(fileName = "So_ShieldRotation_Automatic_Default", menuName = "Shield Rotation/Automatic Data", order = 0)]
    public class AutomaticInputDataSo : ScriptableObject, IAutomaticInputData
    {
        [SerializeField] private float checkRate;
        [SerializeField] private RotationData rotationData;

        public float CheckRate => checkRate;
        public IRotationData RotationData => rotationData;
    }
}