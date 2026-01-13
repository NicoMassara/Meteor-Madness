using UnityEngine;

namespace _Main.Scripts.Environment.Comet.Spawner
{
    internal interface ICometData
    {
        public float MovementSpeed { get; }
        public float SpeedVariation { get; }

        public Vector2 SpawnOffset { get; }
        public Vector2 ScaleRange { get; }
    }

    internal interface IDistanceData
    {
        public float TravelRatioToTriggerSpawn { get; }
    }

    internal interface ISpawnData
    {
        public float SpawnDelay { get; }
        public float SpawnDelayVariation { get; }
    }

    [CreateAssetMenu(fileName = "So_CometSpawn_Default", menuName = "Comet/Spawn Data", order = 0)]
    public class CometSpawnDataSo : ScriptableObject, ICometData, IDistanceData, ISpawnData
    {
        [System.Serializable]
        private class RangeData
        {
            public float min;
            public float max;
            
            public Vector2 Range() => new Vector2(min, max);
        }

        [Header("Comet Data")]
        [Space]
        [Header("Movement")]
        [Min(1)]
        [SerializeField] private float movementSpeed;
        [Min(0)]
        [SerializeField] private float speedVariation;
        [Header("Offset")]
        [SerializeField] private float spawnXOffset;
        [SerializeField] private float spawnYOffset;
        [Header("Scale")]
        [SerializeField] private RangeData scaleRange;
        [Space(2)]
        [Header("Distance Data")]
        [Range(0,1f)]
        [SerializeField] private float travelRatioToTriggerSpawn;
        [Space(2)]
        [Header("Spawn Data")]
        [SerializeField] private float spawnDelay;
        [Min(0)]
        [SerializeField] private float spawnDelayVariation;

        public float MovementSpeed => movementSpeed;

        public float SpeedVariation => speedVariation;

        public Vector2 SpawnOffset => new Vector2(spawnXOffset, spawnYOffset);
        public Vector2 ScaleRange => scaleRange.Range();
        public float TravelRatioToTriggerSpawn => travelRatioToTriggerSpawn;

        public float SpawnDelay => spawnDelay;

        public float SpawnDelayVariation => spawnDelayVariation;
    }
}