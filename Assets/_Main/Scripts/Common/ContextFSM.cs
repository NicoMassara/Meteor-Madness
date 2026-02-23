using System;
using MeteorMadness.Common;

namespace _Main.Scripts.Common
{
    public class ContextFSM<T,TS,TD>
        where T : IFsmController
        where TS : Enum
        where TD : StateContext
    {
        protected readonly T FsmController;
        public TS CurrentState { get; private set; }
        
        public event Action<TS,TS> OnStateChange;

        protected ContextFSM(T controller)
        {
            FsmController = controller;
        }
        

        public void Update(float deltaTime) => ExecuteState(deltaTime);

        public void ChangeState(TS newState, TD awakeContext = null, TD sleepContext = null, bool canReEnter = false)
        {
            if(canReEnter == false)
                if(AreEqual(CurrentState,newState)) return;
            
            SleepState(CurrentState, sleepContext);
            
            // LastState, New State
            OnStateChange?.Invoke(newState, CurrentState = newState);
            CurrentState = newState;
            
            
            AwakeState(CurrentState, awakeContext);
        }

        protected virtual void AwakeState(TS state, TD context){}
        protected virtual void ExecuteState(float deltaTime){}
        protected virtual void SleepState(TS state, TD context){}
        
        private bool AreEqual(TS a, TS b)
        {
            return Equals(a, b);
        }
    }

    public abstract class StateContext { }
}