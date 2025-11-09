namespace _Main.Scripts.Managers.UpdateManager
{
    public interface IUpdatable
    {
        public UpdateGroup SelfUpdateGroup { get; }
        public TickGroup SelfTickGroup { get; }
        public float LastUpdateTime { get; set; }
        void ManagedUpdate();
    }
}