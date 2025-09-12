namespace _Main.Scripts.Managers.UpdateManager.Interfaces
{
    public interface IFixedUpdatable
    {
        public UpdateManager.UpdateGroup UpdateGroup { get; }
        void ManagedFixedUpdate();
    }
}