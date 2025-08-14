using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Main.Scripts.FiniteStateMachine
{
    public class FSM<T>
    {
        IState<T> _current;
        public T CurrentState { get; set; }
        public string FSMName { get; set; }

        public FSM() {}
        public FSM(IState<T> init)
        {
            SetInit(init);
        }
        public void SetInit(IState<T> init)
        {
            _current = init;
            _current.Awake();
        }
        public void Execute()
        {
            if (_current != null)
                _current.Execute();
        }

        public void FixedExecute()
        {
            if(_current != null)
                _current.FixedExecute();
        }
        
        public void LateExecute()
        {
            if(_current != null)
                _current.LateExecute();
        }

        public void Transitions(T input)
        {
            IState<T> newState = _current.GetTransition(input);
            
            if (newState == null)
            {
                //Debug.Log($"Transition From {CurrentState.ToString()} to {input.ToString()} Not Found in {FSMName}");
                return;
            }

            _current.Sleep();
            _current = newState;
            _current.Awake();
            CurrentState = input;
        }
    }
}
