namespace _Main.Scripts.Managers.UpdateManager.Interfaces
{
    public interface IUpdatable
    {
        public UpdateManager.UpdateGroup UpdateGroup { get; }
        void ManagedUpdate();
    }
}