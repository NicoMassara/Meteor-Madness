namespace _Main.Scripts.Interfaces
{
    public interface IEarthTime
    {
        public IEarthDestruction Destruction { get; }
        public IEarthSlice Slice { get; }
        public IEarthRestart Restart { get; }
    }
    
    public interface IEarthDestruction
    {
        public float StartTriggerDestructionTime { get; }
        public float EndTriggerDestructionTime { get; }
        public float StartShake { get; }
        public float DeathShakeDuration { get; }
        public float ShowEarthDestruction { get; }
        public float StartRotatingAfterDeath { get; }
    }
    
    public interface IEarthSlice
    {
        public float StartSlice { get; }
        public float MoveSlices { get; }
        public float ReturnToNormalTime { get; }
        public float ReturnSlices { get; }
    }
    
    public interface IEarthRestart
    {
        public float RestartZRotation { get; }
        public float RestartYRotation { get; }
        public float TimeBeforeRotateZ { get; }
        public float TimeBeforeRotateY { get; }
        public float RestartHealth { get; }
        public float FinishRestart { get; }
    }
}