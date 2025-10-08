namespace _Main.Scripts.Gameplay.Projectile
{
    public class ProjectileSlotController
    {
        private readonly SlotData _slot;
        
        private class SlotData
        {
            public int LevelIndex;
            private int[] minProximity;
            private int[] maxProximity;

            public SlotData(int[] minProximity, int[] maxProximity)
            {
                this.minProximity = minProximity;
                this.maxProximity = maxProximity;
                LevelIndex = 0;
            }

            public int GetMinSlot()
            {
                return minProximity[LevelIndex];
            }

            public int GetMaxSlot()
            {
                return maxProximity[LevelIndex];
            }
        }
        
        public ProjectileSlotController(int[] minProximity, int[] maxProximity)
        {
            _slot = new SlotData(minProximity, maxProximity);
        }

        public int GetMinSlotDistance()
        {
            return _slot.GetMinSlot();
        }

        public int GetMaxSlotDistance()
        {
            return _slot.GetMaxSlot();
        }

        public void SetLevel(int levelIndex)
        {
            _slot.LevelIndex = levelIndex;
        }
    }
}