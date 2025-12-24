using System.Collections.Generic;
using _Main.Scripts.DebugGUI;
using _Main.Scripts.FiniteStateMachine;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticController
    {
        #region States
    
        private class BaseState<T> : State<T>
        {
            protected CosmeticController Controller { get; private set; }
            public void Initialize(CosmeticController controller) => Controller = controller;
        }

        private class EnableState<T> : BaseState<T>
        {
            public override void Awake() => Controller.Enable();
        }
    
        private class DisableState<T> : BaseState<T>
        {
            public override void Awake() => Controller.Disable();
        }
    
        private class InitialState<T> : BaseState<T>
        {
            public override void Awake() => Controller.Initialize();
        }
    
        #endregion
        
        private readonly CosmeticMotor _motor;
        private FSM<States> _fsm;
        
        private enum States
        {
            None,
            Initialize,
            Enable,
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
            var initial = new InitialState<States>();
            var enable = new EnableState<States>();
            var disable = new DisableState<States>();
            
            temp.Add(none);
            temp.Add(initial);
            temp.Add(enable);
            temp.Add(disable);


            #endregion

            #region Transitions

            none.AddTransition(States.Initialize, initial);
            //
            
            initial.AddTransition(States.Enable, enable);
            
            enable.AddTransition(States.Disable, disable);
            
            disable.AddTransition(States.Initialize, initial);

            #endregion

            foreach (var state in temp)
            {
                state.Initialize(this);
            }
            
            _fsm.SetInit(none);
        }
        
        #region Transitions

        private void SetTransition(States state) => _fsm?.Transitions(state);
        public void TransitionToEnable() => SetTransition(States.Enable);
        public void TransitionToDisable() => SetTransition(States.Disable);
        public void TransitionToInitialize() => SetTransition(States.Initialize);

        #endregion

        #endregion

        #region Motor Caller

        public void Enable() => _motor.Enable();

        public void Disable() => _motor.StartDisable();

        public void ExecuteDisable()
        {
            if (_fsm.CurrentState == States.Disable)
            {
                _motor.Disable();
            }
        }

        public void Initialize() => _motor.Initialize();

        public void TriggerMainMenu() => _motor.TriggerMainMenu();

        #endregion

        public void SkinSelected(int index) => _motor.SelectSkin(index);

        public void TriggerOpened() => _motor.TriggerOpened();

        public void TryUnlockSkin(int skinIndex) => _motor.TryUnlockSkin(skinIndex);
        public void SkinUnlocked(int skinIndex) => _motor.SkinUnlocked(skinIndex);
        public void FailedToUnlock() => _motor.FailedToUnlock();
        public void OpenFirstPanel() => _motor.OpenFirstPanel();
    }
}