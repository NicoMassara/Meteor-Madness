using _Main.Scripts.Gameplay.Meteor;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile
{
    public class ProjectileSpawnValues
    {
        private int _currentIndex;
        private readonly ProjectileSpawnDataSo _soData;

        public ProjectileSpawnValues(ProjectileSpawnDataSo soData)
        {
            _soData = soData;
        }

        public void SetIndex(int index)
        {
            _currentIndex = index;
            _currentIndex = Mathf.Clamp(_currentIndex, 0, _soData.SpawnData.Length - 1);
        }

        public float GetMovementMultiplier()
        {
            return _soData.SpawnData[_currentIndex].SpeedMultiplier;
        }

        public float GetMaxTravelDistance()
        {
            return _soData.SpawnData[_currentIndex].TravelDistance;
        }
    }
}