namespace _Main.Scripts.Interfaces
{
    public interface IProjectileData
    {
        public IMeteorRingData MeteorRingData { get; }
        public float MaxProjectileSpeed { get; }
        public float MeteorSpawnDelayAfterRing { get; }
        public float GetSpeedMultiplier(float index);
        public float GetTravelRatio(float index);
        public (int[] minSlot, int[] maxSlot) GetSlotData();
    }
}