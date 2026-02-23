using System;
using MeteorMadness.Common;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.Components
{
    public class AngularRotation : IAngularRotation, AngularRotation.IController
    {
        private enum States
        {
            Disabled,
            Idle,
            Rotating,
            Snapping
        }
        
        private interface IController : IFsmController
        {
            public bool GetHasTargetAngle();
            public void Rotate(float deltaTime);
            public void SnapToTargetAngle();
            public void FinishRotation();
            public bool GetHasReachedTargetAngle();
            public void DisableRotation();
            public void TriggerOnStartRotation();
            public void TriggerOnTargetReached();
        }

        private class Controller : SimpleFsm<IController, States>
        {
            public Controller(IController controller) : base(controller) { }
            protected override void AwakeState(States state)
            {
                switch (state)
                {
                    case States.Disabled:
                        FsmController.DisableRotation();
                        break;
                    
                    case States.Rotating:
                        FsmController.TriggerOnStartRotation();
                        break;
                    
                    case States.Snapping:
                        
                        FsmController.SnapToTargetAngle();
                        FsmController.TriggerOnTargetReached();
                        ChangeState(States.Idle);
                        
                        break;
                }
            }

            protected override void ExecuteState(float deltaTime)
            {
                switch (CurrentState)
                {
                    case States.Idle:
                        
                        if (FsmController.GetHasTargetAngle())
                        {
                            ChangeState(States.Rotating);
                        }

                        break;
                    
                    case States.Rotating:
                        
                        FsmController.Rotate(deltaTime);

                        if (FsmController.GetHasReachedTargetAngle())
                        {
                            ChangeState(States.Snapping);
                        }

                        break;
                }
            }

            protected override void SleepState(States state)
            {
                switch (state)
                {
                    case States.Rotating:

                        FsmController.FinishRotation();
                        
                        break;
                }
            }
        }

        private readonly Transform _objectToRotate;
        private readonly Controller _controller;
        private IRotationData _rotationData;
        private bool _hasTargetAngle;
        private float _targetAngle;
        private float _speedMultiplier;
        private float _currentAcceleration;
        
        public event Action OnTargetReached;
        public event Action OnStartRotation;

        public AngularRotation(Transform objectToRotate)
        {
            _objectToRotate = objectToRotate;
            _controller = new Controller(this);
        }

        #region Private API

        private float GetMaxSpeed() => _rotationData.MaxSpeed * _speedMultiplier;
        private float GetSpeedRatio() => Mathf.Clamp01(_currentAcceleration / GetMaxSpeed());
        private float GetCurrentAngle() => _objectToRotate.rotation.eulerAngles.z;

        #endregion

        #region IAngularRotation
        
        public void Execute(float deltaTime) => _controller.Update(deltaTime);
        public void SetEnable(bool isEnabled) => _controller.ChangeState(isEnabled ? States.Idle : States.Disabled);
        public void SetRotationData(IRotationData rotationData) => _rotationData = rotationData;
        public void SetTargetAngle(float targetAngle)
        {
            _targetAngle = targetAngle;
            _hasTargetAngle = true;
        }
        
        public void SetSpeedMultiplier(float speedMultiplier = 1) => _speedMultiplier = Mathf.Max(speedMultiplier, 0f);

        public void StopRotation()
        {
            _targetAngle = 0;
            _hasTargetAngle = false;
        }

        public void RestartRotation()
        {
            _targetAngle = 0;
            SnapToTargetAngle();
        }

        #endregion
        
        #region IController

        public void DisableRotation()
        {
            FinishRotation();
            _rotationData = null;
        }

        public bool GetHasTargetAngle()
        {
            if (_rotationData == null)
            {
                Debug.LogWarning("No rotation data found");
                return false;
            }
            
            return _hasTargetAngle;
        }

        public void Rotate(float deltaTime)
        {
            var finalAcc = _rotationData.Acceleration * _speedMultiplier;
            _currentAcceleration += finalAcc * deltaTime;
            
            var finalSpeed = finalAcc * deltaTime;
            var targetAngle = Quaternion.Euler(0, 0, _targetAngle);
            var currentAngle = _objectToRotate.rotation;
            var finalRotation = Quaternion.RotateTowards(currentAngle ,targetAngle, finalSpeed);
            _objectToRotate.rotation = finalRotation;
        }

        public void SnapToTargetAngle() => _objectToRotate.rotation = Quaternion.Euler(0, 0, _targetAngle);
        public void FinishRotation()
        {
            _currentAcceleration = 0;
            _hasTargetAngle = false;
        }
        
        public bool GetHasReachedTargetAngle()
        {
            const float angleThreshold = 0.1f;
            return Mathf.Abs(GetCurrentAngle() - _targetAngle) <= angleThreshold;
        }
        
        public void TriggerOnStartRotation() => OnStartRotation?.Invoke();
        public void TriggerOnTargetReached() => OnTargetReached?.Invoke();
        

        #endregion
    }
}