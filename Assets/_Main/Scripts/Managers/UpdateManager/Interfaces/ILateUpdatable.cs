namespace _Main.Scripts.Managers.UpdateManager.Interfaces
{
    public interface ILateUpdatable
    {
        public UpdateManager.UpdateGroup UpdateGroup { get; }
        void ManagedLateUpdate();
    }
}