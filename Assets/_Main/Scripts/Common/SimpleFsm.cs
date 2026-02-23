using System;

namespace MeteorMadness.Common
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

        protected virtual void AwakeState(S state){}
        protected virtual void ExecuteState(float deltaTime){}
        protected virtual void SleepState(S state){}
        
        private bool AreEqual(S a, S b)
        {
            return Equals(a, b);
        }
    }
    public interface IFsmController { }
}