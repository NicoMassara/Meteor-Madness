namespace _Main.Scripts.Interfaces
{
    public interface IShakeData
    {
        public float ShakeTime { get; }
        public bool DoesLoop { get; }
        public float ShakeIntensity { get; }
        public float XShakeMagnitude { get; }
        public float YShakeMagnitude { get; }
    }
}