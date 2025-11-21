using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityController
    {
        private readonly AbilityMotor _motor;
        private FSM<States> _fsm;
        
        private enum States
        {
            None,
            Enable,
            Running,
            Disabled,
        }
        
        private class AbilityActionGate : FsmActionGate<States>
        {
            public AbilityActionGate(FSM<States> fsm) : base(fsm) { }
            public bool IsAbilityEnable { get; private set; }

            protected override void OnNewState(States state)
            {

            }

            protected override void OnEnterState(States state)
            {
                IsAbilityEnable = state is not States.Disabled;
            }

            protected override void OnExitState(States state)
            {

            }
        }
        
        private AbilityActionGate _actionGate;

        public AbilityController(AbilityMotor motor)
        {
            _motor = motor;
        }

        public void Initialize()
        {
            InitializeFsm();
        }

        #region FSM

        private void InitializeFsm()
        {
            var temp = new List<BaseState<States>>();
            _fsm = new FSM<States>("Ability");
            _actionGate = new AbilityActionGate(_fsm);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _fsm.CreateDebugGUI(2);
#endif

            #region Variables

            var none = new BaseState<States>();
            var enable = new EnableState<States>();
            var disable = new DisableState<States>();
            var running = new RunningState<States>();
            
            temp.Add(none);
            temp.Add(enable);
            temp.Add(running);
            temp.Add(disable);

            #endregion

            #region Transitions
            
            none.AddTransition(States.Enable, enable);
            
            enable.AddTransition(States.Disabled, disable);
            enable.AddTransition(States.Running, running);
            
            disable.AddTransition(States.Enable, enable);
            
            running.AddTransition(States.Enable, enable);
            running.AddTransition(States.Disabled, disable);
            

            #endregion

            foreach (var state in temp)
            {
                state.Initialize(this);
            }
            
            _fsm.SetInit(none);
        }

        #region Transitions

        private void SetTransition(States state)
        {
            _fsm?.Transitions(state);
        }
        
        public void TransitionToEnable()
        {
            SetTransition(States.Enable);
        }

        public void TransitionToDisable()
        {
            SetTransition(States.Disabled);
        }
        
        public void TransitionToRunning()
        {
            SetTransition(States.Running);
        }

        #endregion

        #endregion

        public void TryAddAbility(int abilityTypeIndex, Vector2 abilityPosition)
        {
            _motor.TryAddAbility(abilityTypeIndex,abilityPosition);
        }

        public void SelectAbility()
        {
            _motor.SelectAbility();
        }

        public void TriggerAbility()
        {
            _motor.TriggerAbility();
        }
        
        public void SetCanUseAbility(bool canUse)
        {
            _motor.SetCanUseAbility(canUse);
        }

        public void SetEnableUI(bool isEnable)
        {
            _motor.SetEnableUI(isEnable);
        }

        public void FinishAbility()
        {
            _motor.FinishAbility();
        }

        public void RestartAbilities()
        {
            _motor.RestartAbilities();
        }

        public void RunActiveTimer()
        {
            _motor.RunActiveTimer();
        }

        public void ForceFinishAbility()
        {
            _motor.ForceFinishAbility();
        }

        public void SetCanUse(bool inputCanUse)
        {
            _motor.SetCanUseAbility(inputCanUse);
        }
    }
    
    #region States

    public class BaseState<T> : State<T>
    {
        protected AbilityController Controller { get; private set; }

        public void Initialize(AbilityController controller)
        {
            Controller = controller;
        }
    }
    
    public class RunningState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.TriggerAbility();
        }

        public override void Sleep()
        {
            Controller.FinishAbility();
        }
    }
    
    public class EnableState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.SetCanUseAbility(true);
            Controller.SetEnableUI(true);
        }

        public override void Sleep()
        {
            Controller.SetCanUseAbility(false);
        }
    }
    
    public class DisableState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.RestartAbilities();
            Controller.ForceFinishAbility();
            Controller.SetCanUseAbility(false);
            Controller.SetEnableUI(false);
        }
    }

    #endregion
}