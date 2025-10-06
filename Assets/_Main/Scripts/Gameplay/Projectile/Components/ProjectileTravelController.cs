using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile
{
    public class ProjectileTravelController 
    {
        private TravelData _travelData;
        
        private class TravelData
        {
            public int LevelIndex;
            private float[] movementMultiplier;
            private float[] travelRatio;

            public TravelData(float[] movementMultiplier, float[] travelRatio)
            {
                this.movementMultiplier = movementMultiplier;
                this.travelRatio = travelRatio;
                LevelIndex = 0;
            }

            public float GetMovementMultiplier()
            {
                return movementMultiplier[LevelIndex];
            }

            public float GetTravelRatio()
            {
                return travelRatio[LevelIndex];
            }
        }

        public ProjectileTravelController(float[] movementMultiplier, float[] travelRatio)
        {
            _travelData = new TravelData(movementMultiplier, travelRatio);
        }

        public float GetMaxTravelDistance()
        {
            return _travelData.GetTravelRatio();
        }

        public float GetMovementMultiplier()
        {
            return _travelData.GetMovementMultiplier();
        }

        public void SetLevel(int levelIndex)
        {
            _travelData.LevelIndex = levelIndex;
        }
    }
}