using System.Collections.Generic;

namespace _Main.Scripts.FiniteStateMachine
{
    public class State<T> : IState<T>
    {
        private readonly Dictionary<T, IState<T>> _transitions = new Dictionary<T, IState<T>>();
        
        public virtual void Awake(){}
        public virtual void Execute(float deltaTime){}
        public virtual void FixedExecute(float fixedDeltaTime){}
        public virtual void LateExecute(float deltaTime){}
        public virtual void Sleep(){}
        

        public IState<T> GetTransition(T input)
        {
            return _transitions.ContainsKey(input) ? _transitions[input] : null;
        }

        public void AddTransition(T input, IState<T> state)
        {
            _transitions[input] = state;
        }

        public void RemoveTransition(IState<T> state)
        {
            foreach (var item in _transitions)
            {
                if (item.Value == state)
                {
                    _transitions.Remove(item.Key);
                    break;
                }
            }
        }

        public void RemoveTransition(T input)
        {
            if (_transitions.ContainsKey(input))
            {
                _transitions.Remove(input);
            }
        }
    }
}