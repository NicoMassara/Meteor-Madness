using System;
using System.Collections;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using _Main.Scripts.Shaker;
using _Main.Scripts.ScriptableObjects;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    [RequireComponent(typeof(ShieldMovement))]
    [RequireComponent(typeof(ShieldAppereance))]
    public class ShieldView : ManagedBehavior, IObserver
    {
        [Header("Components")] 
        [SerializeField] private GameObject spriteContainer;
        [SerializeField] private GameObject normalShieldSprite;
        [SerializeField] private CapsuleCollider2D shieldCollider;
        [Space]
        [Header("Sounds")]

        [Space] 
        [Header("Scriptable Objects")]
        [SerializeField] private ShakeDataSo hitShakeData;
        [SerializeField] private ShakeDataSo cameraShakeData;
        [SerializeField] private ParticleDataSo deflectParticleData;
        [SerializeField] private ShieldMovementDataSo movementData;

        private ShieldAppereance _appereance;
        private ShieldMovement _movement;
        private ShakerController _shakerController;
        private ShieldColliderExtender _colliderExtender;
        public event Action<bool> OnShieldActivated;
        public event Action OnRotate;
        public event Action OnDeflect;
        public event Action<AbilityType> OnAbilityStarted;
        public event Action<AbilityType> OnAbilityRunning;
        public event Action OnAbilityFinished;

#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private ShieldDebugData _debugData;
        
#endif
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Shield;

        private void Awake()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData = new ShieldDebugData();
        
#endif
            
            _movement = GetComponent<ShieldMovement>();
            _appereance = GetComponent<ShieldAppereance>();
            
            _shakerController = new ShakerController(normalShieldSprite.transform,hitShakeData);
            _colliderExtender = new ShieldColliderExtender(shieldCollider);
        }

        private void Start()
        {
            _appereance.SetActiveSuperShieldSprite(false);;
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
            _appereance.SetAutomaticEnable(isActive);

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
            _appereance.SetGoldEnable(isActive);
            
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
            _appereance.SetSlowEnable(isActive);
            
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
        
        private IEnumerator Coroutine_RunActionByTime(Action<float> action, float targetTime)
        {
            var elapsedTime = 0f;
            
            while (elapsedTime < targetTime)
            {
                var deltaTime = CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup);
                elapsedTime += deltaTime;
                action?.Invoke(deltaTime);
                
                yield return null;
            }
        }
        
        private void RunSuperShieldQueue()
        {
            var temp = ActionBuilder.Start()
                .Do(new InstantAction(() =>
                {
                    OnAbilityStarted?.Invoke(AbilityType.SuperShield);
                    _appereance.SetActiveSuperShieldSprite(true);
                    CustomTime.SetChannelTimeScale(UpdateGroup.Ability, 0);
                }))
                .Then(new TimedUpdateAction(HandleSuperShieldEnable, _appereance.TimeToEnableSuperShield))
                .Then(new InstantAction(() =>
                {
                    _movement.RestartSpeedValues();
                    _appereance.RestartValues();
                    CustomTime.SetChannelTimeScale(UpdateGroup.Ability, 1);
                    OnAbilityRunning?.Invoke(AbilityType.SuperShield);
                }))
                .Build();
            
            ActionManager.Add(temp, PriorityTick.High);
        }
        
        private void RunNormalShieldQueue()
        {
            var temp = ActionBuilder.Start()
                .Do(new InstantAction(() =>
                {
                    CustomTime.SetChannelTimeScale(UpdateGroup.Ability, 0);
                }))
                .Then(new TimedUpdateAction(HandleNormalShieldEnable, _appereance.TimeToDisableSuperShield))
                .Then(new InstantAction(() =>
                {
                    CustomTime.SetChannelTimeScale(UpdateGroup.Ability, 1);
                    _appereance.SetActiveSuperShieldSprite(false);
                    _appereance.RestartValues();
                    _movement.RestartSpeedValues();
                    _movement.RotateTowardsNearestProjectileSlot();
                    OnAbilityFinished?.Invoke();
                }))
                .Build();
            
            ActionManager.Add(temp, PriorityTick.High);
        }
        
        private void HandleSuperShieldEnable(float deltaTime)
        {
            _appereance.EnableSuperShield(deltaTime);
            _movement.IncreaseSpeed(deltaTime);
        }

        private void HandleNormalShieldEnable(float deltaTime)
        {
            _appereance.EnableNormalShield(deltaTime);
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