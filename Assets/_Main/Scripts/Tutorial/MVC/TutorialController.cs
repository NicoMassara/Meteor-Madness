using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Tutorial.States;

namespace _Main.Scripts.Tutorial.MVC
{
    public class TutorialController
    {
        private readonly TutorialMotor _motor;
        private FSM<States> _fsm;
        private enum States
        {
            Enable,
            Start,
            Movement,
            Ability,
            Finish,
            Disable
        }
        
        public TutorialController(TutorialMotor motor)
        {
            _motor = motor;
        }

        public void Initialize()
        {
            InitializeFsm();
        }

        public void Execute(float deltaTime)
        {
            _fsm?.Execute(deltaTime);
        }

        #region FSM

        private void InitializeFsm()
        {
            var temp = new List<TutorialStateBase<States>>();
            _fsm = new FSM<States>();

            #region Variables

            var disable = new TutorialDisableState<States>();
            var enable = new TutorialEnableState<States>();
            var start = new TutorialStartState<States>();
            var movement = new TutorialMovementState<States>();
            var ability = new TutorialAbilityState<States>();
            var finish = new TutorialFinishState<States>();
            
            temp.Add(disable);
            temp.Add(enable);
            temp.Add(start);
            temp.Add(movement);
            temp.Add(ability);
            temp.Add(finish);

            #endregion

            #region Transitions
            
            enable.AddTransition(States.Start, start);
            
            start.AddTransition(States.Movement, movement);
            
            movement.AddTransition(States.Ability, ability);
            
            ability.AddTransition(States.Finish, finish);
            
            finish.AddTransition(States.Disable, disable);
            
            disable.AddTransition(States.Enable, enable);
            
            #endregion
            
            foreach (var state in temp)
            {
                state.Initialize(this);
            }
            
            _fsm.SetInit(disable);
            _fsm.FSMName = "Tutorial";
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

        public void TransitionToStart()
        {
            SetTransition(States.Start);
        }
        
        public void TransitionToDisable()
        {
            SetTransition(States.Disable);
        }
        
        public void TransitionToMovement()
        {
            SetTransition(States.Movement);
        }
        
        public void TransitionToAbility()
        {
            SetTransition(States.Ability);
        }
        
        public void TransitionToFinish()
        {
            SetTransition(States.Finish);
        }

        #endregion
        
        #endregion

        public void SetStart()
        {
            _motor.Start();
        }

        public void SetMovement()
        {
            _motor.Movement();
        }

        public void SetAbility()
        {
            _motor.Ability();
        }

        public void SetFinish()
        {
            _motor.Finish();
        }
        
        public void SetDisable()
        {
            _motor.Disable();
        }

        public void SetEnable()
        {
            _motor.Enable();
        }

        public void SpawnExtraMeteors()
        {
            _motor.SpawnExtraMeteors();
        }
    }
}