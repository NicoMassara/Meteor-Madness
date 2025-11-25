using System;
using System.Collections;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Observer;
using _Main.Scripts.Shaker;
using _Main.Scripts.ScriptableObjects;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    [RequireComponent(typeof(ShieldMovement))]
    public class ShieldView : ManagedBehavior, IObserver, IShieldSounds, IAbilityShield
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
        [SerializeField] private ShieldMovementDataSo movementData;
        
        private ShieldMovement _movement;
        private ShakerController _shakerController;
        private ShieldColliderExtender _colliderExtender;
        public event Action<bool> OnShieldActivated;
        public event Action OnRotate;
        public event Action OnDeflect;
        public event Action<AbilityType> OnAbilityStarted;
        public event Action<AbilityType> OnAbilityRunning;
        public event Action OnAbilityFinished;
        public event Action<AbilityType, bool> OnAbilitySetActive;
        public event Action<float> OnEnableSuperShield;
        public event Action<float> OnDisableSuperShield;
        public event Action OnDisableAbility;

#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private ShieldDebugData _debugData;
        
#endif
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Shield;

        private void Awake()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData = new ShieldDebugData();
        
#endif
            superShieldCollider.enabled = false;
            _movement = GetComponent<ShieldMovement>();
            _shakerController = new ShakerController(normalShieldSprite.transform,hitShakeData);
            _colliderExtender = new ShieldColliderExtender(shieldCollider);
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
                case ShieldObserverMessage.StopRotate:
                    HandleStopRotate();
                    break;
                case ShieldObserverMessage.ChangedDirection:
                    HandleChangedDirection();
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
            _movement.SetAutomaticEnable(isActive);
            OnAbilitySetActive?.Invoke(AbilityType.Automatic, isActive);

            if (isActive)
            {
                OnAbilityRunning?.Invoke(AbilityType.Automatic);
            }
            else
            {
                OnAbilityFinished?.Invoke();
            }
        }

        private void HandleSetGold(bool isActive)
        {
            OnAbilitySetActive?.Invoke(AbilityType.DoublePoints, isActive);
            
            if (isActive)
            {
                OnAbilityRunning?.Invoke(AbilityType.DoublePoints);
            }
            else
            {
                OnAbilityFinished?.Invoke();
            }
        }
        private void HandleSetSlow(bool isActive)
        {
            OnAbilitySetActive?.Invoke(AbilityType.SlowMotion, isActive);
            
            if (isActive)
            {
                OnAbilityRunning?.Invoke(AbilityType.SlowMotion);
            }
            else
            {
                OnAbilityFinished?.Invoke();
            }
        }
        
        private void HandleSetActiveShield(bool isActive)
        {
            spriteContainer.SetActive(isActive);
            OnShieldActivated?.Invoke(isActive);
            if (isActive == false)
            {
                OnDisableAbility?.Invoke();
            }
        }
        
        private void HandleRotation(float direction)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.Rotation = direction;
#endif
            
            if (_movement.TryRotate((int)direction))
            {
                _colliderExtender.Extend();
            }
        }
        
        private void HandleStopRotate()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.Rotation = 0;
#endif
            
            if (_movement.TryForceStop())
            {
                _colliderExtender.Retract();
            }
        }
        
        private void HandleChangedDirection()
        {
            OnRotate?.Invoke();
        }
        
        private void HandleRestartPosition()
        {
            _movement.RestartPosition();
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
                    OnEnableSuperShield?.Invoke(targetTime);
                    CustomTime.SetChannelTimeScale(UpdateGroup.Ability, 0);
                }))
                .Then(new TimedUpdateAction(HandleSuperShieldEnable, targetTime))
                .Then(new InstantAction(() =>
                {
                    _movement.RestartSpeedValues();
                    CustomTime.SetChannelTimeScale(UpdateGroup.Ability, 1);
                    OnAbilityRunning?.Invoke(AbilityType.SuperShield);
                }))
                .Build();
            
            ActionManager.Add(temp, PriorityTick.High);
        }
        
        private void RunNormalShieldQueue()
        {
            var targetTime = 0.75f;
            
            var temp = ActionBuilder.Start()
                .Do(new InstantAction(() =>
                {
                    CustomTime.SetChannelTimeScale(UpdateGroup.Ability, 0);
                    OnDisableSuperShield?.Invoke(targetTime);
                }))
                .Then(new TimedUpdateAction(HandleNormalShieldEnable, targetTime))
                .Then(new InstantAction(() =>
                {
                    CustomTime.SetChannelTimeScale(UpdateGroup.Ability, 1);
                    OnAbilitySetActive?.Invoke(AbilityType.SuperShield, false);
                    superShieldCollider.enabled = false;
                    _movement.RestartSpeedValues();
                    _movement.RotateTowardsNearestProjectileSlot();
                    OnAbilityFinished?.Invoke();
                }))
                .Build();
            
            ActionManager.Add(temp, PriorityTick.High);
        }
        
        private void HandleSuperShieldEnable(float deltaTime)
        {
            _movement.IncreaseSpeed(deltaTime);
        }

        private void HandleNormalShieldEnable(float deltaTime)
        {
            _movement.DecreaseSpeed(deltaTime);
        }
        
        #endregion

        #region Coroutine
        
        private IEnumerator Coroutine_Shake()
        {
            _shakerController.StartShake();
            
            while (_shakerController.IsShaking == true)
            {
                _shakerController.HandleShake(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
                
                yield return null;
            }
        }

        #endregion
    }
}