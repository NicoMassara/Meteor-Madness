using UnityEngine;

namespace _Main.Scripts.Gameplay
{
    public class MultiplierController
    {
        private int _currentLevelIndex;

        private int[] _meteorsNeeded;

        private float[] levelMultipliers = new float[]
        {
            0.60f,
            0.65f, // 1
            0.70f, 
            0.75f, 
            0.80f, 
            0.85f, 
            0.91f, 
            0.97f, 
            1.03f,
            1.10f, 
            1.17f,
            1.24f,
            1.32f, 
            1.40f, 
            1.48f, 
            1.56f, 
            1.65f, 
            1.74f, 
            1.83f, 
            1.92f, 
            2.00f // 20
        };
        
        
        public MultiplierController()
        {
            SetMeteorsNeeded();
        }

        private void SetMeteorsNeeded()
        {
            int initialNeed = GameValues.BaseMeteorNeed;
            
            _meteorsNeeded = new int[20];
            _meteorsNeeded[0] = initialNeed;
            for (int i = 1; i < 20; i++)
            {
                _meteorsNeeded[1] = Mathf.RoundToInt(_meteorsNeeded[i - 1] * 1.5f);
            }
        }

        public void Restart()
        {
            _currentLevelIndex = 0;
        }

        public float GetCurrentMultiplier()
        {
            return levelMultipliers[_currentLevelIndex];
        }

        public void CheckForNextLevel(int meteorCount)
        {
            if (_meteorsNeeded[_currentLevelIndex] == meteorCount)
            {
                _currentLevelIndex++;
            }
        }

    }
}