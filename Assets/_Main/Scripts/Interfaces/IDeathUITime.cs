namespace _Main.Scripts.Interfaces
{
    public interface IDeathUITime
    {
        public float ShowDeathUI { get; }
        public float SetEnableDeathText { get; }
        public float SetEnableDeathScore { get; }
        public float DeathPointsTimeToIncrease { get; }
        public float CountDeathScore { get; }
        public float EnableRestartButton { get; }
    }
}