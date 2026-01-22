using System;
using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.Movement;
using _Main.Scripts.ShieldRotation.AutomaticMovement;
using _Main.Scripts.ShieldRotation.Contracts;
using _Main.Scripts.ShieldRotation.ProjectileDetector;
using _Main.Scripts.ShieldRotation.RotationSpeeder;
using _Main.Scripts.ShieldRotation.TargetSnapper;
using _Main.Scripts.ShieldRotation.Tools;
using UnityEngine;
using IMovement = _Main.Scripts.Movement.IMovement;

namespace _Main.Scripts.ShieldRotation.Mediator
{
    public class MediatorComponent : IMediator,
        MediatorComponent.IFsmMediator
    {
        #region Fsm

        #region Base
        
        private enum States
        {
            Disable,
            Inputs,
            Automatic,
            Speeder,
            TargetSnapping,
        }
        
        private interface IFsmMediator
        {
            // === Components Update === //
            public void UpdateInput(float deltaTime);
            public void UpdateAutomatic(float deltaTime);
            public void UpdateSpeeder(float deltaTime);
            public void UpdateTargeting(float deltaTime);
            
            // === Transitions === // 
            public void TransitionToDisable();
            public void TransitionToInputs();
            public void TransitionToAutomatic();
            public void TransitionToSpeeder();
            public void TransitionToTargetSnapping();
        }
        
        private abstract class MediatorBaseState : StateBase<IFsmMediator> { }

        #endregion

        #region States

        private class DisableState : MediatorBaseState
        {
            
        }

        private class InputsState : MediatorBaseState
        {
            public override void Execute(float deltaTime)
            {
                Controller.UpdateInput(deltaTime);
            }
        }

        private class AutomaticState : MediatorBaseState
        {
            public override void Execute(float deltaTime)
            {
                Controller.UpdateAutomatic(deltaTime);
            }
        }

        private class SpeederState : MediatorBaseState
        {
            public override void Execute(float deltaTime)
            {
                Controller.UpdateSpeeder(deltaTime);
            }
        }

        private class TargetingState : MediatorBaseState
        {
            public override void Execute(float deltaTime)
            {
                Controller.UpdateTargeting(deltaTime);
            }
        }

        #endregion

        #region Controller

        private class MediatorFsmController : FsmController<States>
        {
            public MediatorFsmController(IFsmMediator mediator)
            {
                Initialize(mediator);
            }

            private void Initialize(IFsmMediator mediator)
            {
                var tempList = new List<StateData>
                {
                    new (States.Disable, new DisableState()),
                    new (States.Inputs, new InputsState()),
                    new (States.Automatic, new AutomaticState()),
                    new (States.Speeder, new SpeederState()),
                    new (States.TargetSnapping, new TargetingState()),
                };

                foreach (var state in tempList.Select(item => (MediatorBaseState)item.State))
                {
                    state.InitializeState(mediator);
                }
                
                InitializeStates(tempList);
            }
        }

        #endregion

        #endregion

        private readonly MediatorFsmController _fsmController;
        private readonly Transform _objectToRotate;
        private readonly IMediatorData _mediatorData;
        
        // === Movement === //
        private readonly IMovement _inputMovement;
        private readonly IAutomaticMovement _automaticMovement;
        private readonly IRotationSpeeder _rotationSpeeder;
        private readonly ITargetSnapper _targetSnapper;
        
        // === Projectile Detector === //
        private readonly IProjectileDetector _projectileDetector;
        
        // === Values === // 
        private int _angleSlots;
        private bool _shouldStopAutomatic;
        private ITargetable _currentTarget;
        
        // === Events === //
        public event Action OnDirectionChanged;
        public event Action OnStopped;  
        public event Action OnMoved;  
        public event Action OnStartSnapping;  
        public event Action OnStopSnapping;  
        public event Action OnSpeedIncreased;  
        public event Action OnSpeedDecreased; 
        public event Action OnReachedMaxSpeed; 
        public event Action OnReachedMinSpeed; 
        public event Action OnSnapping;  
        public event Action OnSnapped;  

        public MediatorComponent(Transform objectToRotate, int angleSlots, IMediatorData mediatorData, LayerMask projectileLayerMask)
        {
            // / / / / / / / / / / / / / / / //
            // === Components Initialize === // 
            // / / / / / / / / / / / / / / / //
            
            _mediatorData = mediatorData;
            _objectToRotate = objectToRotate;
            
            _inputMovement = new MovementComponent(objectToRotate, _mediatorData.MovementData);
            _automaticMovement = new AutomaticMovementComponent(objectToRotate, _mediatorData.AutomaticData, angleSlots);
            _rotationSpeeder = new RotationSpeederController(_mediatorData.SpeederData, objectToRotate);
            _targetSnapper = new TargetSnapperComponent(objectToRotate,_mediatorData.SnapperData, angleSlots);
            
            _projectileDetector = new ProjectileDetectorComponent(projectileLayerMask);
            
            // / / / / / / / / / / / / / //
            // === Components Events === // 
            // / / / / / / / / / / / / / //
            
            // === IMovement === //
            _inputMovement.OnMoved += IMovement_OnMovedHandler;
            _inputMovement.OnDirectionChanged += IMovement_OnDirectionChanged;
            _inputMovement.OnStopped += IMovement_OnStoppedHandler;
            
            // === IAutomaticMovement === //
            _automaticMovement.OnStartSnapping += IAutomaticMovement_OnStartSnappingHandler;
            _automaticMovement.OnStopSnapping += IAutomaticMovement_OnStopSnappingHandler;
            _automaticMovement.OnCheckForTarget += IAutomaticMovement_OnCheckForTargetHandler;
            
            // === IRotationSpeeder === //
            _rotationSpeeder.OnSpeedIncreased += IRotationSpeeder_OnSpeedIncreasedHandler;
            _rotationSpeeder.OnSpeedDecreased += IRotationSpeeder_OnSpeedDecreasedHandler;
            _rotationSpeeder.OnReachedMaxSpeed += IRotationSpeeder_OnReachedMaxSpeedHandler;
            _rotationSpeeder.OnReachedMinSpeed += IRotationSpeeder_OnReachedMinSpeedHandler;

            // === ITargetSnapper === //
            _targetSnapper.OnSnapped += ITargetSnapper_OnSnappedHandler;
            _targetSnapper.OnSnapping += ITargetSnapper_OnSnappingHandler;

            // / / / / / / / / / / / / / //
            // ===       FSM         === // 
            // / / / / / / / / / / / / / //
            
            _fsmController = new MediatorFsmController(this);
            Disable();
        }

        #region Private Methods

        private bool CheckForTargetToSnapper()
        {
            if (SetTarget() == false) return false;
            
            var targetSlot = GetAngleSlotFromTarget(_currentTarget);
            _targetSnapper.SetTargetSlot(targetSlot);
            return true;
        }

        private bool TrySetTargetToAutomatic()
        {
            if (SetTarget() == false)
            {
                return false;
            }

            var targetSlot = GetAngleSlotFromTarget(_currentTarget);
            _automaticMovement.SetTargetAngle(targetSlot);
            
            return true;
        }

        private bool SetTarget()
        {
            _currentTarget = _projectileDetector.GetNearestTarget(_objectToRotate.position);
            if (_currentTarget == null)
            {
                return false;
            }

            _currentTarget.OnTargetDeath += Target_OnDeath;
            
            return true;
        }

        private void ClearTarget()
        {
            if(_currentTarget == null) return;
            
            _currentTarget.OnTargetDeath -= Target_OnDeath;
            _currentTarget.EnableTargetable();
            _currentTarget = null;
        }
        
        private int GetAngleSlotFromTarget(ITargetable target)
        {
            if (target == null) return -1;
            
            return AngleHelper.GetAngleSlotFromPosition(
                _currentTarget.Position, 
                _objectToRotate.position, 
                _angleSlots, 0f);
        }

        #endregion
        
        #region IMediator
        
        public void Update(float deltaTime)
        {
            _fsmController?.Execute(deltaTime);
        }

        #region Inputs

        public void Enable()
        {
            _fsmController.Transition(States.Inputs);
        }

        public void Disable()
        {
            _fsmController.Transition(States.Disable);
            _inputMovement.ForceStop();
        }

        public void SetInputDirection(float inputAngle)
        {
            _inputMovement.SetInputAngle(inputAngle);
        }
        
        public void SetInputMagnitude(float magnitude)
        {
            _inputMovement.SetInputMagnitude(magnitude);
        }

        #endregion
        
        #region Speeder

        public void SpeedUp()
        {
            TransitionToSpeeder();
            _rotationSpeeder.SpeedUp();
        }

        public void SlowDown()
        {
            _rotationSpeeder.SlowDown(_targetSnapper.StartVelocity);
        }

        #endregion

        #region Automatic

        public void EnableAutomatic()
        {
            ClearTarget();
            TransitionToAutomatic();
            _automaticMovement.EnableCheck();
        }

        public void DisableAutomatic()
        {
            _shouldStopAutomatic = true;
        }

        #endregion

        #region Target Snapper

        public void TryToSnapToTarget()
        {
            var hasTargetToSnap = CheckForTargetToSnapper();
            
            if(hasTargetToSnap)
                TransitionToTargetSnapping();
            else
            {
                OnSnapped?.Invoke();
                TransitionToInputs();
            }

        }

        #endregion
        
        #endregion

        #region IFsmMediator

        // === Components Update === //
        #region Components Update
        public void UpdateInput(float deltaTime) => _inputMovement?.Update(deltaTime);
        public void UpdateAutomatic(float deltaTime) => _automaticMovement?.Update(deltaTime);
        public void UpdateSpeeder(float deltaTime) => _rotationSpeeder?.Update(deltaTime);
        public void UpdateTargeting(float deltaTime) => _targetSnapper?.Update(deltaTime);

        #endregion

        // === Transitions === // 
        #region Transitions

        public void TransitionToDisable() => _fsmController.Transition(States.Disable);
        public void TransitionToInputs() => _fsmController.Transition(States.Inputs);
        public void TransitionToAutomatic() => _fsmController.Transition(States.Automatic);
        public void TransitionToSpeeder() => _fsmController.Transition(States.Speeder);
        public void TransitionToTargetSnapping() => _fsmController.Transition(States.TargetSnapping);

        #endregion

        #endregion
        
        #region Handlers

        // === Target === //
        private void Target_OnDeath(ITargetable targetable)
        {
            Debug.Log("Target_OnDeath");
            targetable.OnTargetDeath -= Target_OnDeath;
            _currentTarget = null;
            
            var currentState = _fsmController.GetCurrentState();
            switch (currentState)
            {
                case States.Automatic:
                    _automaticMovement.EnableCheck();
                    break;
                case States.TargetSnapping:
                    ITargetSnapper_OnSnappedHandler();
                    break;
            }
        }
        
        // === IMovement === //
        #region IMovement
        
        private void IMovement_OnMovedHandler()
        {
            OnMoved?.Invoke();
        }
        
        private void IMovement_OnDirectionChanged()
        {
            OnDirectionChanged?.Invoke();
        }
        
        private void IMovement_OnStoppedHandler()
        {
            OnStopped?.Invoke();
        }

        #endregion
        
        // === IAutomaticMovement === //
        #region IAutomaticMovement

        private void IAutomaticMovement_OnStopSnappingHandler()
        {
            if (_shouldStopAutomatic)
            {
                TransitionToInputs();
                _shouldStopAutomatic = false;
            }
            
            OnStopSnapping?.Invoke();
        }

        private void IAutomaticMovement_OnStartSnappingHandler()
        {
            OnStartSnapping?.Invoke();
        }
        
        private void IAutomaticMovement_OnCheckForTargetHandler()
        {
            Debug.Log("IAutomaticMovement_OnCheckForTargetHandler");
            TrySetTargetToAutomatic(); 
        }
        
        #endregion
        
        // === IRotationSpeeder === //
        #region IRotationSpeeder

        private void IRotationSpeeder_OnReachedMinSpeedHandler()
        {
            OnReachedMinSpeed?.Invoke();
        }

        private void IRotationSpeeder_OnReachedMaxSpeedHandler()
        {
            OnReachedMaxSpeed?.Invoke();
        }

        private void IRotationSpeeder_OnSpeedDecreasedHandler()
        {
            OnSpeedDecreased?.Invoke();
        }

        private void IRotationSpeeder_OnSpeedIncreasedHandler()
        {
            OnSpeedIncreased?.Invoke();
        }

        #endregion
        
        // === ITargetSnapper === //
        #region ITargetSnapper

        private void ITargetSnapper_OnSnappingHandler()
        {
            OnSnapping?.Invoke();
        }

        private void ITargetSnapper_OnSnappedHandler()
        {
            OnSnapped?.Invoke();
            TransitionToInputs();
        }

        #endregion

        #endregion
    }
}