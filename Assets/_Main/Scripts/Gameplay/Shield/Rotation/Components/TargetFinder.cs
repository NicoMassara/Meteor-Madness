using System;
using MeteorMadness.GlobalValues.Utilities;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.Components
{
    public class TargetFinder : ITargetFinder
    {
        private readonly ITargetFinderData _data;
        private readonly ITargetDetector _detector;
        
        public event Action<float> OnTargetFound;
        public event Action OnTargetNotFound;

        public TargetFinder(ITargetFinderData data, ITargetDetector detector)
        {
            _data = data;
            _detector = detector;
        }

        #region ITargetFinder

        public void TryToFindTarget()
        {
            var hasTarget = _detector.GetNearestTarget(out var targetSlot);
            
            if (hasTarget)
            {
                var targetAngle = AngleCalculations.GetAngleFromSlot(targetSlot);
                OnTargetFound?.Invoke(targetAngle);
            }
            else
            {
                OnTargetNotFound?.Invoke();
            }
        }

        public IRotationData GetRotationData() => _data.RotationData;

        #endregion
    }
}