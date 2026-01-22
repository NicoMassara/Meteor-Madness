using System;
using _Main.Scripts.Contracts;
using MeteorMadness.Contracts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Movement
{
    public class MovementBehavior : MonoBehaviour, IDebugMovement
    {
        
        [SerializeField] private MovementDataSo movementData;
        [SerializeField] private Transform targetToMove;
        [SerializeField] private bool debugEnable;
        private IInputReader _inputReader;
        private IMovement _movementComponent;
        
        #region IDebugMovement

        public string DebugName { get; } = "Movement";
        public bool IsDebugEnable => debugEnable;
        public Vector3 DebugPosition => transform.position;
        public IDebugMovementComponent DebugComponent { get; private set; }

        #endregion

        private void Awake()
        {
            var movementComponent = new MovementComponent(targetToMove,movementData);
            _movementComponent = movementComponent;
            DebugComponent = movementComponent;
            
            _inputReader = GetComponent<IInputReader>();
        }

        private void Start()
        {
            _movementComponent.Initialize();
            _movementComponent.OnMoved += OnMovedHandler;
            _movementComponent.OnStopped += OnOnStoppedHandler;
            _movementComponent.OnDirectionChanged += OnDirectionChangedHandler;
            
            _inputReader.OnMoved += InputReader_OnMovedHandler;
            _inputReader.OnMagnitudeChanged += InputReader_OnMagnitudeChangedHandler;
            _inputReader.OnStopInput += InputReader_OnStopInputHandler;
        }
        
        private void Update()
        {
            _movementComponent.Update(Time.deltaTime);
        }
        
        #region Handlers
        
        private void InputReader_OnMovedHandler(float angle)
        {
            _movementComponent.SetInputAngle(angle);
        }
        private void InputReader_OnMagnitudeChangedHandler(float magnitude)
        {
            _movementComponent.SetInputMagnitude(magnitude);
        }
        private void InputReader_OnStopInputHandler()
        {
            _movementComponent.RemoveInput();
        }
        
        private void OnDirectionChangedHandler()
        {
            Debug.Log("Direction Changed");
        }

        private void OnOnStoppedHandler()
        {
            Debug.Log("Stopped");
        }

        private void OnMovedHandler()
        {
            Debug.Log("Moving");
        }
        
        #endregion
    }
}