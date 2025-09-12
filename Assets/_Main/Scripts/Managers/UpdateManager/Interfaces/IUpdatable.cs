namespace _Main.Scripts.Managers.UpdateManager.Interfaces
{
    public interface IUpdatable
    {
        public UpdateGroup SelfUpdateGroup { get; }
        void ManagedUpdate();
    }
}