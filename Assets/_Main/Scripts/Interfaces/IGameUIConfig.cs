namespace _Main.Scripts.Interfaces
{
    public interface IGameUIConfig
    {
        public float GameplayPointsTimeToIncrease { get; }
        public float ClosePauseMenu { get; }
        public IDeathUITime DeathUITimeData { get; }
        public IUITextData TextData { get; }
    }
}