using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay.Ability.States;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityController
    {
        private readonly AbilityMotor _motor;
        private FSM<States> _fsm;
        
        private enum States
        {
            Enable,
            Running,
            Disabled,
            Restart
        }

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
            var temp = new List<AbilityBaseState<States>>();
            _fsm = new FSM<States>();

            #region Variables

            var enable = new AbilityEnableState<States>();
            var disable = new AbilityDisableState<States>();
            var running = new AbilityRunningState<States>();
            var restart = new AbilityRestartState<States>();
            
            temp.Add(enable);
            temp.Add(running);
            temp.Add(disable);
            temp.Add(restart);

            #endregion

            #region Transitions
            
            enable.AddTransition(States.Disabled, disable);
            enable.AddTransition(States.Running, running);
            enable.AddTransition(States.Restart, restart);
            
            disable.AddTransition(States.Enable, enable);
            disable.AddTransition(States.Restart, restart);
            
            running.AddTransition(States.Enable, enable);
            
            restart.AddTransition(States.Enable, enable);
            restart.AddTransition(States.Disabled, disable);

            #endregion

            foreach (var state in temp)
            {
                state.Initialize(this);
            }
            
            _fsm.SetInit(enable);
            _fsm.FSMName = "Ability";
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

        public void TransitionToRestart()
        {
            SetTransition(States.Restart);
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

        public void SetEnableUIAbility(bool isEnable)
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
    }
    
    #region States

    public class AbilityRunningState<T> : AbilityBaseState<T>
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
    
    public class AbilityRestartState<T> : AbilityBaseState<T>
    {
        public override void Awake()
        {
            Controller.RestartAbilities();
            Controller.TransitionToEnable();
        }
    }
    
    public class AbilityEnableState<T> : AbilityBaseState<T>
    {
        public override void Awake()
        {
            Controller.SetCanUseAbility(true);
            Controller.SetEnableUIAbility(true);
        }

        public override void Sleep()
        {
            Controller.SetCanUseAbility(false);
        }
    }
    
    public class AbilityDisableState<T> : AbilityBaseState<T>
    {
        public override void Awake()
        {
            Controller.SetCanUseAbility(false);
            Controller.SetEnableUIAbility(false);
        }
    }

    #endregion
}