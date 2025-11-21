using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay.Shield.States;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldController
    {
        private readonly ShieldMotor _motor;
        
        private class ShieldActionGate : FsmActionGate<States>
        {
            public ShieldActionGate(FSM<States> fsm) : base(fsm) { }

            public bool RotationEnable { get; private set; }


            protected override void OnNewState(States state)
            {

            }

            protected override void OnEnterState(States state)
            {
                RotationEnable = state is States.Enable or States.Gold or States.Slow;
            }

            protected override void OnExitState(States state)
            {

            }
        }

        private enum States
        {
            None,
            Disable,
            Enable,
            Super,
            Gold,
            Automatic,
            Slow
        }

        private FSM<States> _fsm;

        private ShieldActionGate _actionGate;

        public ShieldController(ShieldMotor motor)
        {
            _motor = motor;
        }

        public void Initialize()
        {
            InitializeFsm();
        }

        public void Execute(float deltaTime)
        {
            _fsm.Execute(deltaTime);

            
            //Tester
            if (Input.GetKeyDown(KeyCode.R))
            {
                TransitionToSuper();
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                TransitionToEnable();
            }
        }

        #region FSM

        private void InitializeFsm()
        {
            var temp = new List<ShieldBaseState<States>>();
            _fsm = new FSM<States>("Shield");
            _actionGate = new ShieldActionGate(_fsm);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _fsm.CreateDebugGUI(2);
#endif

            #region Variables

            var disable = new DisableState<States>();
            var enable = new EnableState<States>();
            var super = new SuperState<States>();
            var gold = new GoldState<States>();
            var automatic = new AutomaticState<States>();
            var slow = new SlowState<States>();
            
            temp.Add(disable);
            temp.Add(enable);
            temp.Add(super);
            temp.Add(gold);
            temp.Add(automatic);
            temp.Add(slow);

            #endregion

            #region Transitions

            
            disable.AddTransition(States.Enable, enable);
            
            enable.AddTransition(States.Disable, disable);
            enable.AddTransition(States.Super, super);
            enable.AddTransition(States.Gold, gold);
            enable.AddTransition(States.Automatic, automatic);
            enable.AddTransition(States.Slow, slow);
            
            super.AddTransition(States.Enable, enable);
            
            gold.AddTransition(States.Enable, enable);
            gold.AddTransition(States.Disable, disable);
            
            slow.AddTransition(States.Enable, enable);
            slow.AddTransition(States.Disable, disable);
            
            automatic.AddTransition(States.Enable, enable);

            #endregion

            foreach (var state in temp)
            {
                state.Initialize(this);
            }
            
            _fsm.SetInit(disable);
        }

        private void SetTransitions(States state)
        {
            _fsm.Transitions(state);
        }

        public void TransitionToEnable()
        {
            SetTransitions(States.Enable);
        }

        public void TransitionToDisable()
        {
            SetTransitions(States.Disable);
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

        #region Motor

        #region Movement

        public void TryRotate(float direction)
        {
            if (_actionGate.RotationEnable)
            {
                _motor.Rotate(direction);
            }
        }

        public void SetActiveSuperShield(bool isActive)
        {
            _motor.SetActiveSuperShield(isActive);
        }

        public void TryStopRotate()
        {
            if (_actionGate.RotationEnable)
            {
                _motor.StopRotate();
            }
        }

        public void ForceRotate(float direction)
        {
            _motor.ForceRotate(direction);
        }
        
        public void RestartPosition()
        {
            _motor.RestartPosition();
        }
        
        #endregion

        #region Sprites

        public void SetActiveShield(bool isActive)
        {
            _motor.SetActiveShield(isActive);
        }
        
        public void SetActiveGold(bool isActive)
        {
            _motor.SetActiveGold(isActive);
        }

        #endregion
        
        public void HandleHit(Vector3 position, Quaternion rotation, Vector2 direction)
        {
            _motor.HandleHit(position,rotation,direction);
        }

        public void SetActiveAutomatic(bool isActive)
        {
            _motor.SetActiveAutomatic(isActive);
        }
        
        public void SetActiveSlow(bool isActive)
        {
            _motor.SetActiveSlow(isActive);
        }

        #endregion

        #region Handlers
        

        #endregion


    }

    #region States

    public class EnableState<T> : ShieldBaseState<T>
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public override void Awake()
        {
            Controller.SetActiveShield(true);
        }
    }
    
    public class AutomaticState<T> : ShieldBaseState<T>
    {
        public override void Awake()
        {
            Controller.SetActiveAutomatic(true);
        }

        public override void Sleep()
        {
            Controller.SetActiveAutomatic(false);
        }
    }
    
    public class GoldState<T> : ShieldBaseState<T>
    {
        public override void Awake()
        {
            Controller.SetActiveGold(true);
        }

        public override void Sleep()
        {
            Controller.SetActiveGold(false);
        }
    }
    
    public class SuperState<T> : ShieldBaseState<T>
    {
        private const float MovementDirection = 1f;
        
        public override void Awake()
        {
            Controller.SetActiveSuperShield(true);
        }

        public override void Execute(float deltaTime)
        {
            Controller.ForceRotate(MovementDirection);
        }

        public override void Sleep()
        {
            Controller.SetActiveSuperShield(false);
        }
    }
    
    public class DisableState<T> : ShieldBaseState<T>
    {
        public override void Awake()
        {
            Controller.SetActiveShield(false);
        }
    }
    
    public class SlowState<T> : ShieldBaseState<T>
    {
        public override void Awake()
        {
            Controller.SetActiveSlow(true);
        }

        public override void Sleep()
        {
            Controller.SetActiveSlow(false);
        }
    }

    #endregion
}