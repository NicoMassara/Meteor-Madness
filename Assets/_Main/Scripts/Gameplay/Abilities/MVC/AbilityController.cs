using System.Collections.Generic;
using MeteorMadness.Systems;
using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities
{
    public class AbilityController : 
        AbilityController.IAbilityController, 
        AbilityController.IController,
        AbilityController.IAbilityUI
    {
        #region Intefaces

        public interface IAbilityController
        {
            public void Initialize();
            public void TryInitialize();
            public void SelectAbility();
            public void TryAddAbility(int inputAbilityType, Vector2 inputPosition);
            public void SetCanUse(bool inputCanUse);
            public void RunActiveTimer();
            public void TryEnableUI();
            public void TryDisableUI();
            public void TryEnableAbility();
            public void TryDisableAbility();
            public void TryTriggerAbility();
        }
        
        private interface IController
        {
            public void TriggerAbility();
            public void FinishAbility();
            public void SetCanUseAbility(bool canUse);
            public void RestartAbilities();
            public void ForceFinishAbility();
            public void DisableUI();
            public void InitializeData();
        }
        
        #endregion
        
        #region UI Controller
        
        private interface IAbilityUI
        {
            public void SetEnableUI(bool isEnable);
        }

        private class UIController
        {
            #region States

            private class BaseState<T> : State<T>
            {
                protected IAbilityUI Controller { get; private set; }

                public void Initialize(IAbilityUI controller)
                {
                    Controller = controller;
                }
            }
            
            private class EnableState<T> : BaseState<T>
            {
                public override void Awake() => Controller.SetEnableUI(true);
            }
    
            private class DisableState<T> : BaseState<T>
            {
                public override void Awake() => Controller.SetEnableUI(false);
            }

            #endregion
            
            private enum States
            {
                None,
                Enable,
                Disabled,
            }
            private FSM<States> _fsm;

            public UIController(IAbilityUI controller)
            {
                InitializeFsm(controller);
            }

            #region FSM

            private void InitializeFsm(IAbilityUI controller)
            {
                var temp = new List<BaseState<States>>();
                _fsm = new FSM<States>("Ability - UI");
                

                #region Variables

                var none = new BaseState<States>();
                var enable = new EnableState<States>();
                var disable = new DisableState<States>();
            
                temp.Add(none);
                temp.Add(enable);
                temp.Add(disable);

                #endregion

                #region Transitions
            
                none.AddTransition(States.Enable, enable);
            
                enable.AddTransition(States.Disabled, disable);
            
                disable.AddTransition(States.Enable, enable);
                

                #endregion

                foreach (var state in temp)
                {
                    state.Initialize(controller);
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

            #endregion

            #endregion
        }

        #endregion

        #region Main Controller

        private class MainController
        {
            #region States

            private class BaseState<T> : State<T>
            {
                protected IController Controller { get; private set; }

                public void Initialize(IController controller)
                {
                    Controller = controller;
                }
            }
    
            private class RunningState<T> : BaseState<T>
            {
                public override void Awake() => Controller.TriggerAbility();
                public override void Sleep() => Controller.FinishAbility();
            }
            
            private class InitializeState<T> : BaseState<T>
            {
                public override void Awake() => Controller.InitializeData();
            }
    
            private class EnableState<T> : BaseState<T>
            {
                public override void Awake() => Controller.SetCanUseAbility(true);

                public override void Sleep() => Controller.SetCanUseAbility(false);
            }
    
            private class DisableState<T> : BaseState<T>
            {
                public override void Awake()
                {
                    Controller.DisableUI();
                    Controller.RestartAbilities();
                    Controller.ForceFinishAbility();
                    Controller.SetCanUseAbility(false);
                }
            }

            #endregion
            
            private enum States
            {
                None,
                Initialize,
                Enable,
                Running,
                Disabled,
            }
            private FSM<States> _fsm;
        
            private class AbilityActionGate : FsmActionGate<States>
            {
                public bool IsAbilityDisabled { get; private set; }
                public bool IsInitialized { get; private set; }
                public AbilityActionGate(FSM<States> fsm) : base(fsm) { }

                protected override void OnEnterState(States state)
                {
                    IsAbilityDisabled = state is States.Disabled;
                    
                    if (state is States.Initialize)
                    {
                        IsInitialized = true;
                    }
                }
            }
            
            private AbilityActionGate _actionGate;

            public MainController(IController controller)
            {
                InitializeFsm(controller);
            }

            #region FSM

            private void InitializeFsm(IController controller)
            {
                var temp = new List<BaseState<States>>();
                _fsm = new FSM<States>("Ability");
                _actionGate = new AbilityActionGate(_fsm);

                #region Variables

                var none = new BaseState<States>();
                var initialize = new InitializeState<States>();
                var enable = new EnableState<States>();
                var disable = new DisableState<States>();
                var running = new RunningState<States>();
            
                temp.Add(none);
                temp.Add(initialize);
                temp.Add(enable);
                temp.Add(running);
                temp.Add(disable);

                #endregion

                #region Transitions


                none.AddTransition(States.Initialize, initialize);
                
                initialize.AddTransition(States.Enable, enable);
            
                enable.AddTransition(States.Disabled, disable);
                enable.AddTransition(States.Running, running);
            
                running.AddTransition(States.Enable, enable);
                running.AddTransition(States.Disabled, disable);
            
                disable.AddTransition(States.Enable, enable);

                #endregion

                foreach (var state in temp)
                {
                    state.Initialize(controller);
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
            
            public void TransitionToInitialize()
            {
                SetTransition(States.Initialize);
            }

            #endregion

            #endregion

            public bool GetIsAbilityDisabled() => _actionGate.IsAbilityDisabled;

            public bool GetIsInitialized() => _actionGate.IsInitialized;
        }

        #endregion
        
        private readonly AbilityMotor _motor;
        private MainController _mainController;
        private UIController _uiController;
        
        public AbilityController(AbilityMotor motor)
        {
            _motor = motor;
        }
        
        #region IAbilityController

        public void Initialize()
        {
            _mainController = new MainController(this);
            _uiController = new UIController(this);
        }

        public void TryInitialize()
        {
            _mainController.TransitionToInitialize();
        }

        public void TryEnableUI()
        {
            if (_mainController.GetIsAbilityDisabled()) return;
            
            _uiController.TransitionToEnable();
        }

        public void TryDisableUI()
        {
            _uiController.TransitionToDisable();
        }

        public void TryEnableAbility()
        {
            if (_mainController.GetIsInitialized())
            {
                _mainController.TransitionToEnable();
            }
            else
            {
                _mainController.TransitionToInitialize();
            }
        }
        public void TryDisableAbility() => _mainController.TransitionToDisable();
        public void TryTriggerAbility()
        {
            _mainController.TransitionToRunning();
        }

        public void TryAddAbility(int abilityTypeIndex, Vector2 abilityPosition)
        {
            if(_mainController.GetIsAbilityDisabled()) return;
            
            _motor.TryAddAbility(abilityTypeIndex,abilityPosition);
        }


        public void SelectAbility()
        {
            if(_mainController.GetIsAbilityDisabled()) return;
            
            _motor.SelectAbility();
        }

        public void RunActiveTimer() => _motor.RunActiveTimer();

        public void SetCanUse(bool inputCanUse) => _motor.SetCanUseAbility(inputCanUse);

        #endregion
        
        #region IController

        public void FinishAbility()
        {
            _motor.FinishAbility();
        }

        public void RestartAbilities()
        {
            _motor.RestartAbilities();
        }
        
        public void ForceFinishAbility()
        {
            _motor.ForceFinishAbility();
        }

        public void DisableUI()
        {
            _uiController.TransitionToDisable();
        }

        public void InitializeData()
        {
            _motor.InitializeData();
        }

        public void TriggerAbility()
        {
            _motor.TriggerAbility();
        }
        
        public void SetCanUseAbility(bool canUse)
        {
            _motor.SetCanUseAbility(canUse);
        }

        #endregion
        
        #region IAbilityUI

        public void SetEnableUI(bool isEnable)
        {
            _motor.SetEnableUI(isEnable);
        }

        #endregion
    }
}