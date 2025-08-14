namespace _Main.Scripts.FiniteStateMachine
{
    public interface IState<T>
    {
        void Awake();
        void Execute();
        void FixedExecute();
        void LateExecute();
        void Sleep();

        IState<T> GetTransition(T input);

        void AddTransition(T input, IState<T> state);
        void RemoveTransition(IState<T> state);
        void RemoveTransition(T input);
    }
}