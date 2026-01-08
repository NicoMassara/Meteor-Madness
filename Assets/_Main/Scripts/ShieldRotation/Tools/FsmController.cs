using System;
using System.Collections.Generic;
using UnityEngine.XR;

namespace _Main.Scripts.ShieldRotation.Tools
{
    public abstract class FsmController<T>
    {
        private readonly Dictionary<T, IState> _states = new Dictionary<T, IState>();
        
        private IState _currentState;
        private T _currentStateType;
        
        public event Action<T> OnStateChanged;
        
        protected class StateData
        {
            public readonly T StateType;
            public readonly IState State;

            public StateData(T stateType, IState state)
            {
                StateType = stateType;
                State = state;
            }
        }

        protected void InitializeStates(List<StateData> statesList)
        {
            foreach (var state in statesList)
            {
                AddState(state.StateType, state.State);
            }
        }

        protected void AddState(T type, IState state)
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
    
    public abstract class StateBase<T> : IState
    {
        protected T Controller { get; private set; }
        public void InitializeState(T controller) => Controller = controller;
        public virtual void Awake() { }
        public virtual void Execute(float deltaTime) { }
        public virtual void Sleep() { }
            
    }
    
    public interface IState
    {
        public void Awake();
        public void Execute(float deltaTime);
        public void Sleep();
    }
}