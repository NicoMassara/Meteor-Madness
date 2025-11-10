using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticController
    {
        private readonly CosmeticMotor _motor;
        private FSM<States> _fsm;
        
        private enum States
        {
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
            var temp = new List<CosmeticStateBase<States>>();
            _fsm = new FSM<States>("Cosmetic");

            #region Variables

            var enable = new CosmeticEnableState<States>();
            var disable = new CosmeticDisableState<States>();
            var initial = new CosmeticInitialState<States>();
            
            temp.Add(enable);
            temp.Add(disable);
            temp.Add(initial);


            #endregion

            #region Transitions

            enable.AddTransition(States.Initial, initial);
            
            initial.AddTransition(States.Disable, disable);
            
            disable.AddTransition(States.Enable, enable);

            #endregion

            foreach (var state in temp)
            {
                state.Initialize(this);
            }
            
            _fsm.SetInit(disable);
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
            _motor.Disable();
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

    }

    #region States

    public class CosmeticEnableState<T> : CosmeticStateBase<T>
    {
        public override void Awake()
        {
            Controller.Enable();
        }
    }
    
    public class CosmeticDisableState<T> : CosmeticStateBase<T>
    {
        public override void Awake()
        {
            Controller.Disable();
        }
    }
    
    public class CosmeticInitialState<T> : CosmeticStateBase<T>
    {
        public override void Awake()
        {
            Controller.Initial();
        }
    }
    
    #endregion
}