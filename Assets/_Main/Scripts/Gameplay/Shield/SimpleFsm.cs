using System;
using System.Collections.Generic;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield
{
    public abstract class SimpleFsm<T,S>
    where T : IFsmController
    where S : Enum
    {
        protected readonly T FsmController;
        public S CurrentState { get; private set; }
        
        public event Action<S,S> OnStateChange;

        protected SimpleFsm(T controller)
        {
            FsmController = controller;
        }
        

        public void Update(float deltaTime) => ExecuteState(deltaTime);

        public void ChangeState(S newState, bool canReEnter = false)
        {
            if(canReEnter == false)
                if(AreEqual(CurrentState,newState)) return;
            
            SleepState(CurrentState);
            
            // LastState, New State
            OnStateChange?.Invoke(newState, CurrentState = newState);
            CurrentState = newState;
            
            
            AwakeState(CurrentState);
        }

        protected abstract void AwakeState(S state);
        protected abstract void ExecuteState(float deltaTime);
        protected abstract void SleepState(S state);
        
        private bool AreEqual(S a, S b)
        {
            return Equals(a, b);
        }
    }
    public interface IFsmController { }
}