using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects.GameUIConfig
{
    [CreateAssetMenu(fileName = "SO_DeathUITimeData_Name", menuName = "Scriptable Objects/Game UI Config/Death Panel Data", order = -1)]
    public class DeathUITimeDataSo : ScriptableObject, IDeathUITime
    {
        [Range(0f, 2f)]
        [SerializeField] private float showDeathUI = 1.5f;
        [Range(0f, 2f)]
        [SerializeField] private float setEnableDeathText = 1.5f;
        [Range(0f, 2f)]
        [SerializeField] private float setEnableDeathScore = 1f;
        [Range(0f, 2f)]
        [SerializeField] private float deathPointsTimeToIncrease = 1f;
        [Range(0f, 2f)]
        [SerializeField] private float countDeathScore =  0.75f;
        [Range(0f, 2f)]
        [SerializeField] private float enableRestartButton = 1f;

        public float ShowDeathUI => showDeathUI;
        public float SetEnableDeathText => setEnableDeathText;
        public float SetEnableDeathScore => setEnableDeathScore;
        public float DeathPointsTimeToIncrease => deathPointsTimeToIncrease;
        public float CountDeathScore => countDeathScore;
        public float EnableRestartButton => enableRestartButton;
    }
}