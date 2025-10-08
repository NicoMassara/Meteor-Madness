namespace _Main.Scripts.Interfaces
{
    public interface IMeteorRingData
    {
        public int MeteorAmount { get; }
        public int RingsAmount { get; }
        public int WavesAmount { get; }
        public float DelayBetweenRings { get; }
        public float DelayBetweenWaves { get; }
    }
}