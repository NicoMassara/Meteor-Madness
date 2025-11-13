namespace _Main.Scripts.Managers.UpdateManager
{
    public interface IBaseUpdatable
    {
        public UpdateGroup SelfUpdateGroup { get; }
        public TickGroup SelfTickGroup { get; }
        public float LastUpdateTime { get; set; }
        void ExecuteUpdate();
    }
    
    public interface IUpdatable : IBaseUpdatable { }
    public interface IFixedUpdatable : IBaseUpdatable { }
    public interface ILateUpdatable : IBaseUpdatable { }
}