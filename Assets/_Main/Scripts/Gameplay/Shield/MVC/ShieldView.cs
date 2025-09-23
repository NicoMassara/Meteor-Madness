﻿using System;
using System.Collections;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Observer;
using _Main.Scripts.Particles;
using _Main.Scripts.Shaker;
using _Main.Scripts.Sounds;
using _Main.Scripts.Gameplay.AutoTarget;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldView : ManagedBehavior, IObserver, IUpdatable
    {
        [Header("Components")] 
        [SerializeField] private GameObject spriteContainer;
        [SerializeField] private GameObject normalSprite;
        [SerializeField] private GameObject superSprite;
        [Space]
        [Header("Sounds")]
        [SerializeField] private SoundBehavior hitSound;
        [SerializeField] private SoundBehavior moveSound;
        [Space] 
        [Header("Scriptable Objects")]
        [SerializeField] private ShakeDataSo hitShakeData;
        [SerializeField] private ShakeDataSo cameraShakeData;
        [SerializeField] private ParticleDataSo deflectParticleData;
        [SerializeField] private ShieldMovementSo shieldMovementData;
        [Space]
        [Header("Values")]
        [Range(0.1f, 5f)]
        [SerializeField] private float timeToEnableSuperShield;
        [Range(0.1f, 5f)]
        [SerializeField] private float timeToDisableSuperShield;
        [Range(0, 1000)]
        [SerializeField] private float decayConstant = 100f;
        [Header("Meteor Detector")]
        [SerializeField] private LayerMask meteorLayer;
        [Range(0.5f, 3f)] 
        [SerializeField] private float meteorCheckRadius = 10f;
        
        private MeteorDetector _meteorDetector;
        private ShieldMovement _movement;
        private ShieldSpeeder _shieldSpeeder;
        private ShieldSpriteAlphaSetter _spriteAlphaSetter;
        private ShakerController _shakerController;
        
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Shield;

        private void Awake()
        {
            _movement = new ShieldMovement(shieldMovementData,spriteContainer.transform);
            _shieldSpeeder = new ShieldSpeeder(_movement,timeToEnableSuperShield,timeToDisableSuperShield,decayConstant);
            _spriteAlphaSetter = new ShieldSpriteAlphaSetter(normalSprite,superSprite, timeToEnableSuperShield,timeToDisableSuperShield);
            _meteorDetector = new MeteorDetector(_movement.GetAngle,meteorLayer);
        }

        private void Start()
        {
            superSprite.gameObject.SetActive(false);
            _shakerController = new ShakerController(transform);
            _shakerController.SetShakeData(hitShakeData);
        }

        public void ManagedUpdate()
        {
            if (_shakerController.IsShaking)
            {
                _shakerController.HandleShake(CustomTime.GetChannel(SelfUpdateGroup).DeltaTime);
            }
        }

        public void OnNotify(string message, params object[] args)
        {
            switch (message)
            {
                case ShieldObserverMessage.Rotate:
                    HandleRotation((float)args[0]);
                    break;
                case ShieldObserverMessage.Deflect:
                    HandleDeflect((Vector3)args[0],
                        (Quaternion)args[1],
                        (Vector2)args[2]);
                    break;
                case ShieldObserverMessage.PlayMoveSound:
                    PlayMoveSound();
                    break;
                case ShieldObserverMessage.RestartPosition:
                    HandleRestartPosition();
                    break;
                case ShieldObserverMessage.SetActiveShield:
                    HandleSetActiveShield((bool)args[0]);
                    break;
                case ShieldObserverMessage.SetActiveSuperShield:
                    HandleSetSuperActive((bool)args[0]);
                    break;
            }
        }
        

        #region Sprites

        private void HandleSetActiveShield(bool isActive)
        {
            spriteContainer.SetActive(isActive);
        }

        #endregion

        #region Movement
        private void HandleRotation(float direction)
        {
            _movement.Move(direction, CustomTime.GetChannel(SelfUpdateGroup).DeltaTime);
        }
        
        private void PlayMoveSound()
        {
            moveSound?.PlaySound();
        }
        
        private void HandleRestartPosition()
        {
            transform.rotation = Quaternion.Euler(0,0,0);
        }
        

        #endregion

        private void HandleDeflect(Vector3 position, Quaternion rotation, Vector2 direction)
        {
            _shakerController.StartShake();
            hitSound?.PlaySound();
            
            GameManager.Instance.EventManager.Publish
            (
                new SpawnParticle
                {
                    ParticleData = deflectParticleData,
                    Position = position,
                    Rotation = rotation,
                    MoveDirection = direction
                }
            );
            
            GameManager.Instance.EventManager.Publish(new CameraShake{ShakeData = cameraShakeData});
        }

        private IEnumerator Coroutine_RunActionByTime(Action<float> action, float targetTime)
        {
            var elapsedTime = 0f;
            
            while (elapsedTime < targetTime)
            {
                var deltaTime = CustomTime.GetChannel(SelfUpdateGroup).DeltaTime;
                elapsedTime += deltaTime;
                action?.Invoke(deltaTime);
                
                yield return null;
            }
        }

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
                    superSprite.gameObject.SetActive(true);
                    CustomTime.SetChannelTimeScale(UpdateGroup.Ability, 0);
                    StartCoroutine(Coroutine_RunActionByTime(HandleSuperShieldEnable, timeToEnableSuperShield));
                }),
                new(() =>
                {
                    //Debug.Log("Ability Time Scale Set to 1");
                    _shieldSpeeder.RestartValues();
                    _spriteAlphaSetter.RestartValues();
                    CustomTime.SetChannelTimeScale(UpdateGroup.Ability, 1);
                },timeToEnableSuperShield),
            };
            
            
            ActionQueue tempQueue = new ActionQueue(actionData);
            StartCoroutine(Coroutine_RunActionQueue(tempQueue));
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
                        Coroutine_RunActionByTime(HandleNormalShieldEnable, timeToDisableSuperShield));
                }),
                new(() =>
                {
                    //Debug.Log("Ability Time Scale Set To 1");
                    superSprite.gameObject.SetActive(false);
                    _spriteAlphaSetter.RestartValues();
                    _shieldSpeeder.RestartValues();
                    StartCoroutine(Coroutine_RotateTowardsNearestMeteor());
                },timeToDisableSuperShield),
            };
            
            
            ActionQueue tempQueue = new ActionQueue(actionData);
            StartCoroutine(Coroutine_RunActionQueue(tempQueue));
        }

        private IEnumerator Coroutine_RunActionQueue(ActionQueue actionQueue)
        {
            
            while (!actionQueue.IsEmpty)
            {
                actionQueue.Run(CustomTime.GetChannel(SelfUpdateGroup).DeltaTime);
                
                yield return null;
            }
        }
        
        private void HandleSuperShieldEnable(float deltaTime)
        {
            _spriteAlphaSetter.EnableSuper(deltaTime);
            _shieldSpeeder.IncreaseSpeed(deltaTime);
        }

        private void HandleNormalShieldEnable(float deltaTime)
        {
            _spriteAlphaSetter.EnableNormal(deltaTime);
            _shieldSpeeder.DecreaseSpeed(deltaTime);
        }
        
        #endregion

        private IEnumerator Coroutine_RotateTowardsNearestMeteor()
        {
            _meteorDetector.CheckForNearMeteor(_movement.GetPosition(), Mathf.Infinity);
            
            while (_meteorDetector.GetDirectionToMeteorAngle() != 0)
            {
                _movement.Move(0.5f, CustomTime.GetChannel(SelfUpdateGroup).DeltaTime);
                
                yield return null;
            }
            
            CustomTime.SetChannelTimeScale(UpdateGroup.Ability, 1);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(normalSprite.transform.position, meteorCheckRadius);
        }
    }
}