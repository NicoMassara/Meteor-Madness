using System.Collections.Generic;
using _Main.Scripts.Projectile;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.SO.WeightsData
{
    [CreateAssetMenu(fileName = "So_WeightsData_Default", menuName = "Scriptable Objects/Projectile Spawn/Weights Data", order = 1)]
    public class SpawnWeightsDataSo : ScriptableObject, ISpawnWeightsData
    {
        [SerializeField] private SpawnBaseWeights weights;
        
        public ISpawnBaseWeights GetWeights() => weights;
        
        private void OnValidate()
        {
            SetArray();
        }
        
        private void OnEnable()
        {
            SetArray();
        }

        private void SetArray()
        {
            weights?.TryCreateDefaultArray();
        }
    }
}