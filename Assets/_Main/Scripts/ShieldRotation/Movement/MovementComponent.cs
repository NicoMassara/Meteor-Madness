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
        MovementComponent.IFsmMovement
    {
        #region FSM

        #region Interfaces
        private interface IFsmMovement
        {
            // === Data === //
            public float GetSpeedRatio();
            public float GetDirection();
            public bool GetIsSnapFinished();
            public bool GetHasToCorrectSnap();
            public bool GetIsCorrectionSnapFinished();
            
            // === Setters === //
            
            public void SetIsSnapping(bool isSnapping);
            public void StopCorrectionSnapping();
            public void ClearTravelledSlots();
            
            // === Actions === //
            public void Stop();
            public void IncreaseSpeed(float deltaTime, float direction);
            public void ChangeDirection(float deltaTime, float getDirection);
            public void SnapToSlot();
            public void SmoothSnapToSlot(float deltaTime);
            public void SaveLastMovementSpeedRatio();
            public void CalculateSnapAngle();
            public void HandleSnapCorrection(float deltaTime);
            
            // === Event Triggers === //
            
            public void TriggerOnStartMoving();
            public void TriggerOnDirectionChanged();
            public void TriggerOnStartStop();
            public void TriggerOnStopped();
            public void TriggerOnSnapCorrected();
            public void TriggerOnCheckForCorrection();
            
            // === Transitions === //
            public void TransitionToStationary();
            public void TransitionToRotating();
            public void TransitionToChangingDirection();
            public void TransitionSnapping();
            public void TransitionCorrectionSnapping();
        }
        #endregion
        
        #region States

        private enum States
        {
            None,
            Stationary,
            Rotating,
            ChangingDirection,
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
            public override void Awake()
            {
                Controller.ClearTravelledSlots();
            }

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
                        Controller.SaveLastMovementSpeedRatio();
                        Controller.Stop();
                        Controller.TransitionSnapping();
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
            private float _timeOutTimer;
            private float _direction;
            private bool _hasChangedDirection;
            private bool _shouldTransitionToRotation;
            
            public override void Awake()
            {
                RestartTimer();
                _shouldTransitionToRotation = false;
                _hasChangedDirection = false;
                _direction = Controller.GetDirection();
                Controller.TriggerOnDirectionChanged();
            }

            public override void Execute(float deltaTime)
            {
                ChangeDirection(deltaTime);
                RunTimeOutTimer(deltaTime);

                if (_shouldTransitionToRotation)
                {
                    Controller.TransitionToRotating();
                }
            }

            private void ChangeDirection(float deltaTime)
            {
                Controller.ChangeDirection(deltaTime, _direction);

                var speedRatio = Controller.GetSpeedRatio();
                
                if (speedRatio <= Data.ChangeDirectionSpeedRatio && _hasChangedDirection == false)
                {
                    _hasChangedDirection = true;
                }
                
                if (_hasChangedDirection == false) return;

                if (speedRatio >= Data.StopChangeDirectionSpeedRatio)
                {
                    _shouldTransitionToRotation = true;
                }
            }

            private void RunTimeOutTimer(float deltaTime)
            {
                _timeOutTimer -= deltaTime;
                if(_timeOutTimer > 0) return;

                _shouldTransitionToRotation = true;
            }

            private void RestartTimer()
            {
                _timeOutTimer = Data.ChangeDirectionTimeOut + Data.StopChangeDirectionTimeOut;
                _timeOutTimer *= Data.ChangeDirectionTimeOutThreshold;
            }
        }
        private class SnappingState : MovementStateBase
        {
            public override void Awake()
            {
                Controller.CalculateSnapAngle();
                Controller.TriggerOnStartStop();
                Controller.SetIsSnapping(true);
                
                Controller.TriggerOnCheckForCorrection();
            }

            public override void Execute(float deltaTime)
            {
                Controller.TriggerOnCheckForCorrection();
                
                Controller.SmoothSnapToSlot(deltaTime);

                if (Controller.GetIsSnapFinished())
                {
                    FinishSnapping();
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

            private void FinishSnapping()
            {
                Controller.SnapToSlot();
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
                if (Controller.GetHasToCorrectSnap() == false)
                {
                    Controller.TransitionSnapping();
                    return;
                }

                if (Controller.GetIsCorrectionSnapFinished())
                {
                    FinishSnap();
                    return;
                }

                
                Controller.HandleSnapCorrection(deltaTime);
            }

            private void FinishSnap()
            {
                Controller.Stop();
                Controller.TransitionToStationary();
                Controller.TriggerOnStopped();
                Controller.TriggerOnSnapCorrected();
            }

            public override void Sleep()
            {
                Controller.SetIsSnapping(false);
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
        private const float MinDeltaSnapDistance = 2f;
        private float _movementStopDelayTimer;
        private float _direction;
        private float _lastDirection;
        private float _angularSpeed;
        private float _lastMovementSpeedRatio; // Is the last speed ratio before going to 'Stopping' state
        private float _angleToSnap;
        private float _targetCorrectionAngle;
        private float _lastTravelledSlotAngle;
        private bool _hasToCorrectSnap;
        private int _travelledSlots;
        

        public float AngularSpeed => Mathf.Abs(_angularSpeed);
        public float SpeedRatio => AngularSpeed / _data.MaxSpeed;
        public bool HasToCorrect => _hasToCorrectSnap;
        public bool IsSnapping { get; private set; }
        public bool IsStopping { get; private set; }

        public Vector2 Position => _objectToRotate.position;
        public event Action<int> OnStartMoving;
        public event Action OnStartStop;
        public event Action OnStopped;
        public event Action<int> OnDirectionChange;
        public event Action OnSnapCorrected;
        public event Action OnCheckForCorrection;
        
        
        public MovementComponent(IMovementData data, Transform objectToRotate, int angleSlots)
        {
            _data = data;
            _objectToRotate = objectToRotate;
            _angleSlots = angleSlots;

            _controller = new MovementFsm(this, (IFsmMovementData)_data);
            TransitionToStationary();
        }
        
        
        private void RotateObject(float deltaTime) 
            => _objectToRotate.Rotate(0f, 0f, _angularSpeed * deltaTime);

        private void ClampAngularSpeed() 
            => _angularSpeed = Mathf.Clamp(_angularSpeed, -_data.MaxSpeed, _data.MaxSpeed);

        private void CalculateTravelledSlots()
        {
            if(_travelledSlots >= _data.MaxSlotTravelDistance) return;
            
            var angleDiff = GetAngleDistance(_lastTravelledSlotAngle);

            if (angleDiff >= GetSlotSize())
            {
                _travelledSlots++;
                _lastTravelledSlotAngle = GetCurrentAngle();
            }
        }
        
        private float GetTravelledSlotsRatio() => (float)_travelledSlots / (float)_data.MaxSlotTravelDistance;

        #region IMovement

        public void SetDirection(float direction)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if(_direction == direction) return;
            
            _lastDirection = _direction;
            _direction = direction;
        }
        
        public void Update(float deltaTime)
        {
            _controller?.Execute(deltaTime);
            
            ClampAngularSpeed();
            RotateObject(deltaTime);
            CalculateTravelledSlots();
        }
        
        public void SetCorrectionData(int targetSlot)
        {
            _hasToCorrectSnap = true;
            
            _targetCorrectionAngle = GetAngleFromSlot(targetSlot, 0);
        }

        public void ClearCorrectionData()
        {
            _hasToCorrectSnap = false;
        }

        public void ForceStop()
        {
            Stop();
            TransitionToStationary();
        }

        public int GetCurrentSlot()
        {
            var slot = Mathf.RoundToInt(GetCurrentAngle() / GetSlotSize());
            return (slot + _angleSlots) % _angleSlots;
        }

        #endregion
        
        #region IFSMMovement
        
        // === Data === //

        #region Data
        
        public float GetSpeedRatio() => SpeedRatio;
        public float GetDirection() => _direction;
        public float GetLastMovementSpeedRatio() => _lastMovementSpeedRatio;
        public float GetAngleDistance(float targetAngle) => Mathf.Abs(Mathf.DeltaAngle(GetCurrentAngle(), targetAngle));
        public bool GetIsSnapFinished() => GetAngleDistance(_angleToSnap) <= MinDeltaSnapDistance;
        public bool GetIsCorrectionSnapFinished() => GetAngleDistance(_targetCorrectionAngle) <= MinDeltaSnapDistance;
        public bool GetHasToCorrectSnap() => _hasToCorrectSnap;

        #endregion
        
        // === Actions === //

        #region Actions

        #region Movement

        public void Stop()
        {
            _angularSpeed = 0;
        }

        public void ClearTravelledSlots()
        {
            _travelledSlots = 0;
            _lastTravelledSlotAngle = GetCurrentAngle();
        }

        public void ChangeDirection(float deltaTime, float direction)
        {
            _angularSpeed += (_data.ChangeDirectionAcceleration * deltaTime) * direction;
        }

        public void IncreaseSpeed(float deltaTime, float direction)
        {
            _angularSpeed += (_data.Acceleration * deltaTime) * direction;
        }
        
        public void SaveLastMovementSpeedRatio() => _lastMovementSpeedRatio = SpeedRatio;

        #endregion
        
        #region Snapping

        public void SnapToSlot()
        {
            _objectToRotate.rotation = Quaternion.Euler(0f,0f,_angleToSnap);
        }
        
        public void CalculateSnapAngle()
        {
            int targetSlot = GetCurrentSlot();
            var selectedValue = Mathf.Lerp(_data.SlotRangeToSnap.x, _data.SlotRangeToSnap.y, GetTravelledSlotsRatio());
            var signedLastDir = (int)Mathf.Sign(_lastDirection);
            targetSlot += (int)(selectedValue * signedLastDir);
            
            _angleToSnap = GetAngleFromSlot(targetSlot);
        }
        
        public void SmoothSnapToSlot(float deltaTime)
        {
            var finalSnapSpeed = _data.SnapSpeed * deltaTime;
            var rotationLerp= Quaternion.Lerp( 
                _objectToRotate.rotation, 
                Quaternion.Euler(0f,0f, _angleToSnap), 
                finalSnapSpeed);
            
            _objectToRotate.rotation = rotationLerp;
        }

        public void HandleSnapCorrection(float deltaTime)
        {
            var finalSnapSpeed = _data.CorrectionSnapSpeed * deltaTime;
            var rotationLerp= Quaternion.Lerp( 
                _objectToRotate.rotation, 
                Quaternion.Euler(0f,0f, _targetCorrectionAngle), 
                finalSnapSpeed);
            
            _objectToRotate.rotation = rotationLerp;
        }

        #endregion
        
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
        public void TriggerOnCheckForCorrection() => OnCheckForCorrection?.Invoke();
        
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
        public void TransitionSnapping() => _controller.Transition(States.Snapping);
        public void TransitionCorrectionSnapping() => _controller.Transition(States.CorrectionSnapping);
        
        #endregion

        #endregion
        
        #region Angle Slots
        
        private float GetCurrentAngle() => _objectToRotate.localEulerAngles.z;
        private float GetSlotSize() => AngleHelper.GetSlotSize(_angleSlots);
        private float GetAngleFromSlot(int slot, float angleOffset = 0f) => AngleHelper.GetAngleFromSlot(slot, _angleSlots, angleOffset);

        #endregion
        
    }
}