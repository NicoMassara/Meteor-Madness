using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.So
{
    [CreateAssetMenu(fileName = "So_ShieldRotation_Input_Default", menuName = "Shield Rotation/Input Data", order = 0)]
    public class InputRotationDataSo : ScriptableObject, IInputRotationData
    {
        [SerializeField] private float minInputAngle;
        [SerializeField] private RotationData rotationData;
        [SerializeField] private AnimationCurve magnitudeCurve;

        public float MinInputAngle => minInputAngle;
        public IRotationData RotationData => rotationData;
        public AnimationCurve MagnitudeCurve => magnitudeCurve;
    }
}