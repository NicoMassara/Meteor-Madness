using System;
using _Main.Scripts.ShieldRotation.Contracts;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.ShieldRotation.Mediator.Test
{
    public class MediatorTester : MonoBehaviour
    {
        [SerializeField] private MediatorDataSo mediatorData;
        [SerializeField] private Transform shieldTransform;
        [SerializeField] private LayerMask projectileLayerMask;
        private IMediator _shieldMediator;
        private float _direction;

        private void Awake()
        {
            _shieldMediator = new MediatorComponent(shieldTransform, 32, mediatorData, projectileLayerMask);
        }

        private void Start()
        {
            _shieldMediator.Enable();
        }

        private void Update()
        {
            UpdateInputs();
            
            _shieldMediator?.SetInputDirection(_direction);
            _shieldMediator?.Update(Time.deltaTime);
        }
        
        private void UpdateInputs()
        {
            _direction = 0f;
            
            bool isPressingA = Input.GetKey(KeyCode.A);
            bool isPressingD = Input.GetKey(KeyCode.D);

            if (isPressingA && !isPressingD)
                _direction = 1;
            else if (isPressingD && !isPressingA)
                _direction = -1;
            else
                _direction = 0;
        }
        
        // === Input === //

        internal void EnableInput() => _shieldMediator.Enable();
        internal void DisableInput() => _shieldMediator.Disable();
        
        // === Speeder === //
        internal void SpeedUp() => _shieldMediator.SpeedUp();
        internal void SlowDown() => _shieldMediator.SlowDown();
        
        // === Target Snapper === //
        internal void TryToSnapToTarget() => _shieldMediator.TryToSnapToTarget();
        
        // === Automatic === //
        internal void EnableAutoTarget() => _shieldMediator.EnableAutomatic();
        internal void DisableAutoTarget() => _shieldMediator.DisableAutomatic();
    }
    
#if UNITY_EDITOR
    
    [CustomEditor(typeof(MediatorTester))]
    public class MediatorTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            MediatorTester script = (MediatorTester)target;
            
            GUILayout.Space(10);
            GUILayout.Label("Inputs");
            if (GUILayout.Button("Enable Input")) script.EnableInput();
            if (GUILayout.Button("Disable Input")) script.DisableInput();
            
            GUILayout.Space(10);
            GUILayout.Label("Speeder");
            if (GUILayout.Button("Speed Up")) script.SpeedUp();
            if (GUILayout.Button("Slow Down")) script.SlowDown();
            
            GUILayout.Space(10);
            GUILayout.Label("Target Snapper");
            if (GUILayout.Button("Try To Snap")) script.TryToSnapToTarget();
            
            GUILayout.Space(10);
            GUILayout.Label("Auto Target");
            if (GUILayout.Button("Enable Auto Target")) script.EnableAutoTarget();
            if (GUILayout.Button("Disable Auto Target")) script.DisableAutoTarget();
        }
    }
    
#endif
}