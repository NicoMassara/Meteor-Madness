using System;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Inputs
{
    public class InputReader : MonoBehaviour, IInputReader, IInputUI
    {
        #region Data Classes

        [System.Serializable]
        private class InputData : IInputData
        {
            [Header("Limited Zone")] 
            [SerializeField] private RectTransform limitedZone;
            public RectTransform LimitedZone => limitedZone;
            
            [Header("Safe / Dead Zone Radius")]
            [SerializeField] private float deadZoneRadius = 3;
            [SerializeField] private float safeZoneRadius = 5;
            public float DeadZoneRadius => deadZoneRadius;
            public float SafeZoneRadius => safeZoneRadius;
        }

        #endregion
        
        [SerializeField] private InputData touchInputData;
        [SerializeField] private bool debugEnable = true;
        private Camera _gameCamera;
        private TouchInput _deviceInput;
        private bool _isEnable;

        #region IInputReader

        public event Action<float> OnMoved;
        public event Action<float> OnMagnitudeChanged;
        public event Action OnPrepareToMove;
        public event Action OnStopInput;

        #endregion

        #region IInputUI

        public event Action OnEnable;
        public event Action OnDisable;

        #endregion

        private void Awake()
        {
            _gameCamera = Camera.main;
            _deviceInput = new TouchInput(touchInputData, _gameCamera);;
            _deviceInput.OnPressingToMove += OnPressingToMoveHandler;
            _deviceInput.OnReleasedToMove += OnReleasedToMoveHandler;
            _deviceInput.OnDirectionChanged += OnDirectionChangedHandler;
            
            InputsEventSubscriber.SetEnable(EventBus_Inputs_SetEnable);
        }

        private void Start()
        {
            _deviceInput.Initialize();
        }

        #region Handlers
        
        private void OnPressingToMoveHandler(Vector2 direction)
        {
            if(_isEnable == false) return;
            
            OnPrepareToMove?.Invoke();
        }
        
        private void OnDirectionChangedHandler(float angle)
        {
            if(_isEnable == false) return;
            
            OnMoved?.Invoke(angle);
        }

        private void OnReleasedToMoveHandler()
        {
            if(_isEnable == false) return;
            
            OnStopInput?.Invoke();
        }
        
        private void OnMagnitudeChangedHandler(float magnitude)
        {
            if(_isEnable == false) return;
            
            OnMagnitudeChanged?.Invoke(magnitude);
        }

        #endregion
        
        private void EventBus_Inputs_SetEnable(InputsEvents.SetEnable input)
        {
            _isEnable = input.IsEnable;

            if (_isEnable)
                OnEnable?.Invoke();
            else
                OnDisable?.Invoke();
        }


    }
}