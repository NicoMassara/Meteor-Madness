using System;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Tutorial
{
    public class TutorialMotor : ObservableComponent
    {
        #region Private Classes
        private class MovementCounter
        {
            private readonly float _maxMovementTime;
            
            private float _currentMovementTime;

            public event Action OnFinished;

            public MovementCounter(float maxMovementTime = 3)
            {
                _maxMovementTime = maxMovementTime;
            }

            public void Update(float deltaTime)
            {
                _currentMovementTime += deltaTime;

                if (_currentMovementTime > _maxMovementTime)
                {
                    OnFinished?.Invoke();
                }
            }

            public void Reset() => _currentMovementTime = 0;
        }
        
        #endregion

        private const float RotateTime = 2f;
        private MovementCounter _rightCounter;
        private MovementCounter _leftCounter;
        
        private float _currentDirection;

        public TutorialMotor()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            _rightCounter = new MovementCounter(RotateTime);
            _leftCounter = new MovementCounter(RotateTime);

            _rightCounter.OnFinished += () =>
            {
                NotifyAll(TutorialObserverMessage.RightMovementFinished);
            };
            
            _leftCounter.OnFinished += () =>
            {
                NotifyAll(TutorialObserverMessage.LeftMovementFinished);
            };
        }

        public void Meteor() => NotifyAll(TutorialObserverMessage.Meteor);
        public void Ability() => NotifyAll(TutorialObserverMessage.Ability);
        public void Finish() => NotifyAll(TutorialObserverMessage.Finish);
        public void Disable() => NotifyAll(TutorialObserverMessage.Disable);
        public void Enable()
        {
            _rightCounter.Reset();
            _leftCounter.Reset();
            NotifyAll(TutorialObserverMessage.Enable);
        }

        public void SpawnExtraMeteors() => NotifyAll(TutorialObserverMessage.ExtraMeteors);
        public void SendAdditionalProjectile(int projectileTypeIndex) => NotifyAll(TutorialObserverMessage.AdditionalProjectile, projectileTypeIndex);
        public void SetMultiPage() => NotifyAll(TutorialObserverMessage.MultiPage);
        public void TriggerSphereDeflected() => NotifyAll(TutorialObserverMessage.SphereDeflected);
        public void SetAbilityRunning() => NotifyAll(TutorialObserverMessage.AbilityRunning);
        public void EnableHint() => NotifyAll(TutorialObserverMessage.EnableHint);
        public void DisableHint() => NotifyAll(TutorialObserverMessage.DisableHint);
        public void SetRightMovement() => NotifyAll(TutorialObserverMessage.RightMovement);
        public void SetLeftMovement() => NotifyAll(TutorialObserverMessage.LeftMovement);

        #region Movement

        public void SetMovementDirection(float direction) => _currentDirection = direction;

        public void ExecuteRightMovement(float deltaTime)
        {
            if(Mathf.Sign(_currentDirection) < 0)
                _rightCounter.Update(deltaTime);
        }
        
        public void ExecuteLeftMovement(float deltaTime)
        {
            if(Mathf.Sign(_currentDirection) > 0)
                _leftCounter.Update(deltaTime);
        }

        #endregion
    }
}