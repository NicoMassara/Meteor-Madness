using System;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile
{
    public class ProjectileTravelController 
    {
        private int _levelIndex;
        private int _levelAmount;
        private readonly Func<float, float> _getSpeedMultiplier;
        private readonly Func<float, float> _getTravelRatio;

        public ProjectileTravelController(
            Func<float, float> getSpeedMultiplier, 
            Func<float, float> getTravelRatio)
        {
            _getSpeedMultiplier = getSpeedMultiplier;
            _getTravelRatio = getTravelRatio;
        }

        public float GetMaxTravelDistance()
        {
            return _getTravelRatio.Invoke(GetLevelRatio());
        }

        public float GetMovementMultiplier()
        {
            return _getSpeedMultiplier.Invoke(GetLevelRatio());
        }

        private float GetLevelRatio()
        {
            return _levelIndex / (float)(_levelAmount - 1);
        }

        public void SetLevelIndex(int levelIndex)
        {
            _levelIndex = levelIndex;
        }

        public void SetLevelAmount(int levelAmount)
        {
            _levelAmount = levelAmount;
        }
    }
}