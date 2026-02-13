using System;
using MeteorMadness.GlobalValues.Utilities;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.Components
{
    public class AutomaticInput : IAutomaticInput
    {
        private readonly IAutomaticInputData _data;
        private readonly ITargetDetector _detector;
        private bool _isActive;

        private float _currentTimer;

        public AutomaticInput(IAutomaticInputData data, ITargetDetector detector)
        {
            _data = data;
            _detector = detector;
        }
        
        public event Action<float> OnTargetFound;

        #region IAutomaticInput

        public void SetActive(bool isActive)
        {
            _isActive = isActive;
        }

        public void Execute(float deltaTime)
        {
            if(_isActive == false) return;
            
            if (_currentTimer > 0)
            {
                _currentTimer -= deltaTime;
                return;
            }

            var hasTarget = _detector.GetNearestTarget(out int targetSlot);

            if (hasTarget)
            {
                var targetAngle = AngleCalculations.GetAngleFromSlot(targetSlot);
                
                SetActive(false);
                OnTargetFound?.Invoke(targetAngle);
            }
            else
            {
                _currentTimer = _data.CheckRate;
            }
        }

        public IRotationData GetRotationData() => _data.RotationData;
        
        #endregion
    }
}