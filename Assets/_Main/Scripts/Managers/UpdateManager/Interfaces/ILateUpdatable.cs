namespace _Main.Scripts.Managers.UpdateManager
{
    public interface ILateUpdatable
    {
        public UpdateGroup SelfLateUpdateGroup { get; }
        void ManagedLateUpdate();
    }
}