using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay.FSM.Ability;
using _Main.Scripts.Managers;

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
            Disabled
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
            
            temp.Add(enable);
            temp.Add(running);
            temp.Add(disable);

            #endregion

            #region Transitions
            
            enable.AddTransition(States.Disabled, disable);
            enable.AddTransition(States.Running, running);
            
            disable.AddTransition(States.Enable, enable);
            
            running.AddTransition(States.Enable, enable);

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

        #endregion

        #endregion

        public void TryAddAbility(AbilityType abilityType)
        {
            _motor.TryAddAbility(abilityType);
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
    }
}