namespace _Main.Scripts.Managers.UpdateManager
{
    public interface IFixedUpdatable
    {
        public UpdateGroup SelfFixedUpdateGroup { get; }
        void ManagedFixedUpdate();
    }
}