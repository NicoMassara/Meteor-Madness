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
            AbilityRunning,
            Finish,
            Disable
        }
        
        private class ActionGate
        {
            public bool ProjectileReStockEnable { get; private set; }
            public ActionGate(FSM<States> fsm)
            {
                fsm.OnEnterState += state =>
                {
                    ProjectileReStockEnable = state is States.Ability or States.Movement;
                };
            }
        }
        
        private ActionGate _actionGate;
        
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
            _actionGate = new ActionGate(_fsm);

            #region Variables

            var disable = new TutorialDisableState<States>();
            var enable = new TutorialEnableState<States>();
            var start = new TutorialStartState<States>();
            var movement = new TutorialMovementState<States>();
            var ability = new TutorialAbilityState<States>();
            var finish = new TutorialFinishState<States>();
            var abilityRunning = new TutorialAbilityRunningState<States>();
            
            temp.Add(disable);
            temp.Add(enable);
            temp.Add(start);
            temp.Add(abilityRunning);
            temp.Add(movement);
            temp.Add(ability);
            temp.Add(finish);

            #endregion

            #region Transitions
            
            enable.AddTransition(States.Start, start);
            
            start.AddTransition(States.Movement, movement);
            start.AddTransition(States.Disable, disable);
            
            movement.AddTransition(States.Ability, ability);
            
            ability.AddTransition(States.AbilityRunning, abilityRunning);
            
            abilityRunning.AddTransition(States.Finish, finish);
            
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
        public void TransitionToAbilityRunning()
        {
            SetTransition(States.AbilityRunning);
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

        public void SendAdditionalProjectile(int projectileTypeIndex)
        {
            if (_actionGate.ProjectileReStockEnable)
            {
                _motor.SendAdditionalProjectile(projectileTypeIndex);
            }
        }
        
    }

    #region States
    public class TutorialAbilityState<T> : TutorialStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetAbility();
        }
    }
    
    public class TutorialDisableState<T> : TutorialStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetDisable();
        }
    }
    
    public class TutorialEnableState<T> : TutorialStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetEnable();
        }
    }
    
    public class TutorialFinishState<T> : TutorialStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetFinish();
        }
    }
    
    public class TutorialMovementState<T> : TutorialStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetMovement();
        }
    }
    
    public class TutorialStartState<T> : TutorialStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetStart();
        }
    }
    
    public class TutorialAbilityRunningState<T> : TutorialStateBase<T> { }

    #endregion
}