using System;
using System.Collections;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Observer;
using _Main.Scripts.Shaker;
using _Main.Scripts.Sounds;
using _Main.Scripts.Gameplay.AutoTarget;
using _Main.Scripts.ScriptableObjects;
using _Main.Scripts.Utilities;
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
        [SerializeField] private SoundBehavior hitSound;
        [SerializeField] private SoundBehavior moveSound;
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
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Shield;

        private void Awake()
        {
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
                case ShieldObserverMessage.PlayMoveSound:
                    HandlePlayMoveSound();
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
        }

        private void HandleSetGold(bool isActive)
        {
            _appereance.SetGoldEnable(isActive);
        }
        private void HandleSetSlow(bool isActive)
        {
            _appereance.SetSlowEnable(isActive);
        }
        
        private void HandleSetActiveShield(bool isActive)
        {
            spriteContainer.SetActive(isActive);
        }
        
        private void HandleRotation(float direction)
        {
            if (_movement.TryRotate((int)direction))
            {
                _colliderExtender.Extend();
            }
        }
        
        private void HandleStopRotate()
        {
            if (_movement.TryForceStop())
            {
                _colliderExtender.Retract();
            }
        }
        
        private void HandlePlayMoveSound()
        {
            moveSound?.PlaySound();
        }
        
        private void HandleRestartPosition()
        {
            _movement.RestartPosition();
        }
        
        private void HandleDeflect(Vector3 position, Quaternion rotation, Vector2 direction)
        {
            hitSound?.PlaySound();
            
            StartCoroutine(Coroutine_Shake());
            
            ParticleEventCaller.Spawn(new ParticleSpawnData
            {
                ParticleData = deflectParticleData,
                Position = position,
                Rotation = rotation,
                MoveDirection = direction
            });
            
            CameraEventCaller.Shake(cameraShakeData);
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
            var actionData = new ActionData[]
            {
                new(() =>
                {
                    //Debug.Log("Ability Time Scale Set to 0");
                    _appereance.SetActiveSuperShieldSprite(true);
                    CustomTime.SetChannelTimeScale(UpdateGroup.Ability, 0);
                    StartCoroutine(Coroutine_RunActionByTime(HandleSuperShieldEnable, _appereance.TimeToEnableSuperShield));
                }),
                new(() =>
                {
                    //Debug.Log("Ability Time Scale Set to 1");
                    _movement.RestartSpeedValues();
                    _appereance.RestartValues();
                    CustomTime.SetChannelTimeScale(UpdateGroup.Ability, 1);
                },_appereance.TimeToEnableSuperShield),
            };
            
            
            ActionManager.Add(new ActionQueue(actionData),SelfUpdateGroup);
        }
        
        private void RunNormalShieldQueue()
        {
            var actionData = new ActionData[]
            {
                new(() =>
                {
                    //Debug.Log("Ability Time Scale Set To 0");
                    CustomTime.SetChannelTimeScale(UpdateGroup.Ability, 0);
                    StartCoroutine(
                        Coroutine_RunActionByTime(HandleNormalShieldEnable, _appereance.TimeToDisableSuperShield));
                }),
                new(() =>
                {
                    //Debug.Log("Ability Time Scale Set To 1");
                    _appereance.SetActiveSuperShieldSprite(false);
                    _appereance.RestartValues();
                    _movement.RestartSpeedValues();
                    _movement.RotateTowardsNearestProjectileSlot();
                },_appereance.TimeToDisableSuperShield),
            };
            
            ActionManager.Add(new ActionQueue(actionData),SelfUpdateGroup);
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