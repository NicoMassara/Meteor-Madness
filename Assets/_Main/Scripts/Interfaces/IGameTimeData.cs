namespace _Main.Scripts.Interfaces
{
    public interface IGameTimeData
    {
        public int StartGameCount { get; }
        public int CometSpawnDelay { get; }
        public int FirstCometSpawnDelay { get; }
        public float TimeToLoadGameScene { get; }
        public float TriggerRestart { get; }
        public float RestartEarth { get; }
    }
}