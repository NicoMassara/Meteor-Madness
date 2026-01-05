using System;
using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.ShieldRotation.Contracts;
using _Main.Scripts.ShieldRotation.Tools;
using UnityEngine;

namespace _Main.Scripts.ShieldRotation.Movement
{
    public class MovementComponent : 
        IMovement,
        MovementComponent.IFsmMovement,
        MovementComponent.IMovementDebug
    {
        public interface IMovementDebug
        {
            public float AngularSpeed { get;}
            public string CurrentState { get; }
            public bool HasToCorrect { get; }
            public void StopMovement();
        }
        
        #region FSM
        
        private interface IFsmMovement
        {
            // === Data === //
            public float GetSpeedRatio();
            public float GetDirection();
            public bool GetIsMovingAtMaxSpeed();
            public float GetLastMovementSpeedRatio();
            public bool GetIsSnapFinished();
            public bool GetHasToCorrectSnap();
            public bool GetIsCorrectionSnapFinished();
            
            // === Setters === //

            public void SetIsStopping(bool isStopping);
            public void SetIsSnapping(bool isSnapping);
            public void StopCorrectionSnapping();
            
            // === Actions === //
            public void Stop();
            public void IncreaseSpeed(float deltaTime, float direction);
            public void ChangeDirection(float deltaTime, float getDirection);
            public void DecreaseSpeed(float deltaTime);
            public void SnapToSlot();
            public void SmoothSnapToSlot(float deltaTime);
            public void SaveLastMovementSpeedRatio();
            public void CalculateSnapAngle(bool shouldExtraSnap);
            public void HandleSnapCorrection(float deltaTime);
            
            // === Event Triggers === //
            
            public void TriggerOnStartMoving();
            public void TriggerOnDirectionChanged();
            public void TriggerOnStartStop();
            public void TriggerOnStopped();
            public void TriggerOnSnapCorrected();
            
            // === Transitions === //
            public void TransitionToStationary();
            public void TransitionToRotating();
            public void TransitionToChangingDirection();
            public void TransitionToStopping();
            public void TransitionSnapping();
            public void TransitionCorrectionSnapping();
        }
        
        #region States

        private enum States
        {
            None,
            Stationary,
            Rotating,
            ChangingDirection,
            Stopping,
            Snapping,
            CorrectionSnapping
        }
        
        
        private abstract class MovementStateBase : StateBase<IFsmMovement>
        {
            protected IFsmMovementData Data { get; private set; }

            public void Initialize(IFsmMovement controller, IFsmMovementData movementData)
            {
                InitializeState(controller);
                Data = movementData;
            }
        }
        
        private class StationaryState : MovementStateBase
        {
            public override void Execute(float deltaTime)
            {
                var hasInput = Controller.GetDirection() != 0;

                if (hasInput)
                {
                    Controller.TransitionToRotating();
                }
            }

            public override void Sleep()
            {
                Controller.TriggerOnStartMoving();
            }
        }
        private class RotatingState : MovementStateBase
        {
            private float _stopTimer;
            private float _lastDirection;

            public override void Awake()
            {
                ResetStopTimer();
                _lastDirection = Controller.GetDirection();
            }

            public override void Execute(float deltaTime)
            {
                var currentDirection = Controller.GetDirection();
                var hasInput = currentDirection != 0;
                var direction = hasInput ? currentDirection : _lastDirection;
                
                if(currentDirection != 0  && _lastDirection != direction)
                {
                    Controller.TransitionToChangingDirection();
                    return;
                }
                
                _lastDirection = direction;
                
                if (hasInput == false)
                {
                    _stopTimer -= deltaTime;
                    if (_stopTimer <= 0)
                    {
                        Controller.TransitionToStopping();
                        return;
                    }
                }
                else
                    ResetStopTimer();
                
                Controller.IncreaseSpeed(deltaTime, direction);
            }

            private void ResetStopTimer()
            {
                _stopTimer = Data.StopThreshold;
            }
        }
        private class ChangingDirectionState : MovementStateBase
        {
            private bool _hasChangedDirection;
            private float _direction;
            
            public override void Awake()
            {
                _hasChangedDirection = false;
                Controller.TriggerOnDirectionChanged();
                _direction = Controller.GetDirection();
            }

            public override void Execute(float deltaTime)
            {
                Controller.ChangeDirection(deltaTime, _direction);

                var speedRatio = Controller.GetSpeedRatio();
                
                if (speedRatio <= 0.09f && _hasChangedDirection == false)
                {
                    _hasChangedDirection = true;
                }
                
                if (_hasChangedDirection == false) return;

                if (speedRatio >= Data.StopChangeDirectionSpeedRatio)
                {
                    Controller.TransitionToRotating();
                }
            }
        }
        private class StoppingState : MovementStateBase
        {
            public override void Awake()
            {
                Controller.SaveLastMovementSpeedRatio();
                Controller.TriggerOnStartStop();
                Controller.SetIsStopping(true);
            }

            public override void Execute(float deltaTime)
            {
                var hasInput = Controller.GetDirection() != 0;

                if (hasInput)
                {
                    Controller.TransitionToRotating();
                    return;
                }
                
                Controller.DecreaseSpeed(deltaTime);

                if (Controller.GetSpeedRatio() > Data.MinSpeedRatioToSnap) return;

                if (Controller.GetHasToCorrectSnap())
                {
                    Debug.Log("Correcting Snap");
                    Controller.TransitionCorrectionSnapping();
                }
                else
                {
                    Controller.TransitionSnapping();
                }
            }

            public override void Sleep()
            {
                Controller.SetIsStopping(false);
            }
        }
        private class SnappingState : MovementStateBase
        {
            private bool _shouldInstaSnap;
            
            public override void Awake()
            {
                Controller.SetIsSnapping(true);
                _shouldInstaSnap = Controller.GetLastMovementSpeedRatio() < Data.MinSpeedRatioToSnap;
                Controller.CalculateSnapAngle(_shouldInstaSnap);
                
                if (_shouldInstaSnap)
                {
                    Controller.SnapToSlot();
                    Stop();
                }
            }

            public override void Execute(float deltaTime)
            {
                if (_shouldInstaSnap) return;
                
                Controller.SmoothSnapToSlot(deltaTime);

                if (Controller.GetIsSnapFinished())
                {
                    Stop();
                    return;
                }
                
                var hasInput = Controller.GetDirection() != 0;

                if (hasInput)
                {
                    Controller.TransitionToRotating();
                    return;
                }
                
                if (Controller.GetHasToCorrectSnap())
                {
                    Controller.TransitionCorrectionSnapping();
                    return;
                }
            }

            public override void Sleep()
            {
                Controller.SetIsSnapping(false);
            }

            private void Stop()
            {
                Controller.Stop();
                Controller.TriggerOnStopped();
                Controller.TransitionToStationary();
            }
        }

        private class CorrectionSnapping : MovementStateBase
        {
            public override void Awake()
            {
                Controller.SetIsSnapping(true);
            }

            public override void Execute(float deltaTime)
            {
                if (Controller.GetIsCorrectionSnapFinished())
                {
                    Controller.TransitionToStationary();
                    return;
                }
                
                Controller.HandleSnapCorrection(deltaTime);
            }

            public override void Sleep()
            {
                Controller.Stop();
                Controller.SetIsSnapping(false);
                Controller.TriggerOnStopped();
                Controller.TriggerOnSnapCorrected();
                Controller.StopCorrectionSnapping();
            }
        }

        #endregion

        #region Controller
        
        private class MovementFsm : FsmController<States>
        {
            public MovementFsm(IFsmMovement movement, IFsmMovementData movementData)
            {
                Initialize(movement, movementData);
            }

            private void Initialize(IFsmMovement movement, IFsmMovementData movementData)
            {
                var tempList = new List<StateData>
                {
                    new (States.Stationary, new StationaryState()),
                    new (States.Rotating, new RotatingState()),
                    new (States.ChangingDirection, new ChangingDirectionState()),
                    new (States.Stopping, new StoppingState()),
                    new (States.Snapping, new SnappingState()),
                    new (States.CorrectionSnapping, new CorrectionSnapping())
                };

                foreach (var state in tempList.Select(item => (MovementStateBase)item.State))
                {
                    state.Initialize(movement, movementData);
                }
                
                InitializeStates(tempList);
            }
        }
        
        #endregion
        
        #endregion
        
        private readonly MovementFsm _controller;
        private readonly IMovementData _data;
        private readonly Transform _objectToRotate;
        private readonly int _angleSlots;
        private const float MinDeltaSnapAngle = 1f;
        private float _movementStopDelayTimer;
        private float _direction;
        private float _lastDirection;
        private float _angularSpeed;
        private float _lastMovementSpeedRatio; // Is the last speed ratio before going to 'Stopping' state
        private float _angleToSnap;
        private float _targetCorrectionAngle;
        private bool _hasToCorrectSnap;
        

        public float AngularSpeed => Mathf.Abs(_angularSpeed);
        public float SpeedRatio => AngularSpeed / _data.MaxSpeed;
        public string CurrentState => _controller.GetCurrentState();
        public bool HasToCorrect => _hasToCorrectSnap;
        public bool IsSnapping { get; private set; }
        public bool IsStopping { get; private set; }

        public Vector2 Position => _objectToRotate.position;
        public event Action<int> OnStartMoving;
        public event Action OnStartStop;
        public event Action OnStopped;
        public event Action<int> OnDirectionChange;
        public event Action OnSnapCorrected;
        
        
        public MovementComponent(IMovementData data, Transform objectToRotate, int angleSlots)
        {
            _data = data;
            _objectToRotate = objectToRotate;
            _angleSlots = angleSlots;

            _controller = new MovementFsm(this, (IFsmMovementData)_data);
            TransitionToStationary();
        }
        
        private void RotateObject(float deltaTime)
        {
            _objectToRotate.Rotate(0f, 0f, _angularSpeed * deltaTime);
        }

        private void ClampAngularSpeed()
        {
            _angularSpeed = Mathf.Clamp(_angularSpeed, -_data.MaxSpeed, _data.MaxSpeed);
        }

        #region IMovement

        public void SetDirection(float direction)
        {
            _lastDirection = _direction;
            _direction = direction;
        }
        
        public void Update(float deltaTime)
        {
            _controller?.Execute(deltaTime);
            
            ClampAngularSpeed();
            RotateObject(deltaTime);
        }
        
        public void SetCorrectionData(int targetSlot)
        {
            _hasToCorrectSnap = true;
            _targetCorrectionAngle = GetAngleFromSlot(targetSlot);
        }

        public void ClearCorrectionData()
        {
            _hasToCorrectSnap = false;
        }

        public int GetCurrentSlot() => Mathf.RoundToInt(GetCurrentAngle() / GetSlotSize());

        #endregion

        #region Angle Slots
        
        private float GetCurrentAngle() => _objectToRotate.localEulerAngles.z;
        private float GetSlotSize() => 360f / _angleSlots;

        private float GetAngleFromSlot(int slot)
        {
            var angle = slot * GetSlotSize();
            return (angle + 360f) % 360f;
        }

        #endregion

        #region IFSMMovement
        
        // === Data === //

        #region Data
        
        public float GetSpeedRatio() => SpeedRatio;
        public float GetDirection() => _direction;
        public bool GetIsMovingAtMaxSpeed() => SpeedRatio >= 1;

        public bool GetHasLeftInput() => _movementStopDelayTimer > 0;
        public float GetLastMovementSpeedRatio() => _lastMovementSpeedRatio;
        
        public bool GetIsSnapFinished() 
            => Mathf.Abs(Mathf.DeltaAngle(GetCurrentAngle(), _angleToSnap)) < MinDeltaSnapAngle;
        public bool GetIsCorrectionSnapFinished() 
            => Mathf.Abs(Mathf.DeltaAngle(GetCurrentAngle(), _targetCorrectionAngle)) < MinDeltaSnapAngle;
        
        public bool GetHasToCorrectSnap() => _hasToCorrectSnap;

        #endregion
        
        // === Actions === //

        #region Actions
        
        public void Stop() => _angularSpeed = 0;

        public void ChangeDirection(float deltaTime, float direction)
        {
            _angularSpeed += (_data.ChangeDirectionAcceleration * deltaTime) * direction;
        }

        public void IncreaseSpeed(float deltaTime, float direction)
        {
            _angularSpeed += (_data.Acceleration * deltaTime) * direction;
        }

        public void DecreaseSpeed(float deltaTime)
        {
            _angularSpeed = Mathf.MoveTowards(_angularSpeed, 0, _data.Deceleration * deltaTime);
        }
        
        public void SnapToSlot()
        {
            _objectToRotate.rotation = Quaternion.Euler(0f,0f,_angleToSnap);
        }

        public void SmoothSnapToSlot(float deltaTime)
        {
            var finalSnapSpeed = _data.SnapSpeed * deltaTime;
                    
            _objectToRotate.rotation = Quaternion.Lerp( 
                _objectToRotate.rotation, 
                Quaternion.Euler(0f,0f, _angleToSnap), 
                finalSnapSpeed);
            
            if (Math.Abs(Mathf.DeltaAngle(GetCurrentAngle(), _angleToSnap)) < MinDeltaSnapAngle)
            {
                _objectToRotate.rotation = Quaternion.Euler(0f,0f,_angleToSnap);
            }
        }

        public void SaveLastMovementSpeedRatio() 
            => _lastMovementSpeedRatio = SpeedRatio;

        public void CalculateSnapAngle(bool shouldExtraSnap)
        {
            int currentSlot = GetCurrentSlot();

            if (shouldExtraSnap)
            {
                Debug.Log("Not Enough Speed, snapping to next slot");
                currentSlot += (int)Mathf.Sign(_lastDirection);
            }

            _angleToSnap = GetAngleFromSlot(currentSlot);
        }

        public void HandleSnapCorrection(float deltaTime)
        {
            var finalSnapSpeed = _data.CorrectionSnapSpeed * deltaTime;
                    
            _objectToRotate.rotation = Quaternion.Lerp( 
                _objectToRotate.rotation, 
                Quaternion.Euler(0f,0f, _targetCorrectionAngle), 
                finalSnapSpeed);
            
            if (Math.Abs(Mathf.DeltaAngle(GetCurrentAngle(), _targetCorrectionAngle)) < MinDeltaSnapAngle)
            {
                _objectToRotate.rotation = Quaternion.Euler(0f,0f,_targetCorrectionAngle);
            }
        }
        

        #endregion
        
        // === Action Triggers === //

        #region Action Triggers
        
        public void TriggerOnStartMoving() 
            => OnStartMoving?.Invoke((int)Mathf.Sign(_direction));

        public void TriggerOnDirectionChanged() 
            => OnDirectionChange?.Invoke((int)Mathf.Sign(_direction));

        public void TriggerOnStopped() => OnStopped?.Invoke();
        public void TriggerOnStartStop() => OnStartStop?.Invoke();
        public void TriggerOnSnapCorrected() => OnSnapCorrected?.Invoke();
        
        #endregion
        
        // === Setters === //

        #region Setters
        public void SetIsStopping(bool isStopping) => IsStopping = isStopping;
        public void SetIsSnapping(bool isSnapping) => IsSnapping = isSnapping;
        public void StopCorrectionSnapping()
        {
            _hasToCorrectSnap = false;
            _targetCorrectionAngle = -1;
        }

        #endregion
        
        // === Transitions === //
        
        #region Transitions
        
        public void TransitionToStationary() => _controller.Transition(States.Stationary);
        public void TransitionToRotating() => _controller.Transition(States.Rotating);
        public void TransitionToChangingDirection() => _controller.Transition(States.ChangingDirection);
        public void TransitionToStopping() => _controller.Transition(States.Stopping);
        public void TransitionSnapping() => _controller.Transition(States.Snapping);
        public void TransitionCorrectionSnapping() => _controller.Transition(States.CorrectionSnapping);
        
        #endregion

        #endregion

        #region Debug

        public void StopMovement()
        {
            Stop();
            TransitionToStationary();
        }

        #endregion
    }
}