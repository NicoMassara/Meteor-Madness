using System;
using _Main.Scripts.GameplayComponents.Movement;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.GameplayComponents.MovementCorrection.Test
{
    internal interface IMovementCorrectionTester
    {
        public int TargetSlot { get; }
        public int SlotDistance { get; }
        public float Distance { get; }
        public float DirectionToTarget { get; }
        public bool IsInRange { get; }
        public bool IsInFrontOfTarget { get; }
    }
    
    internal interface IMovementTester
    {
        public float AngularSpeed { get; }
        public float Direction { get; }
        public int CurrentSlot { get; }
        public string CurrentState { get; }
        public bool HasToCorrect { get; }
    }

    internal class MovementCorrectionTester : MonoBehaviour, IMovementCorrectionTester
    {
        [SerializeField] private TargetTester targetTester;

        [Space]
        [SerializeField] private CorrectionData correctionData;

        [System.Serializable]
        private class CorrectionData : IMovementCorrectionData
        {
            [Range(0, 16)]
            [SerializeField] private int correctionSlotDistance;
            [Range(0,50)]
            [SerializeField] private float maxDistance;
            
            public int CorrectionSlotDistance => correctionSlotDistance;
            public float MaxDistance => maxDistance;
        }
        
        [SerializeField] private Transform objectToRotate;
        [SerializeField] private MovementDataSo movementDataSo;

        private class MovementBehavior : IMovementTester
        {
            private readonly IMovement _movement;
            private readonly MovementComponent.IMovementDebug _debug;
            private float _direction;

            public IMovement Movement => _movement;
            public event Action<float> OnInputChanged;
            public event Action OnSnapCorrected;

            public MovementBehavior(IMovementData data, Transform objectToRotate)
            {
                _movement = new MovementComponent(data, objectToRotate, 32);
                _debug = (MovementComponent.IMovementDebug)_movement;
                
                _movement.OnSnapCorrected += ()=> OnSnapCorrected?.Invoke();
            }
            
            public void UpdateInputs()
            {
                var lastDirection = _direction;
            
                bool isPressingA = Input.GetKey(KeyCode.A);
                bool isPressingD = Input.GetKey(KeyCode.D);

                if (isPressingA && !isPressingD)
                    _direction = 1;
                else if (isPressingD && !isPressingA)
                    _direction = -1;
                else
                    _direction = 0;

                if (lastDirection != _direction)
                {
                    OnInputChanged?.Invoke(_direction);
                }
            }

            public void SetDirection(float direction) 
                => _movement.SetDirection(direction);

            public void Execute(float deltaTime) 
                => _movement?.Update(deltaTime);

            public float AngularSpeed => _debug.AngularSpeed;
            public float Direction => _direction;
            public int CurrentSlot => _movement.GetCurrentSlot();
            public string CurrentState => _debug.CurrentState;
            public bool HasToCorrect => _debug.HasToCorrect;
        }

        private MovementBehavior _movement;
        public IMovementTester Movement => _movement;

        public int TargetSlot => GetTargetSlot();
        public int SlotDistance => GetSlotDistance();
        public float Distance => GetDistanceToTarget();
        public float DirectionToTarget => GetDirectionToTarget();
        public bool IsInRange => GetTargetIsInRange();
        public bool IsInFrontOfTarget => GetIsInFrontOfTarget();

        private IMovementCorrection _movementCorrection;
        private bool _isCorrecting;

        private void Awake()
        {
            _movement = new MovementBehavior(movementDataSo, objectToRotate);
            _movementCorrection = new MovementCorrectionComponent(correctionData, _movement.Movement, transform, 32);
        }

        private void Start()
        {
            _movement.OnInputChanged += Movement_OnInputChanged;
            _movement.OnSnapCorrected += Movement_OnSnapCorrected;
        }

        private void Update()
        {
            if (IsInRange && IsInFrontOfTarget == false
                && (_movement.Movement.IsStopping || _movement.Movement.IsSnapping))
            {
                _movement?.Movement.SetCorrectionData(TargetSlot);
            }
            else
            {
                _movement?.Movement.ClearCorrectionData();
            }

            _movement?.UpdateInputs();
            _movement?.Execute(Time.deltaTime);
        }

        private void Movement_OnInputChanged(float direction)
        {
            _movement?.SetDirection(direction);
        }
        
        private void Movement_OnSnapCorrected()
        {
            ClearTarget();
        }

        public void FindTarget()
        {
            targetTester = FindAnyObjectByType<TargetTester>();
        }

        public void ClearTarget()
        {
            targetTester = null;
        }

        #region IMovementCorrectionTester

        private int GetTargetSlot()
        {
            if(_movementCorrection == null) return -1;
            if(targetTester == null) return -1;
            
            return _movementCorrection.GetAngleSlotFromTarget(targetTester);
        }
        
        private int GetSlotDistance()
        {
            if(_movementCorrection == null) return -1;
            if(targetTester == null) return -1;
            
            return _movementCorrection.GetSlotDistance(targetTester);
        }
        
        private int GetDirectionToTarget()
        {
            if(_movementCorrection == null) return -1;
            if(targetTester == null) return -1;
            
            return _movementCorrection.GetDirectionToTarget(targetTester);
        }
        private float GetDistanceToTarget()
        {
            if(_movementCorrection == null) return -1;
            if(targetTester == null) return -1;
            
            return _movementCorrection.GetDistanceToTarget(targetTester);
        }
        
        private bool GetTargetIsInRange()
        {
            if(_movementCorrection == null) return false;
            if(targetTester == null) return false;
            
            return _movementCorrection.GetTargetIsInRange(targetTester);
        }
        
        private bool GetIsInFrontOfTarget()
        {
            if(_movementCorrection == null) return false;
            if(targetTester == null) return false;
            
            return _movementCorrection.GetIsInFrontOfTarget(targetTester);
        }

        #endregion
    }
#if UNITY_EDITOR
    
    [CustomEditor(typeof(MovementCorrectionTester))]
    public class MovementCorrectionTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            MovementCorrectionTester script = (MovementCorrectionTester)target;
            
            if (GUILayout.Button("Find Target")) script.FindTarget();
        }
    }
    
#endif
}