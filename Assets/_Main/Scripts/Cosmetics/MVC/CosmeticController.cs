using System.Collections.Generic;
using _Main.Scripts.DebugGUI;
using _Main.Scripts.FiniteStateMachine;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticController
    {
        private readonly CosmeticMotor _motor;
        private FSM<States> _fsm;
        
        private enum States
        {
            None,
            Enable,
            Initial,
            Disable
        }

        public CosmeticController(CosmeticMotor motor)
        {
            _motor = motor;
        }

        public void InitializeValues()
        {
            InitializeFsm();
        }

        #region FSM

        private void InitializeFsm()
        {
            var temp = new List<BaseState<States>>();
            _fsm = new FSM<States>("Cosmetic");
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _fsm.CreateDebugGUI(DebugGUISortingOrder.SubGroup.Cosmetics);
#endif
            

            #region Variables

            var none = new BaseState<States>();
            var enable = new EnableState<States>();
            var disable = new DisableState<States>();
            var initial = new InitialState<States>();
            
            temp.Add(none);
            temp.Add(enable);
            temp.Add(disable);
            temp.Add(initial);


            #endregion

            #region Transitions

            none.AddTransition(States.Enable, enable);
            //
            enable.AddTransition(States.Initial, initial);
            
            initial.AddTransition(States.Disable, disable);
            
            disable.AddTransition(States.Enable, enable);

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
            SetTransition(States.Disable);
        }

        public void TransitionToInitial()
        {
            SetTransition(States.Initial);
        }

        #endregion

        #endregion

        #region Motor Caller

        public void Enable()
        {
            _motor.Enable();
        }

        public void Disable()
        {
            _motor.StartDisable();
        }
        
        public void ExecuteDisable()
        {
            if (_fsm.CurrentState == States.Disable)
            {
                _motor.Disable();
            }
        }

        public void Initial()
        {
            _motor.Initial();
        }
        
        public void TriggerMainMenu()
        {
            _motor.TriggerMainMenu();
        }

        #endregion

        public void SkinSelected(int index)
        {
            _motor.SelectSkin(index);
        }


    }
    #region States
    
    public class BaseState<T> : State<T>
    {
        protected CosmeticController Controller { get; private set; }

        public void Initialize(CosmeticController controller)
        {
            Controller = controller;
        }
    }

    public class EnableState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.Enable();
        }
    }
    
    public class DisableState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.Disable();
        }
    }
    
    public class InitialState<T> : BaseState<T>
    {
        public override void Awake()
        {
            Controller.Initial();
        }
    }
    
    #endregion
}