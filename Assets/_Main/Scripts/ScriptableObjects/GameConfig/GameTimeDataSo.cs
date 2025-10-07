using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects.GameConfig
{
    [CreateAssetMenu(fileName = "SO_TimeData_Name", menuName = "Scriptable Objects/Game Config/Time Data", order = -1)]
    public class GameTimeDataSo : ScriptableObject, IGameTimeData
    {
        [Range(0, 10)] 
        [SerializeField] private int startGameCount = 3;
        [Range(1,5)]
        [SerializeField] private int firstCometSpawnDelay = 1;
        [Range(1,20)]
        [SerializeField] private int cometSpawnDelay = 8; 
        [Range(0,1)]
        [SerializeField] private float timeToLoadGameScene = 1f;
        [Range(0,1)]
        [SerializeField] private float triggerRestart = 0f;
        [Range(0,1)]
        [SerializeField] private float restartEarth = 0.25f;

        public int StartGameCount => startGameCount;
        public int CometSpawnDelay => cometSpawnDelay;
        public int FirstCometSpawnDelay => firstCometSpawnDelay;
        public float TimeToLoadGameScene => timeToLoadGameScene;
        public float TriggerRestart => triggerRestart;
        public float RestartEarth => restartEarth;
    }
}