using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay
{
    public class MeteorSpeedController
    {
        private int _currentLevelIndex;
        private int _meteorCount;

        private float[] levelSpeedMultiplier = new float[]
        {
            0.75f,
            0.80f,
            0.85f,
            0.90f,
            1.00f,
            1.10f,
            1.20f,
            1.30f,
            1.40f,
            1.50f,
            1.60f,
            1.70f,
            1.80f,
            1.85f,
            1.90f,
            1.95f,
            2.00f,
            2.05f,
            2.10f,
            2.15f,
            2.20f,
            2.25f,
            2.30f,
            2.35f,
            2.40f,
            2.45f,
            2.50f
        };

        private readonly int[] _meteorsNeeded = new int[]
        {
            3,
            2,
            3,
            2,
            5,
            7,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            13,
            14,
            15,
            16,
            17,
            18,
            19,
            20,
            20,
            20,
            20,
            20,
            25
        };
        
        
        public MeteorSpeedController()
        {

        }

        public void RestartAll()
        {
            _currentLevelIndex = 0;
            _meteorCount = 0;
        }

        public void RestartCount()
        {
            _meteorCount = 0;
        }

        public float GetCurrentMultiplier()
        {
            return levelSpeedMultiplier[_currentLevelIndex];
        }
        
        public void CheckForNextLevel(int meteorCount)
        {
            _meteorCount++;
            
            if (_meteorsNeeded[_currentLevelIndex] == _meteorCount)
            {
                _meteorCount = 0;
                _currentLevelIndex++;
            }
        }

    }
}