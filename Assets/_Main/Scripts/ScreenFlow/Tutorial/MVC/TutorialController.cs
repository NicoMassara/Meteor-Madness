using System.Collections.Generic;
using MeteorMadness.Systems;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Tutorial
{
    public class TutorialController :
        TutorialController.ITutorialController,
        TutorialController.IController
    {
        #region Interfaces
        public interface ITutorialController
        {
            public void Initialize();
            public void Execute(float deltaTime);
            public void TransitionToEnable();
            public void TransitionToMultiPage();
            public void TransitionToDisable();
            public void TransitionToMeteor();
            public void TransitionToAbility();
            public void SpawnExtraMeteors();
            public void TransitionToFinish();
            public void TriggerSphereDeflected();
            public void TransitionToAbilityRunning();
            public void SendAdditionalProjectile(int inputType);
            public void EnableHint();
            public void DisableHint();
            public void SetMovementDirection(float direction);
            public void TransitionToMoveRight();
            public void TransitionToMoveLeft();
        }
        private interface IController
        {
            public void SetAbility();
            public void SetDisable();
            public void SetEnable();
            public void SetFinish();
            public void SetMeteor();
            public void SetAbilityRunning();
            public void SetMultiPage();
            public void SetRightMovement();
            public void SetLeftMovement();
            public void ExecuteLeftMovement(float deltaTime);
            public void ExecuteRightMovement(float deltaTime);
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
    
            private class MeteorState<T> : BaseState<T>
            {
                public override void Awake() => Controller.SetMeteor();
            }

            private class AbilityRunningState<T> : BaseState<T>
            {
                public override void Awake() => Controller.SetAbilityRunning();
            }

            private class MultiPageState<T> : BaseState<T>
            {
                public override void Awake() => Controller.SetMultiPage();
            }

            private class MoveRightState<T> : BaseState<T>
            {
                public override void Awake() => Controller.SetRightMovement();
                public override void Execute(float deltaTime) => Controller.ExecuteRightMovement(deltaTime);
            }
            
            private class MoveLeftState<T> : BaseState<T>
            {
                public override void Awake() => Controller.SetLeftMovement();

                public override void Execute(float deltaTime) => Controller.ExecuteLeftMovement(deltaTime);
            }

            #endregion
            
            private FSM<States> _fsm;
            private enum States
            {
                None,
                Enable,
                MovementRight,
                MovementLeft,
                MeteorDeflect,
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
                    ProjectileReStockEnable = state is States.Ability or States.MeteorDeflect;
                }
            }
            
            private ActionGate _actionGate;

            public Controller(IController controller)
            {
                InitializeFsm(controller);
            }

            public void Execute(float deltaTime) => _fsm?.Execute(deltaTime);

            #region FSM

            private void InitializeFsm(IController controller)
            {
                var temp = new List<BaseState<States>>();
                _fsm = new FSM<States>("Tutorial");
                _actionGate = new ActionGate(_fsm);
                

                #region Variables

                var none = new BaseState<States>();
                var disable = new DisableState<States>();
                var enable = new EnableState<States>();
                var moveRight = new MoveRightState<States>();
                var moveLeft = new MoveLeftState<States>();
                var meteorDeflect = new MeteorState<States>();
                var ability = new AbilityState<States>();
                var finish = new FinishState<States>();
                var abilityRunning = new AbilityRunningState<States>();
                var multiPage = new MultiPageState<States>();
                
                temp.Add(none);
                temp.Add(disable);
                temp.Add(enable);
                temp.Add(moveRight);
                temp.Add(moveLeft);
                temp.Add(abilityRunning);
                temp.Add(meteorDeflect);
                temp.Add(ability);
                temp.Add(finish);
                temp.Add(multiPage);

                #endregion

                #region Transitions
                
                none.AddTransition(States.Enable, enable);
                
                enable.AddTransition(States.MultiPage, multiPage);
                
                // === //
                
                multiPage.AddTransition(States.MovementRight, moveRight);
                
                moveRight.AddTransition(States.MultiPage, multiPage);
                
                // === //
                
                multiPage.AddTransition(States.MovementLeft, moveLeft);
                
                moveLeft.AddTransition(States.MultiPage, multiPage);
                
                // === //
                
                multiPage.AddTransition(States.MeteorDeflect, meteorDeflect);
                
                meteorDeflect.AddTransition(States.MultiPage, multiPage);
                
                // === //
                
                multiPage.AddTransition(States.Ability, ability);
                
                ability.AddTransition(States.AbilityRunning, abilityRunning);
                
                // === //
                
                abilityRunning.AddTransition(States.Finish, finish);
                
                finish.AddTransition(States.MultiPage, multiPage);
                
                multiPage.AddTransition(States.Finish, finish);
                
                // === //
                
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
            private void SetTransition(States state) => _fsm?.Transitions(state);
            public void TransitionToEnable() => SetTransition(States.Enable);
            public void TransitionToDisable() => SetTransition(States.Disable);
            public void TransitionToMeteor() => SetTransition(States.MeteorDeflect);
            public void TransitionToRightMovement() => SetTransition(States.MovementRight);
            public void TransitionToLeftMovement() => SetTransition(States.MovementLeft);
            public void TransitionToAbility() => SetTransition(States.Ability);
            public void TransitionToAbilityRunning() => SetTransition(States.AbilityRunning);
            public void TransitionToFinish() => SetTransition(States.Finish);
            public void TransitionToMultiPage() => SetTransition(States.MultiPage);

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
        public void Execute(float deltaTime) => _controller.Execute(deltaTime);
        public void TransitionToEnable() => _controller.TransitionToEnable();
        public void TransitionToMultiPage() => _controller.TransitionToMultiPage();
        public void TransitionToDisable() => _controller.TransitionToDisable();
        public void TransitionToMeteor() => _controller.TransitionToMeteor();
        public void TransitionToAbility() => _controller.TransitionToAbility();
        public void TransitionToFinish() => _controller.TransitionToFinish();
        public void TransitionToAbilityRunning() => _controller.TransitionToAbilityRunning();
        public void SetMovementDirection(float direction) => _motor.SetMovementDirection(direction);
        public void TransitionToMoveRight() => _controller.TransitionToRightMovement();
        public void TransitionToMoveLeft() => _controller.TransitionToLeftMovement();

        #endregion
        
        #region IController

        public void SetMeteor() => _motor.Meteor();
        public void SetAbility() => _motor.Ability();
        public void SetFinish() => _motor.Finish();
        public void SetDisable() => _motor.Disable();
        public void SetEnable() => _motor.Enable();
        public void SpawnExtraMeteors() => _motor.SpawnExtraMeteors();
        public void SetAbilityRunning() => _motor.SetAbilityRunning();
        
        public void SendAdditionalProjectile(int projectileTypeIndex)
        {
            if (_controller.GetCanReStockProjectile())
            {
                _motor.SendAdditionalProjectile(projectileTypeIndex);
            }
        }

        public void EnableHint() => _motor.EnableHint();
        public void DisableHint() => _motor.DisableHint();
        
        public void SetMultiPage() => _motor.SetMultiPage();
        public void SetRightMovement() => _motor.SetRightMovement();
        public void SetLeftMovement() => _motor.SetLeftMovement();
        public void ExecuteLeftMovement(float deltaTime) => _motor.ExecuteLeftMovement(deltaTime);
        public void ExecuteRightMovement(float deltaTime) => _motor.ExecuteRightMovement(deltaTime);
        public void TriggerSphereDeflected() => _motor.TriggerSphereDeflected();

        #endregion
    }
}