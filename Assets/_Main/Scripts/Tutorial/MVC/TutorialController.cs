using System.Collections.Generic;
using _Main.Scripts.DebugGUI;
using _Main.Scripts.FiniteStateMachine;
using UnityEngine;

namespace _Main.Scripts.Tutorial.MVC
{
    public class TutorialController :
        TutorialController.ITutorialController,
        TutorialController.IController
    {
        #region Interfaces
        public interface ITutorialController
        {
            public void Initialize();
            public void TransitionToEnable();
            public void TransitionToMultiPage();
            public void TransitionToDisable();
            public void TransitionToMovement();
            public void TransitionToAbility();
            public void SpawnExtraMeteors();
            public void TransitionToFinish();
            public void TriggerSphereDeflected();
            public void TransitionToAbilityRunning();
            public void SendAdditionalProjectile(int inputType);
        }
        private interface IController
        {
            public void SetAbility();
            public void SetDisable();
            public void SetEnable();
            public void SetFinish();
            public void SetMovement();
            public void SetAbilityRunning();
            public void SetMultiPage();
        }

        #endregion
        private class Controller
        { 
            #region States
    
            private class BaseState<T> : State<T>
            {
                protected IController Controller { get; private set; }

                public void Initialize(IController controller) => Controller = controller;
            }
    
            private class AbilityState<T> : BaseState<T>
            {
                public override void Awake() => Controller.SetAbility();
            }
    
            private class DisableState<T> : BaseState<T>
            {
                public override void Awake() => Controller.SetDisable();
            }
    
            private class EnableState<T> : BaseState<T>
            {
                public override void Awake() => Controller.SetEnable();
            }
    
            private class FinishState<T> : BaseState<T>
            {
                public override void Awake() => Controller.SetFinish();
            }
    
            private class MovementState<T> : BaseState<T>
            {
                public override void Awake() => Controller.SetMovement();
            }

            private class AbilityRunningState<T> : BaseState<T>
            {
                public override void Awake() => Controller.SetAbilityRunning();
            }

            private class MultiPageState<T> : BaseState<T>
            {
                public override void Awake() => Controller.SetMultiPage();
            }

            #endregion
            
            private FSM<States> _fsm;
            private enum States
            {
                None,
                Enable,
                Movement,
                Ability,
                AbilityRunning,
                Finish,
                Disable,
                MultiPage
            }
            private class ActionGate : FsmActionGate<States>
            {
                public ActionGate(FSM<States> fsm) : base(fsm) { }

                public bool ProjectileReStockEnable { get; private set; }

                protected override void OnEnterState(States state)
                {
                    ProjectileReStockEnable = state is States.Ability or States.Movement;
                }
            }
            
            private ActionGate _actionGate;

            public Controller(IController controller)
            {
                InitializeFsm(controller);
            }
            
            #region FSM

            private void InitializeFsm(IController controller)
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
                var movement = new MovementState<States>();
                var ability = new AbilityState<States>();
                var finish = new FinishState<States>();
                var abilityRunning = new AbilityRunningState<States>();
                var multiPage = new MultiPageState<States>();
                
                temp.Add(none);
                temp.Add(disable);
                temp.Add(enable);
                temp.Add(abilityRunning);
                temp.Add(movement);
                temp.Add(ability);
                temp.Add(finish);
                temp.Add(multiPage);

                #endregion

                #region Transitions
                
                none.AddTransition(States.Enable, enable);
                
                enable.AddTransition(States.MultiPage, multiPage);
                
                multiPage.AddTransition(States.Movement, movement);
                
                movement.AddTransition(States.MultiPage, multiPage);
                
                multiPage.AddTransition(States.Ability, ability);
                
                ability.AddTransition(States.AbilityRunning, abilityRunning);
                
                abilityRunning.AddTransition(States.Finish, finish);
                
                finish.AddTransition(States.MultiPage, multiPage);
                
                multiPage.AddTransition(States.Finish, finish);
                
                multiPage.AddTransition(States.Disable, disable);
                
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

            #region Public Getters

            public bool GetCanReStockProjectile() => _actionGate.ProjectileReStockEnable;

            #endregion
        }

        private Controller _controller;
        private readonly TutorialMotor _motor;
        
        public TutorialController(TutorialMotor motor)
        {
            _motor = motor;
        }

        #region ITutorialController

        public void Initialize()
        {
            _controller = new Controller(this);
        }

        public void TransitionToEnable()
        {
            _controller.TransitionToEnable();
        }

        public void TransitionToMultiPage()
        {
            _controller.TransitionToMultiPage();
        }

        public void TransitionToDisable()
        {
            _controller.TransitionToDisable();
        }

        public void TransitionToMovement()
        {
            _controller.TransitionToMovement();
        }

        public void TransitionToAbility()
        {
            _controller.TransitionToAbility();
        }

        public void TransitionToFinish()
        {
            _controller.TransitionToFinish();
        }
        
        public void TransitionToAbilityRunning()
        {
            _controller.TransitionToAbilityRunning();
        }

        #endregion
        
        #region IController

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
            if (_controller.GetCanReStockProjectile())
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

        #endregion
    }
}