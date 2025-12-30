using System;
using _Main.Scripts.MyInputs;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Gameplay.Shield;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.MyTest.Inputs
{
    public class InputMovementTest : MonoBehaviour
    {
        [Header("Components")] 
        [SerializeField] private Transform spriteContainer;
        
        [SerializeField] private RotationDataSo rotationData;
        private IShieldMovement _shieldMovement;
        private ShieldSpeeder _shieldSpeeder;
        
        [SerializeField] private ShieldSpeeder.ShieldSpeederData data;

        private void Awake()
        {
            _shieldMovement = new RotationMovement(rotationData, spriteContainer);
            _shieldSpeeder = new ShieldSpeeder(_shieldMovement, data);
        }

        private void Start()
        {
            var inputReader = GetComponent<InputReader>();
            inputReader.OnMovementDirectionChanged += (value) =>
            {
                if(_shieldSpeeder.IsActive == false)
                    _shieldMovement.SetDirection(value);
            };
            
            BootEvents.InitializeSubSystems();
        }

        private void Update()
        {
            _shieldMovement.ExecuteMovement(Time.deltaTime);
            _shieldSpeeder.UpdateSpeed(Time.deltaTime);
        }

        public void EnableSpeeder()
        {
            _shieldSpeeder.IncreaseSpeed();
        }

        public void DisableSpeeder()
        {
            _shieldSpeeder.DecreaseSpeed();
        }
    }
    
#if UNITY_EDITOR
    
    [CustomEditor(typeof(InputMovementTest))]
    public class InputMovementTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Dibuja el inspector normal
            DrawDefaultInspector();

            // Agrega el botón
            InputMovementTest script = (InputMovementTest)target;

            if (GUILayout.Button("Enable Speeder")) 
                script.EnableSpeeder();
            
            if (GUILayout.Button("Disable Speeder")) 
                script.DisableSpeeder();
        }
    }
#endif
    
}
