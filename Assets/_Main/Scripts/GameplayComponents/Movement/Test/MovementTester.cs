using System;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.GameplayComponents.Movement.Test
{
    public interface IMovementTester
    {
        public float AngularSpeed { get; }
        public float Direction { get; }
        public int CurrentSlot { get; }
        public string CurrentState { get; }
    }
    
    public class MovementTester : MonoBehaviour, IMovementTester
    {
        [SerializeField] private MovementData movementData;
        
        [System.Serializable]
        private class MovementData : IMovementData, IFsmMovementData
        {
            [SerializeField] private float maxSpeed;
            [SerializeField] private float acceleration;
            [SerializeField] private float deceleration;
            [SerializeField] private float stopThreshold;
            [Range(0,1)]
            [SerializeField] private float minSpeedRatioToSnap;
            [SerializeField] private float snapSpeed;
            [SerializeField] private float changeDirectionAcceleration;
            [Range(0,1)]
            [SerializeField] private float stopChangeDirectionSpeedRatio;

            public float MaxSpeed => maxSpeed;
            public float Acceleration => acceleration;
            public float Deceleration => deceleration;
            public float StopThreshold => stopThreshold;
            public float MinSpeedRatioToSnap => minSpeedRatioToSnap;

            public float SnapSpeed => snapSpeed;
            public float CorrectionSnapSpeed { get; }
            public float ChangeDirectionAcceleration => changeDirectionAcceleration;
            public float StopChangeDirectionSpeedRatio => stopChangeDirectionSpeedRatio;
        }
        
        [Space]
        [SerializeField] private Transform shieldTransform;
        
        private IMovement _movementData;
        private MovementComponent.IMovementDebug _movementDebug;
        [Space]
        [Header("Debug")]
        [Range(1, 10f)] public float radius = 1f;
        private const int AngleSlots = 32;
        
        public float AngularSpeed => _movementDebug.AngularSpeed;
        public string CurrentState => _movementDebug.CurrentState;
        public int CurrentSlot => _movementData.GetCurrentSlot();
        public float Direction { get; private set; }
        
        private void Update()
        {
            InputNotifier();
            _movementData?.Update(Time.deltaTime);
        }

        private void InputNotifier()
        {
            var lastDirection = Direction;
            
            bool isPressingA = Input.GetKey(KeyCode.A);
            bool isPressingD = Input.GetKey(KeyCode.D);

            if (isPressingA && !isPressingD)
                Direction = 1;
            else if (isPressingD && !isPressingA)
                Direction = -1;
            else
                Direction = 0;

            if (lastDirection != Direction)
            {
                _movementData.SetDirection(Direction);
            }
        }

        public void MoveSlot(float direction)
        {
            
        }

        public void StopMovement()
        {
            _movementDebug.StopMovement();
        }

        public IMovement GetMovement()
        {
            _movementData = new MovementComponent(movementData,shieldTransform, AngleSlots);
            _movementDebug = (MovementComponent.IMovementDebug)_movementData;
            
            return _movementData;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;

            for (int i = 0; i < AngleSlots; i++)
            {
                float angle = i * (360f / AngleSlots);
                float rad = angle * Mathf.Deg2Rad;

                Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
                Gizmos.DrawLine(transform.position, transform.position + dir * radius);
            }
        }
    }
    
#if UNITY_EDITOR
    
    [CustomEditor(typeof(MovementTester))]
    public class MovementTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            MovementTester script = (MovementTester)target;
            
            if (GUILayout.Button("Next Slot")) script.MoveSlot(1);
            if (GUILayout.Button("Previous Slot")) script.MoveSlot(-1);
            if (GUILayout.Button("Stop Movement")) script.StopMovement();
        }
    }
    
#endif
}