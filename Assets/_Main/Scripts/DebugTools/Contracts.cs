namespace MeteorMadness.Debug._Main.Scripts.Debug
{
    namespace _Main.Scripts.Contracts
    {
        public interface IDebugComponent
        {
            public string DebugName { get; }
            public bool IsDebugEnable { get; }
        }
    }
}