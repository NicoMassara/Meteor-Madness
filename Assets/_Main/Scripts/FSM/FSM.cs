using System;
using _Main.Scripts.DebugGUI;
using UnityEngine;

namespace _Main.Scripts.FiniteStateMachine
{
    public class FSM<T>
    {
        IState<T> _current;
        /// <summary>
        /// If FALSE Needs a confirmation to sleep 
        /// </summary>
        private IState<T> _newState;
        private T _newInput;
        public T CurrentState { get; set; }
        public T LastState { get; set; }
        public string FSMName { get; private set; }
        

        public event Action<T> OnEnterState;
        public event Action<T> OnExitState;
        public event Action<T> OnNewState;

        public FSM(string fsmName)
        {
            FSMName = fsmName;
        }


#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public void CreateDebugGUI(int sortingOrder = 10)
        {
            DebugGUIManager.Instance.CreateGroup(DebugGUIKeys.Group.Fsm, DebugGUISortingOrder.Group.Fsm)
                ?.CreateSubGroup($"{FSMName}",sortingOrder)
                ?.AddEntry(
                () => $"Current State: {(CurrentState == null ? "None" : CurrentState)}",
                () => $"Last State: {(LastState == null ? "None" : LastState)}"
            );
        }
#endif
        
        public void SetInit(IState<T> init)
        {
            _current = init;
            _current.Awake();
        }
        
        public void Execute(float deltaTime)
        {
            if (_current != null)
                _current.Execute(deltaTime);
        }

        public void FixedExecute(float fixedDeltaTime)
        {
            if(_current != null)
                _current.FixedExecute(fixedDeltaTime);
        }
        
        public void LateExecute(float deltaTime)
        {
            if(_current != null)
                _current.LateExecute(deltaTime);
        }

        public void Transitions(T input)
        {
            IState<T> newState = _current.GetTransition(input);
            
            if (newState == null)
            {
                //Debug.Log($"Transition From {CurrentState.ToString()} to {input.ToString()} Not Found in {FSMName} FSM");
                return;
            }

            OnNewState?.Invoke(input);

            if (CurrentState != null)
            {
                LastState = CurrentState;
                OnExitState?.Invoke(CurrentState);
            }
            
            
            if (_current == null || _current.IsManualSleep == false)
            {
                _current.Sleep();
                _current = newState;
                CurrentState = input;
                OnEnterState?.Invoke(CurrentState);
                _current.Awake();
            }
            else
            {
                _newState = newState;
                _newInput = input;
                _current.OnSleepFinished += OnSleepFinishedHandler;
                _current.Sleep();
            }
        }

        private void OnSleepFinishedHandler()
        {
            _current.OnSleepFinished -= OnSleepFinishedHandler;
            //
            _current = _newState;
            CurrentState = _newInput;
            OnEnterState?.Invoke(CurrentState);
            _current.Awake();
        }
    }
    
    public abstract class FsmActionGate<T>
    {
        protected T NewState { get; private set; }
        protected T LastState { get; private set; }
        protected T CurrentState { get; private set; }
        
        protected FsmActionGate(FSM<T> fsm)
        {
            fsm.OnNewState += (value) =>
            {
                NewState = value;
                OnNewState(value);
            };
            fsm.OnExitState += (value) =>
            {
                LastState = value;
                OnExitState(value);
            };
            fsm.OnEnterState += (value) =>
            {
                CurrentState = value;
                OnEnterState(value);
            };
        }
            
        protected virtual void OnNewState(T state){}
        protected virtual void OnExitState(T state){}
        protected virtual void OnEnterState(T state){}
    }
}
