using System;
using _Main.Scripts.Common;
using _Main.Scripts.Contracts.Interfaces;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.So;
using MeteorMadness.Gameplay.Particles;
using MeteorMadness.GlobalValues.Tools.Observer;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.Gameplay.Shield
{
    internal class ShieldShakerController : BaseShaker, IShaker
    {
        private readonly Func<Transform> _getObjectToShakeFunc;
        private Transform _objectToShake;
        
        public ShieldShakerController(Func<Transform> objectToShakeFunc, IShakerCap capData) : base(capData)
        {
            _getObjectToShakeFunc = objectToShakeFunc;
        }

        public void Execute(float deltaTime)
        {
            if(DoesShake())
                _objectToShake.localPosition = GetShakePosition(deltaTime);
        }

        public void AddShake(ShakeData data)
        {
            _objectToShake = _getObjectToShakeFunc.Invoke();
            AddShake(data.Data,data.Direction, data.DirectionBias, data.Multiplier,_objectToShake.localPosition);
        }

        public void AddShake(IShakerData shakeData, float multiplier = 1f)
        {
            _objectToShake = _getObjectToShakeFunc.Invoke();
            AddShake(shakeData,Vector2.zero, 0f, multiplier, _objectToShake.localPosition);
        }
    }

    public class ShieldView : ManagedBehavior, 
        IObserver,
        IUpdatable, 
        IShieldSounds, 
        IAbilityShield, 
        IShieldVibration
    {
        
        [Header("Rotation Components")]
        [SerializeField] private Transform normalShieldPivot;
        [Space]
        [Header("Shale Components")]
        [SerializeField] private Transform normalShieldSprite;
        [SerializeField] private Transform superShieldSprite;
        [Space]
        [Header("Components")] 
        [SerializeField] private GameObject shieldContainer;
        [SerializeField] private CapsuleCollider2D shieldCollider;
        [SerializeField] private Collider2D superShieldCollider;

        [Space] 
        
        [Header("Sounds")] [Space] 
        [Header("Shield Shaker")] 
        [SerializeField] private ShakerCapDataSo shakerCapData;
        [SerializeField] private DirectionalShakeData deflectShakeData;
        [Header("Scriptable Objects")]
        [SerializeField] private DirectionalShakeData cameraShakeData;
        [SerializeField] private ParticleDataSo deflectParticleData;
        [Header("Movement")] 
        [SerializeField] private RotatorDataSo rotatorData;
        [SerializeField] private LayerMask projectileLayerMask;
        
        private IShaker _shakerController;
        private IShieldRotator _shieldRotator;
        private ShieldColliderExtender _colliderExtender;
        private bool _isSuperShieldSpriteActive;
        
        public event Action<bool> OnShieldActivated;
        public event Action OnRotate;
        public event Action OnStopped;
        public event Action OnDirectionChanged;
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
            shieldContainer.SetActive(false);
            superShieldCollider.enabled = false;
            _shakerController = new ShieldShakerController(GetSpriteToShake,shakerCapData);
            _colliderExtender = new ShieldColliderExtender(shieldCollider);
            _shieldRotator = new ShieldRotator(normalShieldPivot, rotatorData);
        }

        private void Start()
        {
            _shieldRotator.OnRotationStarted += Rotation_OnRotationStartedHandler;
            _shieldRotator.OnRotationStopped += Rotation_OnRotationStoppedHandler;
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _shakerController.Execute(deltaTime);
            _shieldRotator.Execute(deltaTime);
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
                case ShieldObserverMessage.ChangeMagnitude:
                    HandleChangeMagnitude((float)args[0]);
                    break;
            }
        }

        #region ObserverHandlers
        
        private void HandleSetActiveShield(bool isActive)
        {
            shieldContainer.SetActive(isActive);
            
            OnShieldActivated?.Invoke(isActive);
            if (isActive)
            {
                _shieldRotator.TransitionToManualInput();
            }
            else
            {
                _shieldRotator.TransitionToDisable();
                OnDisableAbility?.Invoke();
            }
        }
        
        private void HandleChangeMagnitude(float magnitude) => _shieldRotator.SetInputMagnitude(magnitude);
        private void HandleRotation(float inputAngle) => _shieldRotator.SetInputAngle(inputAngle);
        private void HandleRestartPosition() => _shieldRotator.RestartAngularRotation();
        private void HandleDeflect(Vector3 position, Quaternion rotation, Vector2 direction)
        {
            _shakerController.AddShake(new ShakeData
            {
                Data = deflectShakeData.Data,
                Direction = -direction,
                DirectionBias = deflectShakeData.DirectionBias
            });
            
            ParticleEventCaller.Spawn(new ParticleSpawnData
            {
                ParticleData = deflectParticleData,
                Position = position,
                Rotation = rotation,
                MoveDirection = direction
            });
            
            CameraEventCaller.Shake(new ShakeData
            {
                Data = cameraShakeData.Data,
                Direction = direction,
                DirectionBias = cameraShakeData.DirectionBias
            });
            
            OnDeflect?.Invoke();
        }
        
        private void HandleSetAutomatic(bool isActive)
        {
            OnAbilitySetActive?.Invoke(AbilityType.Automatic, isActive);
            if (isActive)
            {
                _shieldRotator.OnFinderFinish += OnAutomaticEnable;
                _shieldRotator.TransitionToFinder();
                OnAbilityRunning?.Invoke(AbilityType.Automatic);
            }
            else
            {
                OnDeflect -= _shieldRotator.CheckForTarget;
                _shieldRotator.OnFinderFinish += Rotation_OnFinderFinishHandler;
                _shieldRotator.TransitionToFinder();
                ShieldEventCaller.NotifyShieldTypeDisabled(ShieldType.Automatic);
                OnAbilityFinished?.Invoke();
            }
        }

        private void OnAutomaticEnable()
        {
            _shieldRotator.OnFinderFinish -= OnAutomaticEnable;
            OnDeflect += _shieldRotator.CheckForTarget;
            _shieldRotator.TransitionToAutomaticInput();
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
                    _shieldRotator.TransitionToSpeeder();
                }))
                .Then(new WaitForEventAction(
                    subscribe: callback => _shieldRotator.OnSpeederReachedMaxSpeed += callback,
                    unsubscribe: callback => _shieldRotator.OnSpeederReachedMaxSpeed -= callback))
                .Then(new InstantAction(()=> OnEnableSuperShield?.Invoke(targetTime)))
                .Then(new WaitSecondsAction(targetTime))
                .Then(new InstantAction(() =>
                {
                    _isSuperShieldSpriteActive = true;
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
                .Then(new InstantAction(()=> _shieldRotator.SlowSpeederDown()))
                .Then(new WaitForEventAction(
                    subscribe: callback => _shieldRotator.OnSpeederReachedMinSpeed += callback,
                    unsubscribe: callback => _shieldRotator.OnSpeederReachedMinSpeed -= callback))
                .Then(new InstantAction(()=> _shieldRotator.TransitionToFinder()))
                .Then(new WaitForEventAction(
                    subscribe: callback => _shieldRotator.OnFinderFinish += callback,
                    unsubscribe: callback => _shieldRotator.OnFinderFinish -= callback))
                .Then(new InstantAction(() =>
                {
                    _shieldRotator.TransitionToManualInput();
                    OnAbilitySetActive?.Invoke(AbilityType.SuperShield, false);
                    superShieldCollider.enabled = false;
                    _isSuperShieldSpriteActive = false;
                    OnAbilityFinished?.Invoke();
                }))
                .Then(new InstantAction(()=> ShieldEventCaller.NotifyShieldTypeDisabled(ShieldType.Super)))
                .Build();
            
            ActionManager.Add(temp,ActionManager.UpdateType.Update, ActionManager.PriorityTick.EveryFrame);
        }

        #endregion
        
        #endregion
        
        private Transform GetSpriteToShake()
        {
            return _isSuperShieldSpriteActive ? superShieldSprite : normalShieldSprite;
        }
        
        #region Handlers

        private void Rotation_OnRotationStartedHandler()
        {
            _colliderExtender.Extend();
            OnRotate?.Invoke();
        }
        
        private void Rotation_OnRotationStoppedHandler()
        {
            _colliderExtender.Retract();
            OnStopped?.Invoke();
        }
        
        private void Rotation_OnFinderFinishHandler()
        {
            _shieldRotator.OnFinderFinish -= Rotation_OnFinderFinishHandler;
            _shieldRotator.TransitionToManualInput();
        }

        #endregion
    }
}