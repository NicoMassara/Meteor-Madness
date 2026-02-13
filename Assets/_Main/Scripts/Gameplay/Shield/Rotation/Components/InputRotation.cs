using System;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.Components
{
    public class InputRotation : IInputRotation
    {
        private readonly IInputRotationData _data;
        private float _inputAngle;
        private float _inputMagnitude;
        
        public event Action<float> OnInputChanged;  
        public event Action<float> OnMagnitudeChanged;  

        public InputRotation(IInputRotationData data)
        {
            _data = data;
        }

        #region Private API

        private float GetSpeedMagnitudeCurve(float magnitude) => _data.MagnitudeCurve.Evaluate(magnitude);

        #endregion
        
        #region IInputRotation

        public void DisableInput()
        {
            _inputAngle = 0;
            _inputMagnitude = 0;
        }

        public void SetInputAngle(float inputAngle)
        {
            var temp = Mathf.Abs(inputAngle - _inputAngle);
            
            if (temp < _data.MinInputAngle)
            {
                return;
            }
            
            _inputAngle = inputAngle;
            OnInputChanged?.Invoke(_inputAngle);
        }
        
        public void SetInputMagnitude(float inputMagnitude)
        {
            if (Mathf.Approximately(_inputMagnitude, inputMagnitude))
                return;
            
            _inputMagnitude = inputMagnitude;
            OnMagnitudeChanged?.Invoke(GetSpeedMagnitudeCurve(_inputMagnitude));
        }

        public IRotationData GetRotationData() => _data.RotationData;
        public float GetInputAngle() => _inputAngle;
        public float GetInputMagnitude() => _inputMagnitude;

        #endregion
    }
}