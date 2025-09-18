using UnityEngine;

namespace _Main.Scripts.Gameplay.Meteor
{
    [CreateAssetMenu(fileName = "SO_MeteorSpawnData_Name", menuName = "Scriptable Objects/Meteor/Spawn Data", order = 0)]
    public class MeteorSpawnDataSo : ScriptableObject
    {
        [SerializeField] private MeteorSpawnData[] spawnData;

        public MeteorSpawnData[] SpawnData => spawnData;
        
        private void OnValidate()
        {
            for (int i = 1; i < spawnData.Length; i++)
            {
                if (spawnData[i].TravelDistance < spawnData[i - 1].TravelDistance)
                {
                    spawnData[i].TravelDistance = spawnData[i - 1].TravelDistance; // clamp upward
                }
                
                if (spawnData[i].SpeedMultiplier < spawnData[i - 1].SpeedMultiplier)
                {
                    spawnData[i].SpeedMultiplier = spawnData[i - 1].SpeedMultiplier; // clamp upward
                }
            }
        }
    }
    
    [System.Serializable]
    public class MeteorSpawnData
    {
        [Range(0,1f)]
        public float SpeedMultiplier;
        // The distance needs a Meteor to travel in order to spawn a new one, goes from 1 to 0.
        [Range(0,1f)]
        public float TravelDistance; 
    }
}