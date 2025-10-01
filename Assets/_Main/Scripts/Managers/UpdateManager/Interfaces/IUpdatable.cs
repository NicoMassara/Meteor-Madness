namespace _Main.Scripts.Managers.UpdateManager
{
    public interface IUpdatable
    {
        public UpdateGroup SelfUpdateGroup { get; }
        void ManagedUpdate();
    }
}