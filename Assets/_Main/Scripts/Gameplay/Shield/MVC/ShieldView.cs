using System;
using System.Collections;
using _Main.Scripts.EventBus;
using _Main.Scripts.ShieldRotation.Contracts;
using _Main.Scripts.ShieldRotation.Mediator;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.Gameplay.Particles;
using MeteorMadness.Gameplay.Shaker;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.Gameplay.Shield
{
    public class ShieldView : ManagedBehavior, 
        IObserver,
        IUpdatable, 
        IShieldSounds, 
        IAbilityShield, 
        IShieldVibration
    {
        [Header("Components")] 
        [SerializeField] private GameObject spriteContainer;
        [SerializeField] private GameObject normalShieldSprite;
        [SerializeField] private CapsuleCollider2D shieldCollider;
        [SerializeField] private Collider2D superShieldCollider;
        [Space]
        [Header("Sounds")]
        [Space] 
        [Header("Scriptable Objects")]
        [SerializeField] private ShakeDataSo hitShakeData;
        [SerializeField] private ShakeDataSo cameraShakeData;
        [SerializeField] private ParticleDataSo deflectParticleData;
        [Header("Movement")]
        [SerializeField] private Transform normalShieldContainer;
        [SerializeField] private MediatorDataSo mediatorData;
        [SerializeField] private LayerMask projectileLayerMask;
        private IMediator _shieldMovement;
        private ComponentShaker _shakerController;
        private ShieldColliderExtender _colliderExtender;
        public event Action<bool> OnShieldActivated;
        public event Action OnRotate;
        public event Action OnStopped;
        public event Action OnStartStop;
        public event Action<int> OnDirectionChange;
        public event Action OnDeflect;
        public event Action<AbilityType> OnAbilityStarted;
        public event Action<AbilityType> OnAbilityRunning;
        public event Action OnAbilityFinished;
        public event Action<AbilityType, bool> OnAbilitySetActive;
        public event Action<float> OnEnableSuperShield;
        public event Action<float> OnDisableSuperShield;
        public event Action OnDisableAbility;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Shield;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

        private void Awake()
        {
            superShieldCollider.enabled = false;
            _shakerController = new ComponentShaker(normalShieldSprite.transform,hitShakeData);
            _colliderExtender = new ShieldColliderExtender(shieldCollider);
            _shieldMovement = new MediatorComponent(normalShieldContainer, GameParameters.GameplayValues.AngleSlots,
                mediatorData, projectileLayerMask);
        }

        private void Start()
        {
            _shieldMovement.OnStartMoving += Movement_OnStartMovingHandler;
            _shieldMovement.OnStartStop += Movement_OnStartStopHandler;
            _shieldMovement.OnDirectionChange += Movement_OnDirectionChangeHandler;
            _shieldMovement.OnStopped += Movement_OnStoppedHandler;
            _shieldMovement.OnStartSnapping += Movement_OnStartSnappingHandler;
            _shieldMovement.OnStopSnapping += Movement_OnStopSnappingHandler;
            _shieldMovement.OnSpeedIncreased += Movement_OnSpeedIncreasedHandler;
            _shieldMovement.OnSpeedDecreased += Movement_OnSpeedDecreasedHandler;
            _shieldMovement.OnReachedMaxSpeed += Movement_OnReachedMaxSpeedHandler;
            _shieldMovement.OnReachedMinSpeed += Movement_OnReachedMinSpeedHandler;
            _shieldMovement.OnSnapping += Movement_OnSnappingHandler;
            _shieldMovement.OnSnapped += Movement_OnSnappedHandler;
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _shieldMovement?.Update(deltaTime);
        }
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case ShieldObserverMessage.Rotate:
                    HandleRotation((float)args[0]);
                    break;
                case ShieldObserverMessage.Deflect:
                    HandleDeflect(
                        (Vector3)args[0],
                        (Quaternion)args[1],
                        (Vector2)args[2]);
                    break;
                case ShieldObserverMessage.SetActiveShield:
                    HandleSetActiveShield((bool)args[0]);
                    break;
                case ShieldObserverMessage.SetActiveSuperShield:
                    HandleSetSuperActive((bool)args[0]);
                    break;
                case ShieldObserverMessage.SetGold:
                    HandleSetGold((bool)args[0]);
                    break;
                case ShieldObserverMessage.SetAutomatic:
                    HandleSetAutomatic((bool)args[0]);
                    break;
                case ShieldObserverMessage.RestartPosition:
                    HandleRestartPosition();
                    break;
                case ShieldObserverMessage.SetSlow:
                    HandleSetSlow((bool)args[0]);
                    break;
            }
        }

        #region ObserverHandlers

        private void HandleSetAutomatic(bool isActive)
        {
            OnAbilitySetActive?.Invoke(AbilityType.Automatic, isActive);
            if (isActive)
            {
                _shieldMovement.OnStopSnapping += AutomaticTargetFound;
                _shieldMovement.EnableAutomatic();
                OnAbilityRunning?.Invoke(AbilityType.Automatic);
            }
            else
            {
                _shieldMovement.DisableAutomatic();
                ShieldEventCaller.NotifyShieldTypeDisabled(ShieldType.Automatic);
                OnAbilityFinished?.Invoke();
            }
        }

        private void AutomaticTargetFound()
        {
            _shieldMovement.OnStopSnapping -= AutomaticTargetFound;
            ShieldEventCaller.NotifyShieldTypeEnabled(ShieldType.Automatic);
        }

        private void HandleSetGold(bool isActive)
        {
            OnAbilitySetActive?.Invoke(AbilityType.DoublePoints, isActive);
            
            if (isActive)
            {
                ShieldEventCaller.NotifyShieldTypeEnabled(ShieldType.Gold);
                OnAbilityRunning?.Invoke(AbilityType.DoublePoints);
            }
            else
            {
                ShieldEventCaller.NotifyShieldTypeDisabled(ShieldType.Gold);
                OnAbilityFinished?.Invoke();
            }
        }
        private void HandleSetSlow(bool isActive)
        {
            OnAbilitySetActive?.Invoke(AbilityType.SlowMotion, isActive);
            
            if (isActive)
            {
                ShieldEventCaller.NotifyShieldTypeEnabled(ShieldType.Slow);
                OnAbilityRunning?.Invoke(AbilityType.SlowMotion);
            }
            else
            {
                ShieldEventCaller.NotifyShieldTypeDisabled(ShieldType.Slow);
                OnAbilityFinished?.Invoke();
            }
        }
        
        private void HandleSetActiveShield(bool isActive)
        {
            spriteContainer.SetActive(isActive);
            
            if (isActive)
                _shieldMovement.Enable();
            else
                _shieldMovement.Disable();
            
            OnShieldActivated?.Invoke(isActive);
            if (isActive == false)
            {
                OnDisableAbility?.Invoke();
            }
        }
        
        private void HandleRotation(float direction)
        {
            _shieldMovement.SetInputDirection(direction);
        }
        
        private void HandleRestartPosition()
        {
            //_shieldMovement.RestartPosition();
        }
        
        private void HandleDeflect(Vector3 position, Quaternion rotation, Vector2 direction)
        {
            StartCoroutine(Coroutine_Shake());
            
            ParticleEventCaller.Spawn(new ParticleSpawnData
            {
                ParticleData = deflectParticleData,
                Position = position,
                Rotation = rotation,
                MoveDirection = direction
            });
            
            CameraEventCaller.Shake(cameraShakeData);
            OnDeflect?.Invoke();
        }

        #endregion
        
        #region Change Form

        #region SuperShield
        
        private void HandleSetSuperActive(bool isActive)
        {
            if (isActive)
            {
                RunSuperShieldQueue();
            }
            else
            {
                RunNormalShieldQueue();
            }
        }
        
        private void RunSuperShieldQueue()
        {
            var targetTime = 0.75f;
            
            var temp = ActionBuilder.Start()
                .Do(new InstantAction(() =>
                {
                    OnAbilityStarted?.Invoke(AbilityType.SuperShield);
                    OnAbilitySetActive?.Invoke(AbilityType.SuperShield, true); 
                    superShieldCollider.enabled = true;
                    _shieldMovement.SpeedUp();
                }))
                .Then(new WaitForEventAction(
                    subscribe: callback => _shieldMovement.OnReachedMaxSpeed += callback,
                    unsubscribe: callback => _shieldMovement.OnReachedMaxSpeed -= callback))
                .Then(new InstantAction(()=> OnEnableSuperShield?.Invoke(targetTime)))
                .Then(new WaitSecondsAction(targetTime))
                .Then(new InstantAction(() =>
                {
                    OnAbilityRunning?.Invoke(AbilityType.SuperShield);
                }))
                .Then(new InstantAction(()=> ShieldEventCaller.NotifyShieldTypeEnabled(ShieldType.Super)))
                .Build();
            
            ActionManager.Add(temp,ActionManager.UpdateType.Update ,ActionManager.PriorityTick.EveryFrame);
        }
        
        private void RunNormalShieldQueue()
        {
            var targetTime = 0.5f;
            
            var temp = ActionBuilder.Start()
                .Do(new InstantAction(()=> OnDisableSuperShield?.Invoke(targetTime)))
                .Then(new InstantAction(()=> _shieldMovement.SlowDown()))
                .Then(new WaitForEventAction(
                    subscribe: callback => _shieldMovement.OnReachedMinSpeed += callback,
                    unsubscribe: callback => _shieldMovement.OnReachedMinSpeed -= callback))
                .Then(new InstantAction(()=> _shieldMovement.TryToSnapToTarget()))
                .Then(new WaitForEventAction(
                    subscribe: callback => _shieldMovement.OnSnapped += callback,
                    unsubscribe: callback => _shieldMovement.OnSnapped -= callback))
                .Then(new InstantAction(() =>
                {
                    OnAbilitySetActive?.Invoke(AbilityType.SuperShield, false);
                    superShieldCollider.enabled = false;
                    OnAbilityFinished?.Invoke();
                }))
                .Then(new InstantAction(()=> ShieldEventCaller.NotifyShieldTypeDisabled(ShieldType.Super)))
                .Build();
            
            ActionManager.Add(temp,ActionManager.UpdateType.Update, ActionManager.PriorityTick.EveryFrame);
        }

        #endregion
        
        #endregion

        #region Coroutine
        
        private IEnumerator Coroutine_Shake()
        {
            _shakerController.StartShake();
            
            while (_shakerController.IsShaking == true)
            {
                _shakerController.HandleShake(CustomTime.GetDeltaTimeByChannel(UpdateGroup.Shield));
                
                yield return null;
            }
        }

        #endregion

        #region Handlers

        private void Movement_OnSnappedHandler()
        {
            
        }

        private void Movement_OnSnappingHandler()
        {

        }

        private void Movement_OnReachedMinSpeedHandler()
        {

        }

        private void Movement_OnReachedMaxSpeedHandler()
        {

        }

        private void Movement_OnSpeedDecreasedHandler()
        {

        }

        private void Movement_OnSpeedIncreasedHandler()
        {

        }

        private void Movement_OnStopSnappingHandler()
        {

        }

        private void Movement_OnStartSnappingHandler()
        {

        }

        private void Movement_OnStoppedHandler()
        {
            _colliderExtender.Retract();
            OnStopped?.Invoke();
        }

        private void Movement_OnDirectionChangeHandler(int direction)
        {
            OnDirectionChange?.Invoke(direction);
            ShieldEventCaller.NotifyMovement(direction);
        }

        private void Movement_OnStartStopHandler()
        {
            OnStartStop?.Invoke();
            ShieldEventCaller.NotifyMovement(0);
        }

        private void Movement_OnStartMovingHandler(int direction)
        {
            _colliderExtender.Extend();
            ShieldEventCaller.NotifyMovement(direction);
            OnRotate?.Invoke();
        }

        #endregion
    }
}