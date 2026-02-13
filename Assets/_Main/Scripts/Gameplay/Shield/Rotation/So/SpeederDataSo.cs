using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.So
{
    [CreateAssetMenu(fileName = "So_ShieldRotation_Speeder_Default", menuName = "Shield Rotation/Speeder Data", order = 0)]
    public class SpeederDataSo : ScriptableObject, IAutomaticSpeederData
    {
        [SerializeField] private float maxSpeed;
        [SerializeField] private float degreesStep;
        [SerializeField] private int accelerateTurnsAmount;
        [SerializeField] private int deAccelerateTurnsAmount;

        public float MaxSpeed => maxSpeed;
        public float DegreesStep => degreesStep;
        public int AccelerateTurnsAmount => accelerateTurnsAmount;
        public int DeAccelerateTurnsAmount => deAccelerateTurnsAmount;
    }
}