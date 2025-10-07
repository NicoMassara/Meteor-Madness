namespace _Main.Scripts.Interfaces
{
    public interface IGameProjectileData
    {
        public float MaxProjectileSpeed { get; }
        public float GetSpeedMultiplier(float index);
        public float GetTravelRatio(float index);
        public (int[] minSlot, int[] maxSlot) GetSlotData();
    }
}