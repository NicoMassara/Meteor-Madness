using System.Collections.Generic;
using _Main.Scripts.Cosmetics.MVC;
using _Main.Scripts.DebugGUI;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Interfaces;

namespace _Main.Scripts.Gameplay.MyCamera
{
    public class CameraController
    {
        #region States

        private class StateBase<T> : State<T>
        {
            protected CameraController Controller { get; private set; }

            public void Initialize(CameraController controller)
            {
                Controller = controller;
            }
        }
    
        private class IdleState<T> : StateBase<T> {}
        private class ZoomingState<T> : StateBase<T> {}
        private class LookingState<T> : StateBase<T> {}
        private class ShakingState<T> : StateBase<T> {}

        #endregion
        
        private FSM<States> _fsm;
        private readonly CameraMotor _motor;

        private enum States
        {
            None,
            Idle,
            Zooming,
            Looking,
            Shaking
        }

        private class ActionGate : FsmActionGate<States>
        {
            public bool IsIdle { get; private set; }
            
            public ActionGate(FSM<States> fsm) : base(fsm) { }

            protected override void OnNewState(States state) { }
            protected override void OnExitState(States state) { }

            protected override void OnEnterState(States state)
            {
                IsIdle = state == States.Idle;
            }
        }
        private ActionGate _actionGate;
        
        public CameraController(CameraMotor motor)
        {
            _motor = motor;
        }

        public void Initialize()
        {
            InitializeFsm();
        }

        #region FSM

        private void InitializeFsm()
        {
            var temp = new List<StateBase<States>>();
            _fsm = new FSM<States>("Camera");
            _actionGate = new ActionGate(_fsm);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _fsm.CreateDebugGUI(DebugGUISortingOrder.SubGroup.Camera);
#endif

            #region Variables

            var none = new BaseState<States>();
            var idle = new IdleState<States>();
            var zoom = new ZoomingState<States>();
            var look = new LookingState<States>();
            var shake = new ShakingState<States>();
            
            temp.Add(idle);
            temp.Add(zoom);
            temp.Add(look);
            temp.Add(shake);

            #endregion

            #region Transitions

            none.AddTransition(States.Idle, idle);
            
            idle.AddTransition(States.Zooming, zoom);
            idle.AddTransition(States.Looking, look);
            idle.AddTransition(States.Shaking, shake);
            
            zoom.AddTransition(States.Idle, idle);
            
            look.AddTransition(States.Idle, idle);
            
            shake.AddTransition(States.Idle, idle);

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
        
        public void TransitionToIdle()
        {
            SetTransition(States.Idle);
        }
        
        public void TransitionToZoom()
        {
            SetTransition(States.Zooming);
        }
        
        public void TransitionToShaking()
        {
            SetTransition(States.Shaking);
        }
        
        public void TransitionToLooking()
        {
            SetTransition(States.Looking);
        }

        #endregion
        
        #endregion
        
        public void Zoom(CameraZoomPosition zoomPosition, float timeToZoom)
        {
            if(_actionGate.IsIdle == false) return;
            if(_motor.IsUnableToZoom(zoomPosition)) return;
            
            
            TransitionToZoom();
            _motor.Zoom(zoomPosition, timeToZoom);
        }

        public void Look(CameraLookPosition position, float timeToLook)
        {
            if(_actionGate.IsIdle == false) return;
            if(_motor.IsUnableToLook(position)) return;
            
            TransitionToLooking();
            _motor.Look(position, timeToLook);
        }

        public void Shake(IShakeData shakeData)
        {
            if(_actionGate.IsIdle == false) return;
            
            //
            TransitionToShaking();
            _motor.Shake(shakeData);
        }

    }
}