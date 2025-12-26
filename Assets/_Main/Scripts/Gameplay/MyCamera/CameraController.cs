using System.Collections.Generic;
using MeteorMadness.GlobalValues.Interfaces;
using MeteorMadness.Systems;
using UnityEngine;

namespace _Main.Scripts.MyCamera
{
    public class CameraController : 
        CameraController.ICameraController,
        CameraController.IMainCameraController,
        CameraController.ICameraZoom, 
        CameraController.ICameraLook
    {
        #region Interfaces

        public interface ICameraController
        {
            public void Initialize();
            public void TransitionToIdle();
            public void TryShake(IShakeData shakeData);
            public void TryZoomIn(float timeToZoom);
            public void TryZoomOut(float timeToZoom);
            public void TryLookCenter(float timeToLook);
            public void TryLookRight(float timeToLook);
            public void TryLookLeft(float timeToLook);
            public void TryLookAtTop(float timeToLook);
            public void TryLookAtBottom(float timeToLook);
            public void EnableGrayscale();
            public void DisableGrayscale();
        }

        private interface IMainCameraController
        {
            public void ExecuteShake();
        }

        private interface ICameraZoom
        {
            public void ExecuteZoomIn();

            public void ExecuteZoomOut();
        }

        private interface ICameraLook
        {
            public void LookCenter();
            public void LookRight();
            public void LookLeft();
            public void LookAtTop();
            public void LookAtBottom();

        }

        #endregion
        
        #region Private Classes
        
        // Tracks which action is performing
        private class MainController
        {
            #region States

            private class StateBase<T> : State<T>
            {
                protected IMainCameraController Controller { get; private set; }

                public void Initialize(IMainCameraController controller)
                {
                    Controller = controller;
                }
            }
    
            private class IdleState<T> : StateBase<T> {}
            private class ZoomingState<T> : StateBase<T> {}
            private class LookingState<T> : StateBase<T> {}
            private class ShakingState<T> : StateBase<T>
            {
                public override void Awake() => Controller.ExecuteShake();
            }

            #endregion
            
            private enum States
            {
                None,
                Idle,
                Zooming,
                Looking,
                Shaking
            }
            
            private FSM<States> _fsm;

            #region FSM

            public MainController(CameraController controller)
            {
                InitializeFsm(controller);
            }

            private void InitializeFsm(CameraController controller)
            {
                var temp = new List<StateBase<States>>();
                _fsm = new FSM<States>("Camera - Main");

                #region Variables

                var none = new StateBase<States>();
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
                zoom.AddTransition(States.Looking, look);
                zoom.AddTransition(States.Shaking, shake);
            
                look.AddTransition(States.Idle, idle);
                look.AddTransition(States.Zooming, zoom);
                look.AddTransition(States.Shaking, shake);
            
                shake.AddTransition(States.Idle, idle);
                shake.AddTransition(States.Looking, look);
                shake.AddTransition(States.Zooming, zoom);

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
        }

        // Tacks the current Zoom, to prevent a double zoom
        private class ZoomController
        {
            #region States

            private class StateBase<T> : State<T>
            {
                protected ICameraZoom Controller { get; private set; }

                public void Initialize(ICameraZoom controller)
                {
                    Controller = controller;
                }
            }

            private class ZoomInState<T> : StateBase<T>
            {
                public override void Awake() => Controller.ExecuteZoomIn();
            }

            private class ZoomOutState<T> : StateBase<T>
            {
                public override void Awake() => Controller.ExecuteZoomOut();
            }

            #endregion
            
            private class ActionGate : FsmActionGate<States>
            {
                public bool IsZoomedIn { get; private set; }
                public bool IsZoomedOut { get; private set; }
                
                public ActionGate(FSM<States> fsm) : base(fsm) { }

                protected override void OnEnterState(States state)
                {
                    IsZoomedIn = state is States.ZoomIn;
                    IsZoomedOut = state is States.ZoomOut;
                }
            }
            
            private ActionGate _actionGate;
            
            private enum States
            {
                None,
                ZoomIn,
                ZoomOut,
            }
            
            private FSM<States> _fsm;

            #region FSM

            public ZoomController(CameraController controller)
            {
                InitializeFsm(controller);
            }

            private void InitializeFsm(CameraController controller)
            {
                var temp = new List<StateBase<States>>();
                _fsm = new FSM<States>("Camera - Zoom");
                _actionGate = new ActionGate(_fsm);

                #region Variables

                var none = new StateBase<States>();
                var zoomIn = new ZoomInState<States>();
                var zoomOut = new ZoomOutState<States>();
            
                temp.Add(none);
                temp.Add(zoomIn);
                temp.Add(zoomOut);

                #endregion

                #region Transitions

                none.AddTransition(States.ZoomOut, zoomOut);
                none.AddTransition(States.ZoomIn, zoomIn);
                
                zoomIn.AddTransition(States.ZoomOut, zoomOut);
                
                zoomOut.AddTransition(States.ZoomIn, zoomIn);
                

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
        
            public void TransitionToZoomIn()
            {
                if (_actionGate.IsZoomedIn)
                {
                    //Debug.Log("Can't Zoom In");
                    return;
                }

                SetTransition(States.ZoomIn);
            }
        
            public void TransitionToZoomOut()
            {
                if (_actionGate.IsZoomedOut)
                {
                    Debug.Log("Can't Zoom Out");
                    return;
                }
                
                SetTransition(States.ZoomOut);
            }
            
         
            #endregion
            
            #endregion
        }

        // Tacks the current Look Position, to prevent a double zoom
        private class LookController
        {
            #region States

            private class StateBase<T> : State<T>
            {
                protected ICameraLook Controller { get; private set; }

                public void Initialize(ICameraLook controller)
                {
                    Controller = controller;
                }
            }

            private class CenterState<T> : StateBase<T>
            {
                public override void Awake() => Controller.LookCenter();
            }
            private class RightState<T> : StateBase<T>
            {
                public override void Awake() => Controller.LookRight();
            }
            
            private class LeftState<T> : StateBase<T>
            {
                public override void Awake() => Controller.LookLeft();
            }
            
            private class TopState<T> : StateBase<T>
            {
                public override void Awake() => Controller.LookAtTop();
            }
            
            private class BottomState<T> : StateBase<T>
            {
                public override void Awake() => Controller.LookAtBottom();
            }

            #endregion

            private class ActionGate : FsmActionGate<States>
            {
                public bool IsInCenter { get; private set; }
                public bool IsLookingRight { get; private set; }
                public bool IsLookingLeft { get; private set; }
                public bool IsLookingUp { get; private set; }
                public bool IsLookingDown { get; private set; }

                public ActionGate(FSM<States> fsm) : base(fsm) { }

                protected override void OnEnterState(States state)
                {
                    IsInCenter = state is States.Center;
                    IsLookingRight = state is States.Right;
                    IsLookingLeft = state is States.Left;
                    IsLookingUp = state is States.Top;
                    IsLookingDown = state is States.Bottom;
                }
            }
            
            private ActionGate _actionGate;

            private enum States
            {
                None,
                Center,
                Right,
                Left,
                Top,
                Bottom,
            }
            
            private FSM<States> _fsm;

            #region FSM

            public LookController(CameraController controller)
            {
                InitializeFsm(controller);
            }

            private void InitializeFsm(CameraController controller)
            {
                var temp = new List<StateBase<States>>();
                _fsm = new FSM<States>("Camera - Look");
                _actionGate = new ActionGate(_fsm);

                #region Variables

                var none = new StateBase<States>();
                var center = new CenterState<States>();
                var right = new RightState<States>();
                var left = new LeftState<States>();
                var top = new TopState<States>();
                var bottom = new BottomState<States>();
            
                temp.Add(none);
                temp.Add(center);
                temp.Add(right);
                temp.Add(left);
                temp.Add(top);
                temp.Add(bottom);

                #endregion

                #region Transitions

                none.AddTransition(States.Center, center);
                none.AddTransition(States.Right, right);
                none.AddTransition(States.Left, left);
                none.AddTransition(States.Top, top);
                none.AddTransition(States.Bottom, bottom);
                
                center.AddTransition(States.Right, right);
                center.AddTransition(States.Left, left);
                center.AddTransition(States.Top, top);
                center.AddTransition(States.Bottom, bottom);
                
                right.AddTransition(States.Center, center);
                right.AddTransition(States.Left, left);
                right.AddTransition(States.Top, top);
                right.AddTransition(States.Bottom, bottom);
                
                left.AddTransition(States.Center, center);
                left.AddTransition(States.Right, right);
                left.AddTransition(States.Top, top);
                left.AddTransition(States.Bottom, bottom);
                
                top.AddTransition(States.Center, center);
                top.AddTransition(States.Right, right);
                top.AddTransition(States.Left, left);
                top.AddTransition(States.Bottom, bottom);
                
                bottom.AddTransition(States.Center, center);
                bottom.AddTransition(States.Right, right);
                bottom.AddTransition(States.Left, left);
                bottom.AddTransition(States.Top, top);
                
                
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
        
            public void TransitionToCenter()
            {
                if(_actionGate.IsInCenter) return;
                
                SetTransition(States.Center);
            }
        
            public void TransitionToRight()
            {
                if(_actionGate.IsLookingRight) return;
                
                SetTransition(States.Right);
            }
        
            public void TransitionToLeft()
            {
                if(_actionGate.IsLookingLeft) return;
                
                SetTransition(States.Left);
            }
        
            public void TransitionToTop()
            {
                if(_actionGate.IsLookingUp) return;
                
                SetTransition(States.Top);
            }
            
            public void TransitionToBottom()
            {
                if(_actionGate.IsLookingDown) return;
                
                SetTransition(States.Bottom);
            }
            
         
            #endregion
            
            #endregion
        }

        #endregion
        
        private MainController _mainController;
        private ZoomController _zoomController;
        private LookController _lookController;
        private readonly CameraMotor _motor;
        
        public CameraController(CameraMotor motor)
        {
            _motor = motor;
        }

        public void Initialize()
        {
            _mainController = new MainController(this);
            _zoomController = new ZoomController(this);
            _lookController = new LookController(this);
        }

        public void TransitionToIdle()
        {
            _mainController.TransitionToIdle();
        }

        #region Motor Try

        public void TryShake(IShakeData shakeData)
        {
            _motor.TryShake(shakeData);
            _mainController.TransitionToShaking();
        }
        
        #region Zoom

        public void TryZoomIn(float timeToZoom)
        {
            _motor.TryZoom(timeToZoom);
            _zoomController.TransitionToZoomIn();
        }
        
        public void TryZoomOut(float timeToZoom)
        {
            _motor.TryZoom(timeToZoom);
            _zoomController.TransitionToZoomOut();
        }

        #endregion
        
        #region Look

        public void TryLookCenter(float timeToLook)
        {
            _motor.TryLook(timeToLook);
            _lookController.TransitionToCenter();
        }
        
        public void TryLookRight(float timeToLook)
        {
            _motor.TryLook(timeToLook);
            _lookController.TransitionToRight();
        }
        
        public void TryLookLeft(float timeToLook)
        {
            _motor.TryLook(timeToLook);
            _lookController.TransitionToLeft();
        }
        
        public void TryLookAtTop(float timeToLook)
        {
            _motor.TryLook(timeToLook);
            _lookController.TransitionToTop();
        }
        
        public void TryLookAtBottom(float timeToLook)
        {
            _motor.TryLook(timeToLook);
            _lookController.TransitionToBottom();
        }

        public void EnableGrayscale() => _motor.EnableGrayscale();

        public void DisableGrayscale() => _motor.DisableGrayscale();

        #endregion

        #endregion

        #region Motor Execute

        #region Shake

        public void ExecuteShake()
        {
            _motor.ExecuteShake();
        }

        #endregion
        
        #region Zoom

        public void ExecuteZoomIn()
        {
            _mainController.TransitionToZoom();
            _motor.ExecuteZoomIn();
        }
        
        public void ExecuteZoomOut()
        {
            _mainController.TransitionToZoom();
            _motor.ExecuteZoomOut();
        }

        #endregion     
        
        #region Look

        public void LookCenter()
        {
            _mainController.TransitionToLooking();
            _motor.LookCenter();
        }

        public void LookRight()
        {
            _mainController.TransitionToLooking();
            _motor.LookRight();
        }
        
        public void LookLeft()
        {
            _mainController.TransitionToLooking();
            _motor.LookLeft();
        }
        
        public void LookAtTop()
        {
            _mainController.TransitionToLooking();
            _motor.LookAtTop();
        }
        
        public void LookAtBottom()
        {
            _mainController.TransitionToLooking();
            _motor.LookAtBottom();
        }

        #endregion     

        #endregion
    }
}