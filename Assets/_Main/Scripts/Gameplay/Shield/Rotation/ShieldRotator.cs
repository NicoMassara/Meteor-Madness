using System;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.Components;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation
{
    public class ShieldRotator : IShieldRotator,
        ShieldRotator.IController
    {
        private enum States
        {
            Disabled,
            Input,
            Automatic,
            Speeder,
            Finder
        }
        
        private interface IController : IFsmController
        {
            // === Disable === //
            public void EnableRotator();
            public void DisableRotator();
            // === Input === //
            public void Input_Enable();
            public void Input_Disable();
            // === Automatic === //
            public void Automatic_Enable();
            public void Automatic_Disable();
            public void Automatic_Execute(float deltaTime);
            // === Speeder === // 
            public void Speeder_Enable();
            public void Speeder_Disable();
            public void Speeder_Execute(float deltaTime);
            // === Finder === // 
            public void Finder_Enable();
            public void Finder_Disable();
            public void Finder_TryToFindTarget();
            
            // === Rotator === //
            public void Rotator_Execute(float deltaTime);
        }
        
        private class Controller : SimpleFsm<IController, States>
        {
            public Controller(IController controller) : base(controller) { }

            protected override void AwakeState(States state)
            {
                switch (state)
                {
                    case States.Disabled:
                        FsmController.DisableRotator();
                        break;
                    
                    case States.Input:
                        FsmController.Input_Enable();
                        break;
                    
                    case States.Automatic:
                        FsmController.Automatic_Enable();
                        break;
                    
                    case States.Speeder:
                        FsmController.Speeder_Enable();
                        break;
                    
                    case States.Finder:
                        FsmController.Finder_Enable();
                        FsmController.Finder_TryToFindTarget();
                        break;
                }
            }

            protected override void ExecuteState(float deltaTime)
            {
                switch (CurrentState)
                {
                    case States.Input or States.Finder:
                        FsmController.Rotator_Execute(deltaTime);
                        break;
                    
                    case States.Automatic:
                        FsmController.Automatic_Execute(deltaTime);
                        FsmController.Rotator_Execute(deltaTime);
                        break;
                    
                    case States.Speeder:
                        FsmController.Speeder_Execute(deltaTime);
                        break;
                }
            }

            protected override void SleepState(States state)
            {
                switch (state)
                {
                    case States.Disabled:
                        FsmController.EnableRotator();
                        break;
                    
                    case States.Input:
                        FsmController.Input_Disable();
                        
                        break;
                    case States.Automatic:
                        FsmController.Automatic_Disable();
                        
                        break;
                    case States.Speeder:
                        FsmController.Speeder_Disable();
                        break;
                    
                    case States.Finder:
                        FsmController.Finder_Disable();
                        break;
                }
            }
        }

        private readonly IAngularRotation _angularRotation;
        private readonly IInputRotation _inputRotation;
        private readonly IAutomaticSpeeder _speeder;
        private readonly IAutomaticInput _automaticInput;
        private readonly ITargetFinder _targetFinder;
        private readonly Controller _controller;

        #region IShieldRotator
        
        public event Action OnSpeederReachedMaxSpeed;
        public event Action OnSpeederReachedMinSpeed;
        public event Action OnFinderFinish;
        public event Action OnRotationStopped;
        public event Action OnRotationStarted;

        #endregion

        private ShieldRotationDebugData _debugData;

        public ShieldRotator(Transform objectToRotate, IShieldRotatorData data)
        {
            var detector = new TargetDetector(objectToRotate, data.DetectorData);
            
            _angularRotation = new AngularRotation(objectToRotate);
            _inputRotation = new InputRotation(data.InputRotationData);
            _speeder = new AutomaticSpeeder(objectToRotate, data.SpeederData);
            _automaticInput = new AutomaticInput(data.AutomaticInputData, detector);
            _targetFinder = new TargetFinder(data.TargetFinderData, detector);

            _controller = new Controller(this);
            _controller.OnStateChange += (lastState, newState) =>
            {
                _debugData.State = newState.ToString();
                _debugData.LastState = lastState.ToString();
            };
            TransitionToDisable();
        }

        #region IShieldRotator
        
        public void TransitionToDisable() => _controller.ChangeState(States.Disabled);
        public void TransitionToManualInput() => _controller.ChangeState(States.Input);
        public void TransitionToAutomaticInput() => _controller.ChangeState(States.Automatic);
        public void TransitionToSpeeder() => _controller.ChangeState(States.Speeder);
        public void TransitionToFinder() => _controller.ChangeState(States.Finder);

        public void CheckForTarget()
        {
            if (_controller.CurrentState != States.Automatic) return;
            
            _automaticInput.SetActive(true);
        }

        public void SlowSpeederDown() => _speeder.SpeedDown(_targetFinder.GetRotationData().MaxSpeed);
        public void SetInputAngle(float angle)
        {
            _debugData.InputAngle = angle;
            _inputRotation.SetInputAngle(angle);
        }

        public void SetInputMagnitude(float magnitude)
        {
            _debugData.InputMagnitude = magnitude;
            _inputRotation.SetInputMagnitude(magnitude);
        }

        public void RestartAngularRotation() => _angularRotation.RestartRotation();
        public void Execute(float deltaTime)
        {
            ShieldRotationDebugEvents.TriggerOnDebug(_debugData);
            _controller.Update(deltaTime);
        }

        #endregion
        
        #region IController
        
        // === Disable === //
        #region Disable
        public void EnableRotator()
        {
            _angularRotation.OnTargetReached += Rotation_OnTargetReachedHandler;
            _angularRotation.OnStartRotation += Rotation_OnStartRotationHandler;
            _angularRotation.SetEnable(true);
        }

        public void DisableRotator()
        {
            _angularRotation.OnTargetReached -= Rotation_OnTargetReachedHandler;
            _angularRotation.OnStartRotation -= Rotation_OnStartRotationHandler;
            _angularRotation.SetEnable(false);
            _inputRotation.DisableInput();
            
        }
        #endregion
        
        // === Input === //
        #region Input
        public void Input_Enable()
        {
            _inputRotation.OnInputChanged += Input_OnInputChangedHandler;
            _inputRotation.OnMagnitudeChanged += Input_OnMagnitudeChangedHandler;
            _angularRotation.SetRotationData(_inputRotation.GetRotationData());
        }

        public void Input_Disable()
        {
            _inputRotation.OnInputChanged -= Input_OnInputChangedHandler;
            _inputRotation.OnMagnitudeChanged -= Input_OnMagnitudeChangedHandler;
        }
        #endregion

        // === Automatic === //
        #region Automatic
        public void Automatic_Enable()
        {
            _automaticInput.OnTargetFound += Automatic_OnTargetFoundHandler;
            _angularRotation.SetSpeedMultiplier(1);
            _angularRotation.SetRotationData(_automaticInput.GetRotationData());
            _automaticInput.SetActive(true);
        }
        
        public void Automatic_Disable()
        {
            _automaticInput.OnTargetFound -= Automatic_OnTargetFoundHandler;
            _automaticInput.SetActive(false);
        }

        public void Automatic_Execute(float deltaTime) => _automaticInput.Execute(deltaTime);
        #endregion

        // === Speeder === // 
        #region Speeder
        public void Speeder_Enable()
        {
            _angularRotation.StopRotation();
            _angularRotation.SetEnable(false);

            _speeder.OnReachedMaxSpeed += Speeder_OnReachedMaxSpeedHandler;
            _speeder.OnReachedMinSpeed += Speeder_OnReachedMinSpeedHandler;
            _speeder.OnSpeedIncreased += Speeder_OnSpeedIncreasedHandler;
            _speeder.OnSpeedDecreased += Speeder_OnSpeedDecreasedHandler;
            
            _speeder.SpeedUp();
        }
        public void Speeder_Disable()
        {
            _speeder.OnReachedMaxSpeed -= Speeder_OnReachedMaxSpeedHandler;
            _speeder.OnReachedMinSpeed -= Speeder_OnReachedMinSpeedHandler;
            _speeder.OnSpeedIncreased -= Speeder_OnSpeedIncreasedHandler;
            _speeder.OnSpeedDecreased -= Speeder_OnSpeedDecreasedHandler;
            _angularRotation.SetEnable(true);
        }

        public void Speeder_Execute(float deltaTime) => _speeder.Execute(deltaTime);
        #endregion

        // === Finder === // 
        #region Finder
        public void Finder_Enable()
        {
            _targetFinder.OnTargetFound += Finder_OnTargetFoundHandler;
            _targetFinder.OnTargetNotFound += Finder_OnTargetNotFoundHandler;
            _angularRotation.SetRotationData(_targetFinder.GetRotationData());
            _angularRotation.SetSpeedMultiplier(1f);
        }

        public void Finder_Disable()
        {
            _targetFinder.OnTargetFound -= Finder_OnTargetFoundHandler;
            _targetFinder.OnTargetNotFound -= Finder_OnTargetNotFoundHandler;
        }

        public void Finder_TryToFindTarget()
        {
            _targetFinder.TryToFindTarget();
        }

        #endregion

        // === Rotator === //
        #region Rotator
        public void Rotator_Execute(float deltaTime) => _angularRotation.Execute(deltaTime);
        #endregion

        #endregion

        #region Handlers

        // === Disable === //

        // === Input === //
        private void Input_OnMagnitudeChangedHandler(float input)
        {
            _angularRotation.SetSpeedMultiplier(input);
        }

        private void Input_OnInputChangedHandler(float input)
        {
            _debugData.TargetAngle = input;
            _angularRotation.SetTargetAngle(input);
        }
        
        // === Automatic === //
        private void Automatic_OnTargetFoundHandler(float input)
        {
            _angularRotation.SetTargetAngle(input);
        }

        // === Speeder === // 

        #region Speeder
        private void Speeder_OnReachedMinSpeedHandler()
        {
            OnSpeederReachedMinSpeed?.Invoke();
        }
        private void Speeder_OnReachedMaxSpeedHandler()
        {
            OnSpeederReachedMaxSpeed?.Invoke();
        }
        
        private void Speeder_OnSpeedIncreasedHandler()
        {
            //TODO: Should notify 
        }
        private void Speeder_OnSpeedDecreasedHandler()
        {
            //TODO: Should notify 
        }
        #endregion

        // === Finder === // 
        private void Finder_OnTargetFoundHandler(float input)
        {
            _debugData.TargetAngle = input;
            _angularRotation.SetTargetAngle(input);
        }
        
        private void Finder_OnTargetNotFoundHandler()
        {
            OnFinderFinish?.Invoke();
        }
            
        // === Rotator === //
        private void Rotation_OnTargetReachedHandler()
        {
            switch (_controller.CurrentState)
            {
                case States.Finder:
                    OnFinderFinish?.Invoke();
                    break;
            }
            
            OnRotationStopped?.Invoke();
        }

        private void Rotation_OnStartRotationHandler()
        {
            OnRotationStarted?.Invoke(); 
        }

        #endregion
    }
}   