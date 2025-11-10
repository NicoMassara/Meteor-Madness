using System.Collections.Generic;
using _Main.Scripts.DebugGUI;
using _Main.Scripts.FiniteStateMachine;

namespace _Main.Scripts.Tutorial.MVC
{
    public class TutorialController
    {
        private readonly TutorialMotor _motor;
        private FSM<States> _fsm;
        private enum States
        {
            None,
            Enable,
            Start,
            Movement,
            Ability,
            AbilityRunning,
            Finish,
            Disable,
            MultiPage
        }
        
        private class ActionGate
        {
            public bool ProjectileReStockEnable { get; private set; }
            public bool CanMultiPage { get; private set; }
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
            var temp = new List<BaseState<States>>();
            _fsm = new FSM<States>("Tutorial");
            _actionGate = new ActionGate(_fsm);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _fsm.CreateDebugGUI(DebugGUISortingOrder.SubGroup.Tutorial);
#endif

            #region Variables

            var none = new BaseState<States>();
            var disable = new DisableState<States>();
            var enable = new EnableState<States>();
            var start = new StartState<States>();
            var movement = new MovementState<States>();
            var ability = new AbilityState<States>();
            var finish = new FinishState<States>();
            var abilityRunning = new AbilityRunningState<States>();
            var multiPage = new MultiPageState<States>();
            
            temp.Add(none);
            temp.Add(disable);
            temp.Add(enable);
            temp.Add(start);
            temp.Add(abilityRunning);
            temp.Add(movement);
            temp.Add(ability);
            temp.Add(finish);
            temp.Add(multiPage);

            #endregion

            #region Transitions
            
            none.AddTransition(States.Enable, enable);
            
            enable.AddTransition(States.Start, start);
            enable.AddTransition(States.MultiPage, multiPage);
            enable.AddTransition(States.Disable, disable);
            
            start.AddTransition(States.Movement, movement);
            start.AddTransition(States.Disable, disable);
            start.AddTransition(States.MultiPage, multiPage);
            
            movement.AddTransition(States.Ability, ability);
            movement.AddTransition(States.MultiPage, multiPage);
            
            ability.AddTransition(States.AbilityRunning, abilityRunning);
            ability.AddTransition(States.MultiPage, multiPage);
            
            abilityRunning.AddTransition(States.Finish, finish);
            
            finish.AddTransition(States.Disable, disable);
            finish.AddTransition(States.MultiPage, multiPage);
            
            disable.AddTransition(States.Enable, enable);
            
            multiPage.AddTransition(States.Enable, enable);
            multiPage.AddTransition(States.Start, start);
            multiPage.AddTransition(States.Movement, movement);
            multiPage.AddTransition(States.Ability, ability);
            multiPage.AddTransition(States.Disable, disable);
            
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

        public void TransitionToMultiPage()
        {
            SetTransition(States.MultiPage);
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

        public void SetAbilityRunning()
        {
            _motor.SetAbilityRunning();
        }

        public void SendAdditionalProjectile(int projectileTypeIndex)
        {
            if (_actionGate.ProjectileReStockEnable)
            {
                _motor.SendAdditionalProjectile(projectileTypeIndex);
            }
        }

        public void SetMultiPage()
        {
            _motor.SetMultiPage();
        }

        public void TriggerSphereDeflected()
        {
            _motor.TriggerSphereDeflected();
        }
    }

    #region States
    
    public class BaseState<T> : State<T>
    {
        protected TutorialController Controller { get; private set; }

        public void Initialize(TutorialController controller)
        {
            Controller = controller;
        }
    }
    
    public class AbilityState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.SetAbility();
        }
    }
    
    public class DisableState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.SetDisable();
        }
    }
    
    public class EnableState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.SetEnable();
        }
    }
    
    public class FinishState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.SetFinish();
            Controller.TransitionToMultiPage();
        }
    }
    
    public class MovementState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.SetMovement();
        }
    }
    
    public class StartState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.SetStart();
        }
    }

    public class AbilityRunningState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.SetAbilityRunning();
        }
    }

    public class MultiPageState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.SetMultiPage();
        }
    }

    #endregion
}