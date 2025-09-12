namespace _Main.Scripts.Managers.UpdateManager.Interfaces
{
    public interface ILateUpdatable
    {
        public UpdateGroup SelfLateUpdateGroup { get; }
        void ManagedLateUpdate();
    }
}