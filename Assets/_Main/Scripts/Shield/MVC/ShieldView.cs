using System;
using System.Collections;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Interfaces.Vibration;
using _Main.Scripts.Observer;
using _Main.Scripts.Particles;
using _Main.Scripts.Shaker;
using _Main.Scripts.GameConfig.Game;
using _Main.Scripts.GlobalEvents;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Shield
{
    [RequireComponent(typeof(ShieldMovement))]
    public class ShieldView : ManagedBehavior, IObserver, IShieldSounds, IAbilityShield, IShieldVibration
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
        
        private ShieldMovement _movement;
        private ComponentShaker _shakerController;
        private ShieldColliderExtender _colliderExtender;
        public event Action<bool> OnShieldActivated;
        public event Action OnRotate;
        public event Action OnStopped;
        public event Action OnDirectionChange;
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
            _shakerController = new ComponentShaker(normalShieldSprite.transform,hitShakeData);
            _colliderExtender = new ShieldColliderExtender(shieldCollider);

            _movement.OnStopped += () =>
            {
                OnStopped?.Invoke();
            };
            
            _movement.OnDirectionChange += () =>
            {
                OnDirectionChange?.Invoke();
            };
            
            _movement.OnStartMoving += () =>
            {
                OnRotate?.Invoke();
            };
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
                ShieldEventCaller.NotifyShieldTypeEnabled(ShieldType.Automatic);
                OnAbilityRunning?.Invoke(AbilityType.Automatic);
            }
            else
            {
                ShieldEventCaller.NotifyShieldTypeDisabled(ShieldType.Automatic);
                OnAbilityFinished?.Invoke();
            }
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
            
            _movement.ForceStop();
            _colliderExtender.Retract();
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

            var speeder = _movement.ShieldSpeeder;
            
            var temp = ActionBuilder.Start()
                .Do(new InstantAction(() =>
                {
                    OnAbilityStarted?.Invoke(AbilityType.SuperShield);
                    OnAbilitySetActive?.Invoke(AbilityType.SuperShield, true); 
                    superShieldCollider.enabled = true;
                    _movement.IncreaseSpeed();
                }))
                .Then(new WaitForEventUpdateAction(speeder.UpdateSpeed, 
                    subscribe: callback => speeder.OnSpeedIncreased += callback,
                    unsubscribe: callback => speeder.OnSpeedIncreased -= callback))
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
            var speeder = _movement.ShieldSpeeder;
            
            var temp = ActionBuilder.Start()
                .Do(new InstantAction(()=> OnDisableSuperShield?.Invoke(targetTime)))
                .Then(new InstantAction(()=> _movement.DecreaseSpeed()))
                .Then(new WaitForEventUpdateAction(speeder.UpdateSpeed, 
                    subscribe: callback => speeder.OnSpeedDecreased += callback,
                    unsubscribe: callback => speeder.OnSpeedDecreased -= callback))
                .Then(new InstantAction(()=> _movement.RotateTowardsNearestProjectileSlot()))
                .Then(new WaitForEventAction(
                    subscribe: callback => _movement.OnProjectileDetected += callback,
                    unsubscribe: callback => _movement.OnProjectileDetected -= callback))
                .Then(new InstantAction(() =>
                {
                    OnAbilitySetActive?.Invoke(AbilityType.SuperShield, false);
                    superShieldCollider.enabled = false;
                    _movement.RestartSpeedValues();
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
                _shakerController.HandleShake(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
                
                yield return null;
            }
        }

        #endregion
    }
}