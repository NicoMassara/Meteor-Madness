using System;
using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.ShieldRotation.Contracts;
using _Main.Scripts.ShieldRotation.Tools;
using UnityEngine;

namespace _Main.Scripts.ShieldRotation.TargetSnapper
{
    public class TargetSnapperComponent : ITargetSnapper,
        TargetSnapperComponent.IFsmSnapper
    {
        #region FSM
        
        private enum States
        {
            Idle,
            Snapping,
        }

        private interface IFsmSnapper
        {
            // === Data Getters === //
            public bool HasSnapped();
            public bool HasTargetAngle();
            
            // === Actions === //
            public void CalculateSnapping();
            public void ClearSnapData();
            public void DecreaseSpeed(float deltaTime);
            
            // === Triggers === //
            public void TriggerOnSnapped();
            public void TriggerOnSnapping();
            
            // === Transitions === //
            public void TransitionToSnapping();
            public void TransitionToIdle();
        }

        #region States

        private abstract class SnapperStateBase : StateBase<IFsmSnapper> { }

        private class IdleState : SnapperStateBase
        {
            public override void Execute(float deltaTime)
            {
                if (Controller.HasTargetAngle())
                {
                    Controller.TransitionToSnapping();
                }
            }
        }

        private class SnappingState : SnapperStateBase
        {
            public override void Awake()
            {
                Controller.TriggerOnSnapping();
                Controller.CalculateSnapping();
            }

            public override void Execute(float deltaTime)
            {
                Controller.DecreaseSpeed(deltaTime);

                if (Controller.HasSnapped())
                {
                    Controller.TransitionToIdle();
                }
            }

            public override void Sleep()
            {
                Controller.TriggerOnSnapped();
                Controller.ClearSnapData();
            }
        }

        #endregion

        #region Controllers

        private class SnapperFsm : FsmController<States>
        {
            public SnapperFsm(IFsmSnapper snapper)
            {
                Initialize(snapper);
            }
            
            private void Initialize(IFsmSnapper snapper)
            {
                var tempList = new List<StateData>
                {
                    new (States.Idle, new IdleState()),
                    new (States.Snapping, new SnappingState()),
                };

                foreach (var state in tempList.Select(item => (SnapperStateBase)item.State))
                {
                    state.InitializeState(snapper);
                }
                
                InitializeStates(tempList);
            }
        }

        #endregion

        #endregion

        private const float MinDistanceRatio = 0.01f;
        private readonly SnapperFsm _fsmController;
        private readonly Transform _objectToRotate;
        private readonly ITargetSnapperData _data;
        private readonly float _angleSlots;
        private float _angularSpeed;
        private float _targetAngle;
        private float _startAngle;
        private float _startAngleDiff;
        private bool _hasToSnap;
        public float AngularSpeed => _angularSpeed;
        public float StartVelocity => _data.StartVelocity;
        public event Action OnSnapping;
        public event Action OnSnapped;
        

        public TargetSnapperComponent(Transform objectToRotate, ITargetSnapperData data, float angleSlots)
        {
            _objectToRotate = objectToRotate;
            _data = data;
            _angleSlots = angleSlots;
            _fsmController = new SnapperFsm(this);
            TransitionToIdle();
        }

        private void RotateObject(float deltaTime) 
            => _objectToRotate.Rotate(0f, 0f, _angularSpeed * deltaTime);
        
        private float GetCurrentAngle() => _objectToRotate.localEulerAngles.z;
        private float GetCurrentAngleDiff() => AngleHelper.GetAngularDistanceCCW(GetCurrentAngle(), _targetAngle);
        private float GetDistanceRatio() => Mathf.Clamp01((GetCurrentAngleDiff() / _startAngleDiff));

        #region ITargetSnapper

        public void Update(float deltaTime)
        {
            _fsmController?.Execute(deltaTime);
            
            RotateObject(deltaTime);
        }
        
        public void SetTargetSlot(int targetSlot)
        {
            _hasToSnap = true;
            _targetAngle = AngleHelper.GetAngleFromSlot(targetSlot);
            _angularSpeed = StartVelocity;
            Debug.Log($"TargetSnapperComponent :: Target Angle: {_targetAngle}");
        }

        #endregion

        #region IFsmSnapper
        
        // === Data Getters === //
        #region Data Getters

        public bool HasSnapped() => GetDistanceRatio() <= 0;
        public bool HasTargetAngle() => _hasToSnap == true;
        #endregion

        // === Actions === //
        #region Actions

        public void ClearSnapData()
        {
            _hasToSnap = false;
        }

        public void CalculateSnapping()
        {
            var startAngle = GetCurrentAngle();
            _startAngleDiff = AngleHelper.GetAngularDistanceCCW(startAngle, _targetAngle);
        }

        public void DecreaseSpeed(float deltaTime)
        {
            if (GetDistanceRatio() <= MinDistanceRatio)
            {
                _objectToRotate.rotation = Quaternion.Euler(0f,0f,_targetAngle);
            }
        }

        #endregion
        
        // === Triggers === //
        #region Triggers
        public void TriggerOnSnapped() => OnSnapped?.Invoke();
        public void TriggerOnSnapping() => OnSnapping?.Invoke();

        #endregion
        
        // === Transitions === //
        #region Transitions

        public void TransitionToSnapping() => _fsmController?.Transition(States.Snapping);
        public void TransitionToIdle() => _fsmController?.Transition(States.Idle);

        #endregion

        #endregion

        #region Angle Slots

        private float GetSlotSize() => 360f / _angleSlots;

        private float GetAngleFromSlot(int slot)
        {
            var angle = slot * GetSlotSize();
            return (angle + 360f) % 360f;
        }

        #endregion
    }
}