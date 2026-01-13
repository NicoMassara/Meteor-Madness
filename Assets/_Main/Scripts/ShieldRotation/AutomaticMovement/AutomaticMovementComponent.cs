using System;
using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.ShieldRotation.Contracts;
using _Main.Scripts.ShieldRotation.Tools;
using UnityEngine;

namespace _Main.Scripts.ShieldRotation.AutomaticMovement
{
    public class AutomaticMovementComponent : IAutomaticMovement,
        AutomaticMovementComponent.IFsmAutomatic
    {
        #region FSM

        private enum States
        {
            Idle,
            Checking,
            Snapping,
        }

        private interface IFsmAutomatic
        {
            // === Data Getters === // 
            public bool GetHasReachedTargetAngle();
            public bool GetHasTargetAngle();
            public bool GetCanCheck();
            
            // === Actions === // 
            public void DisableCheck();
            public void SnapToAngle(float deltaTime);
            public void ForceSnap();
            public void CalculateAngleData();
            public void ClearAngleData();
            public void CheckForTarget(float deltaTime);
            public void ClearCheckTimer();
            
            // === Triggers === // 
            public void TriggerOnStartSnapping();
            public void TriggerOnStopSnapping();
            public void TriggerOnCheckForTarget();
            public void TriggerOnClearTarget();
            
            // === Transitions === // 
            public void TransitionToIdle();
            public void TransitionToSnapping();
            public void TransitionToChecking();
        }

        #region States

        private abstract class AutomaticStateBase : StateBase<IFsmAutomatic> { }

        private class IdleState : AutomaticStateBase
        {
            public override void Execute(float deltaTime)
            {
                if (Controller.GetCanCheck())
                {
                    Controller.TransitionToChecking();
                }
            }
        }

        private class CheckingState : AutomaticStateBase
        {
            public override void Awake()
            {
                Controller.ClearCheckTimer();
            }
            
            public override void Execute(float deltaTime)
            {
                Controller.CheckForTarget(deltaTime);
                
                if (Controller.GetHasTargetAngle())
                {
                    Controller.TransitionToSnapping();
                }
            }
        }

        private class SnappingState : AutomaticStateBase
        {
            public override void Awake()
            {
                Controller.DisableCheck();
                Controller.TriggerOnStartSnapping();
                Controller.CalculateAngleData();
            }

            public override void Execute(float deltaTime)
            {
                Controller.SnapToAngle(deltaTime);

                if (Controller.GetHasReachedTargetAngle())
                {
                    Controller.ForceSnap();
                    Controller.TransitionToIdle();
                }
            }

            public override void Sleep()
            {
                Debug.Log("Automatic - CheckingState::Sleep");
                Controller.TriggerOnStopSnapping();
                Controller.ClearAngleData();
            }
        }

        #endregion

        #region Controller

        private class AutomaticFsm : FsmController<States>
        {
            public AutomaticFsm(IFsmAutomatic automatic)
            {
                Initialize(automatic);
            }
            
            private void Initialize(IFsmAutomatic automatic)
            {
                var tempList = new List<StateData>
                {
                    new (States.Idle, new IdleState()),
                    new (States.Checking, new CheckingState()),
                    new (States.Snapping, new SnappingState()),
                };

                foreach (var state in tempList.Select(item => (AutomaticStateBase)item.State))
                {
                    state.InitializeState(automatic);
                }
                
                InitializeStates(tempList);
            }
        }

        #endregion

        #endregion
        
        private const float MinDistanceToTarget = 5f;
        private const float CheckTimerDelay = 0.25f;
        private readonly AutomaticFsm _fsmController;
        private readonly Transform _objectToRotate;
        private readonly IAutomaticMovementData _data;
        private readonly int _angleSlots;
        private int _targetDirection;
        private float _checkForTargetTimer;
        private float _angularSpeed;
        private float _targetAngle;
        private float _currentAngle;
        private bool _hasTargetAngle;
        private bool _canCheck;
        public float AngularSpeed => _angularSpeed;
        
        public event Action OnStartSnapping;
        public event Action OnStopSnapping;
        public event Action OnCheckForTarget;
        public event Action OnClearTarget;

        public AutomaticMovementComponent(Transform objectToRotate, IAutomaticMovementData data, int angleSlots = 32)
        {
            _objectToRotate = objectToRotate;
            _data = data;
            _angleSlots = angleSlots;
            _fsmController = new AutomaticFsm(this);
            TransitionToIdle();
        }

        private float GetCurrentAngle() => _objectToRotate.localEulerAngles.z;
        private void RotateObject(float deltaTime) 
            => _objectToRotate.Rotate(0f, 0f, _angularSpeed * deltaTime);
        private void ClampAngularSpeed() 
            => _angularSpeed = Mathf.Clamp(_angularSpeed, -_data.MaxVel, _data.MaxVel);
        
        private float GetSignedDistanceToTarget() => AngleHelper.SignedAngularDistance(GetCurrentAngle(), _targetAngle);
        private float GetAbsoluteDistanceToTarget() => Mathf.Abs(AngleHelper.SignedAngularDistance(GetCurrentAngle(), _targetAngle));

        #region IAutomaticMovement

        public void Update(float deltaTime)
        {
            _fsmController?.Execute(deltaTime);
            
            ClampAngularSpeed();
            RotateObject(deltaTime);
        }

        public void ClearTarget() => SetTargetAngle(-1);

        public void SetTargetAngle(int targetSlot)
        {
            if (targetSlot <= -1)
            {
                _hasTargetAngle = false;
                return;
            }

            _hasTargetAngle = true;
            _targetAngle = AngleHelper.GetAngleFromSlot(targetSlot,_angleSlots, 0);
        }

        public void EnableCheck()
        {
            _canCheck = true;
        }

        #endregion

        #region IFsmAutomatic
        
        // === Data Getters === // 

        #region Data Getters

        public bool GetHasReachedTargetAngle() => GetAbsoluteDistanceToTarget() <= MinDistanceToTarget;
        public bool GetHasTargetAngle() => _hasTargetAngle;
        public bool GetCanCheck() => _canCheck;

        #endregion
        
        // === Actions === // 
        #region Actions

        public void DisableCheck()
        {
            _canCheck = false;
        }

        public void CheckForTarget(float deltaTime)
        {
            _checkForTargetTimer -= deltaTime;
            
            if(_checkForTargetTimer > 0) return;
            
            TriggerOnCheckForTarget();
            _checkForTargetTimer = CheckTimerDelay;
        }

        public void ClearCheckTimer()
        {
            _checkForTargetTimer = 0;
        }

        public void CalculateAngleData()
        {
            if (GetHasReachedTargetAngle())
            {
                return;
            }

            var signedDist = GetSignedDistanceToTarget();
            _targetDirection = (int)Mathf.Sign(signedDist);
            _angularSpeed = (_data.MaxVel * _targetDirection);
        }

        public void SnapToAngle(float deltaTime)
        {

        }
        
        public void ClearAngleData()
        {
            ClearTarget();
            TriggerOnClearTarget();
        }

        public void ForceSnap()
        {
            _angularSpeed = 0;
            _objectToRotate.rotation = Quaternion.Euler(0f,0f,_targetAngle);
        }

        #endregion
        
        // === Triggers === // 
        #region Triggers

        public void TriggerOnStartSnapping() => OnStartSnapping?.Invoke();
        public void TriggerOnStopSnapping() => OnStopSnapping?.Invoke();
        public void TriggerOnCheckForTarget() => OnCheckForTarget?.Invoke();
        public void TriggerOnClearTarget() => OnClearTarget?.Invoke();

        #endregion
        
        // === Transitions === // 
        #region Transitions
        public void TransitionToIdle() => _fsmController?.Transition(States.Idle);
        public void TransitionToChecking() => _fsmController?.Transition(States.Checking);
        public void TransitionToSnapping() => _fsmController?.Transition(States.Snapping);

        #endregion

        #endregion
    }
}