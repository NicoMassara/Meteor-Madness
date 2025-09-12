namespace _Main.Scripts.Managers.UpdateManager.Interfaces
{
    public interface IFixedUpdatable
    {
        public UpdateGroup SelfFixedUpdateGroup { get; }
        void ManagedFixedUpdate();
    }
}