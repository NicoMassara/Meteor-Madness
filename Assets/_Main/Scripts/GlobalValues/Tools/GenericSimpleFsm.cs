using System;
using System.Collections.Generic;

namespace _Main.Scripts.GlobalValues.Tools
{
    public abstract class GenericSimpleFsm<T>
    {
        private readonly Dictionary<T, IFsmState> _states = new Dictionary<T, IFsmState>();
        
        private IFsmState _currentState;
        private T _currentStateType;
        
        public event Action<T> OnStateChanged;
        protected class StateData<TS>
        where TS : IFsmState
        {
            public readonly T StateType;
            public readonly TS State;

            public StateData(T stateType, TS state)
            {
                StateType = stateType;
                State = state;
            }
        }

        protected void InitializeStates<TS>(List<StateData<TS>> statesList)
        where TS : IFsmState
        {
            foreach (var state in statesList)
            {
                AddState(state.StateType, state.State);
            }
        }

        protected void AddState(T type, IFsmState state)
        {
            _states.Add(type, state);
        }
        
        protected void RemoveState(T type)
        {
            if (_states.ContainsKey(type)) 
                _states.Remove(type);
        }
        
        public virtual void Transition(T stateType)
        {
            if(_states.TryGetValue(stateType, out var newState) == false) return;
            _currentStateType = stateType;
            
            if(_currentState != null)
                _currentState.Sleep();
            
            OnStateChanged?.Invoke(_currentStateType);
            _currentState = newState;
            _currentState.Awake();
        }
        public void Execute(float deltaTime) => _currentState?.Execute(deltaTime);
        public T GetCurrentState() => _currentStateType;
    }
    
    public abstract class FsmState<T,TS> : IFsmState
    where TS : IFsmControllerTransitions
    {
        protected T Controller { get; private set; }
        protected TS Transitions { get; private set; }
        public void InitializeState(T controller, TS transitions)
        {
            Controller = controller;
            Transitions = transitions;
        }

        public virtual void Awake() { }
        public virtual void Execute(float deltaTime) { }
        public virtual void Sleep() { }
            
    }
    
    public interface IFsmState
    {
        public void Awake();
        public void Execute(float deltaTime);
        public void Sleep();
    }

    public interface IFsmControllerTransitions { }
}