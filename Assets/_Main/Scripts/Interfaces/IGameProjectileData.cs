namespace _Main.Scripts.Interfaces
{
    public interface IGameProjectileData
    {
        public int MaxProjectileSpeed { get; }
        public (float[] speed, float[] travel) GetTravelData();
        public (int[] minSlot, int[] maxSlot) GetSlotData();
    }
}