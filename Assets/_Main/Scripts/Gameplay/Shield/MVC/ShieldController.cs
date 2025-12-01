using System;
using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldController : 
        ShieldController.IShieldController, 
        ShieldController.IMain,
        ShieldController.IAbility
    {
        
        public interface IShieldController
        {
            public void Initialize();
            public void Execute(float deltaTime);
            public void TryRotate(float direction);
            public void TryStop();
            public void TryEnableType(ShieldType type);

            public void TryEnable();
            public void TryDisable();
            public void TryHit(Vector3 inputPosition, Quaternion inputRotation, Vector2 inputDirection);
            public void TryDisableShieldType();
        }

        #region Private Classes
        
        #region Ability
        
        private interface IAbility
        {
            public void DisableAbility();
            public void EnableAutomatic();
            public void EnableSuper();
            public void EnableGold();
            public void EnableSlow();
        }

        private class AbilityController
        {
            #region States

            private class AbilityBaseState<T> : State<T>
            {
                public IAbility Controller { get; private set; }

                public void Initialize(IAbility controller) => Controller = controller;
            }

            private class NoneState<T> : AbilityBaseState<T>
            {
                public override void Awake() => Controller.DisableAbility();
            }
            
            private class AutomaticState<T> : AbilityBaseState<T>
            {
                public override void Awake() => Controller.EnableAutomatic();
            }
            private class GoldState<T> : AbilityBaseState<T>
            {
                public override void Awake() => Controller.EnableGold();
            }
            private class SuperState<T> : AbilityBaseState<T>
            {
                public override void Awake() => Controller.EnableSuper();
            }
            private class SlowState<T> : AbilityBaseState<T>
            {
                public override void Awake() => Controller.EnableSlow();
            }

            #endregion
            
            private enum States
            {
                None,
                Super,
                Gold,
                Automatic,
                Slow
            }
            
            private FSM<States> _fsm;

            private class ActionGate : FsmActionGate<States>
            {
                public bool IsRotationDisabled { get; private set; }

                public ActionGate(FSM<States> fsm) : base(fsm) { }

                protected override void OnEnterState(States state)
                {
                    IsRotationDisabled = state is States.Super or States.Automatic;
                }
            }
            
            private ActionGate _actionGate;

            public AbilityController(IAbility controller)
            {
                InitializeFsm(controller);
            }

            private void InitializeFsm(IAbility controller)
            {
                var temp = new List<AbilityBaseState<States>>();
                _fsm = new FSM<States>("Shield - Ability");
                _actionGate = new ActionGate(_fsm);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                _fsm.CreateDebugGUI(3);
#endif

                #region Variables

                var none = new NoneState<States>();
                var super = new SuperState<States>();
                var gold = new GoldState<States>();
                var automatic = new AutomaticState<States>();
                var slow = new SlowState<States>();
            
                temp.Add(none);
                temp.Add(super);
                temp.Add(gold);
                temp.Add(automatic);
                temp.Add(slow);

                #endregion

                #region Transitions

            
                none.AddTransition(States.Super, super);
                none.AddTransition(States.Gold, gold);
                none.AddTransition(States.Automatic, automatic);
                none.AddTransition(States.Slow, slow);
                
                super.AddTransition(States.None, none);
                gold.AddTransition(States.None, none);
                automatic.AddTransition(States.None, none);
                slow.AddTransition(States.None, none);

                #endregion

                foreach (var state in temp)
                {
                    state.Initialize(controller);
                }
            
                _fsm.SetInit(none); 
            }
            
            private void SetTransitions(States state)
            {
                _fsm.Transitions(state);
            }

            #region Transitions

            public void TransitionToNone()
            {
                SetTransitions(States.None);
            }

            public void TransitionToSuper()
            {
                SetTransitions(States.Super);
            }
        
            public void TransitionToGold()
            {
                SetTransitions(States.Gold);
            }
        
            public void TransitionToAutomatic()
            {
                SetTransitions(States.Automatic);
            }
        
            public void TransitionToSlow()
            {
                SetTransitions(States.Slow);
            }

            #endregion

            #region Public Getters

            public bool GetIsRotationDisabled() => _actionGate.IsRotationDisabled;

            #endregion
        }

        #endregion
        
        #region Main Controller
                
        private interface IMain
        {
            public void EnableShield();
            public void DisableShield();
        }
        
        private class MainController
        {
            #region States

            private class BaseState<T> : State<T>
            {
                public IMain Controller {get; private set;}
        
                public void Initialize(IMain controller)
                {
                    Controller = controller;
                }
            }
    
            private class EnableState<T> : BaseState<T>
            {
                // ReSharper disable Unity.PerformanceAnalysis
                public override void Awake() => Controller.EnableShield();
            }
        
            private class DisableState<T> : BaseState<T>
            {
                public override void Awake() => Controller.DisableShield();
            }
            
            private class AbilityState<T> : BaseState<T>
            {
                public override void Awake()
                {

                }

                public override void Sleep()
                {

                }
            }
        
            #endregion
            
            private enum States
            {
                None,
                Disable,
                Enable,
                Ability
            }
            
            private FSM<States> _fsm;

            private class ActionGate : FsmActionGate<States>
            {
                public bool IsMovementDisabled { get; private set; }
                public bool IsAbilityDisabled { get; private set; }
                public bool IsHitDisabled { get; private set; }
                public ActionGate(FSM<States> fsm) : base(fsm) { }

                protected override void OnEnterState(States state)
                {
                    IsMovementDisabled = state is States.Disable;
                    IsAbilityDisabled = state is States.Disable;
                    IsHitDisabled = state is States.Disable;
                }
            }

            private ActionGate _actionGate;

            public MainController(IMain controller)
            {
                InitializeFsm(controller);
            }
            
            private void InitializeFsm(IMain controller)
            {
                var temp = new List<BaseState<States>>();
                _fsm = new FSM<States>("Shield - Main");
                _actionGate = new ActionGate(_fsm);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                _fsm.CreateDebugGUI(2);
#endif

                #region Variables

                var disable = new DisableState<States>();
                var enable = new EnableState<States>();
                var ability = new EnableState<States>();
            
                temp.Add(disable);
                temp.Add(enable);
                temp.Add(ability);

                #endregion

                #region Transitions

            
                disable.AddTransition(States.Enable, enable);
            
                enable.AddTransition(States.Disable, disable);
                enable.AddTransition(States.Ability, ability);
                
                ability.AddTransition(States.Disable, enable);
                ability.AddTransition(States.Enable, enable);

                #endregion

                foreach (var state in temp)
                {
                    state.Initialize(controller);
                }
            
                _fsm.SetInit(disable);
            }
            
            
            private void SetTransitions(States state)
            {
                _fsm.Transitions(state);
            }

            #region Transitions
            
            public void TransitionToEnable()
            {
                SetTransitions(States.Enable);
            }

            public void TransitionToDisable()
            {
                SetTransitions(States.Disable);
            }

            public void TransitionToAbility()
            {
                SetTransitions(States.Ability);
            }
            
            #endregion

            #region Public Getters

            public bool GetIsMovementDisabled() => _actionGate.IsMovementDisabled;
            public bool GetIsAbilityDisabled() => _actionGate.IsAbilityDisabled;
            public bool GetIsHitDisable() => _actionGate.IsHitDisabled;

            #endregion
        }

        #endregion

        #endregion
        
        private class PendingData
        {
            public float? Direction;
        }

        private PendingData _pendingActionData;
        private AbilityController _ability;
        private MainController _mainController;
        private readonly ShieldMotor _motor;
        
        public ShieldController(ShieldMotor motor)
        {
            _motor = motor;
        }

        public void Rotate()
        {
            if (_pendingActionData.Direction != null) 
                _motor.Rotate(_pendingActionData.Direction.Value);
        }

        public void StopRotate()
        {
            _motor.StopRotate();
        }

        #region IShieldController
        
        public void Initialize()
        {
            _pendingActionData = new PendingData();
            _mainController = new MainController(this);
            _ability = new AbilityController(this);
        }

        public void Execute(float deltaTime)
        {
            
        }

        public void TryRotate(float direction)
        {
            _pendingActionData.Direction = direction;

            if (_mainController.GetIsMovementDisabled() 
                || _ability.GetIsRotationDisabled())
                return;
            
            Rotate();
        }

        public void TryStop()
        {
            if (_mainController.GetIsMovementDisabled() 
                || _ability.GetIsRotationDisabled())
                return;
            
            StopRotate();
        }

        public void TryEnable()
        {
            _mainController.TransitionToEnable();
        }
        
        public void TryDisable()
        {
            _mainController.TransitionToDisable();
            _ability.TransitionToNone();
        }
        
        public void TryEnableType(ShieldType abilityType)
        {
            if(_mainController.GetIsMovementDisabled())
                return;

            switch (abilityType)
            {
                case ShieldType.None:
                    break;
                case ShieldType.Super:
                    _ability.TransitionToSuper();
                    break;
                case ShieldType.Gold:
                    _ability.TransitionToGold();
                    break;
                case ShieldType.Automatic:
                    _ability.TransitionToAutomatic();
                    break;
                case ShieldType.Slow:
                    _ability.TransitionToSlow();
                    break;
            }
        }

        public void TryDisableShieldType()
        {
            _ability.TransitionToNone();
        }

        public void TryHit(Vector3 inputPosition, Quaternion inputRotation, Vector2 inputDirection)
        {
            if(_mainController.GetIsHitDisable()) return;
            
            _motor.HandleHit(inputPosition, inputRotation, inputDirection);
        }

        #endregion

        #region IAbility

        public void DisableAbility()
        {
            _motor.SetAbility(ShieldType.None);
        }

        public void EnableAutomatic()
        {
            _motor.SetAbility(ShieldType.Automatic);
        }

        public void EnableSuper()
        {
            _motor.SetAbility(ShieldType.Super);
        }

        public void EnableGold()
        {
            _motor.SetAbility(ShieldType.Gold);
        }

        public void EnableSlow()
        {
            _motor.SetAbility(ShieldType.Slow);
        }

        #endregion
        
        #region IMain

        public void EnableShield()
        {
            _motor.EnableShield();
        }

        public void DisableShield()
        {
            _motor.DisableShield();
        }

        #endregion
    }
}