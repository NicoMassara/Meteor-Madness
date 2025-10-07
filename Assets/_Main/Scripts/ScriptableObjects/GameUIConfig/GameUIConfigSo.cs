using _Main.Scripts.Interfaces;
using _Main.Scripts.ScriptableObjects.GameUIConfig;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_GameUIConfigData_Name", menuName = "Scriptable Objects/Game UI Config/Config Data", order = -1)]
    public class GameUIConfigSo : ScriptableObject, IGameUIConfig
    {
        [Range(0,1f)]
        [SerializeField] private float gameplayPointsTimeToIncrease = 0.35f;
        [Range(0,1f)]
        [SerializeField] private float closePauseMenu = 0.25f;
        
        [SerializeField] private DeathUITimeDataSo deathUITimeData;
        [SerializeField] private UITextValuesDataSo textData;

        public float GameplayPointsTimeToIncrease => gameplayPointsTimeToIncrease;
        public float ClosePauseMenu => closePauseMenu;
        public IDeathUITime DeathUITimeData => deathUITimeData;

        public IUITextData TextData => textData;
    }
}