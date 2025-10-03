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
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    [RequireComponent(typeof(MeteorDetector))]
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
        [SerializeField] private ShieldMovementDataSo movementData;
        [Space]
        [Header("Values")]
        [Range(0.1f, 5f)]
        [SerializeField] private float timeToEnableSuperShield;
        [Range(0.1f, 5f)]
        [SerializeField] private float timeToDisableSuperShield;
        [Range(0, 1000)]
        [SerializeField] private float decayConstant = 100f;
        [SerializeField] private bool autoCorrectEnable;
        
        private MeteorDetector _meteorDetector;
        private ShieldMovement _movement;
        private ShieldSpeeder _shieldSpeeder;
        private ShieldSpriteAlphaSetter _spriteAlphaSetter;
        private ShakerController _shakerController;

        private bool _isPlayerInputEnable;
        private bool _canAutoCorrect = true;
        private bool _automaticEnable;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Shield;

        private void Awake()
        {
            _movement = new ShieldMovement(spriteContainer.transform,movementData);
            _shieldSpeeder = new ShieldSpeeder(_movement,timeToEnableSuperShield,timeToDisableSuperShield,decayConstant);
            _spriteAlphaSetter = new ShieldSpriteAlphaSetter(normalSprite,superSprite, timeToEnableSuperShield,timeToDisableSuperShield);
            _meteorDetector = GetComponent<MeteorDetector>();
            _meteorDetector.SetMovement(_movement);
        }

        private void Start()
        {
            _meteorDetector.OnTargetFound += Detector_OnTargetFoundHandler;
            _meteorDetector.OnTargetLost += Detector_OnTargetLostHandler;
            
            superSprite.gameObject.SetActive(false);
            _shakerController = new ShakerController(spriteContainer.transform,hitShakeData);
        }

        public void ManagedUpdate()
        {
            _movement.Update(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));

            if (_automaticEnable)
            {
                _meteorDetector.CheckForMeteor(_movement.GetPosition(), true);
            }
        }

        public void OnNotify(ulong message, params object[] args)
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
                case ShieldObserverMessage.StopRotate:
                    HandleStopRotate();
                    break;
                case ShieldObserverMessage.PlayMoveSound:
                    PlayMoveSound();
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
            }
        }

        private void HandleRestartPosition()
        {
            _movement.Restart();
        }
        
        #region States

        private void HandleSetAutomatic(bool isActive)
        {
            _automaticEnable = isActive;
            var color = isActive ? Color.red : Color.white;
            normalSprite.GetComponent<SpriteRenderer>().color = color;
        }

        private void HandleSetGold(bool isActive)
        {
            var color = isActive ? Color.yellow : Color.white;
            normalSprite.GetComponent<SpriteRenderer>().color = color;
        }
        
        private void HandleSetActiveShield(bool isActive)
        {
            spriteContainer.SetActive(isActive);
        }

        #endregion

        #region Movement
        private void HandleRotation(float direction)
        {
            if(_isPlayerInputEnable) return;
            
            _movement.HandleMove((int)direction,CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
            
            if(_canAutoCorrect == false || autoCorrectEnable == false) return;
            
            _meteorDetector.CheckForNearMeteorInSlotRange(
                _movement.GetPosition(),_movement.GetCurrentSlot());
        }
        
        private void HandleStopRotate()
        {
            _movement.HandleMove(0,0);
        }
        
        private void PlayMoveSound()
        {
            moveSound?.PlaySound();
        }

        #endregion

        #region Deflect

        private void HandleDeflect(Vector3 position, Quaternion rotation, Vector2 direction)
        {
            hitSound?.PlaySound();
            
            StartCoroutine(Coroutine_Shake());
            
            GameManager.Instance.EventManager.Publish
            (
                new ParticleEvents.Spawn
                {
                    ParticleData = deflectParticleData,
                    Position = position,
                    Rotation = rotation,
                    MoveDirection = direction
                }
            );
            
            GameManager.Instance.EventManager.Publish(new CameraEvents.Shake{ShakeData = cameraShakeData});
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
            
            ActionManager.Add(new ActionQueue(actionData),SelfUpdateGroup);
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

        private IEnumerator Coroutine_AutoCorrection()
        {
            _isPlayerInputEnable = true;
            var currentDirection = _meteorDetector.GetSlotDirection(_movement.GetCurrentSlot());
            
            
            while (currentDirection != 0 && _isPlayerInputEnable == true)
            {
                currentDirection = _meteorDetector.GetSlotDirection(_movement.GetCurrentSlot()) * 10;
                _movement.HandleMove((int)currentDirection,CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
                
                yield return null;
            }
            
            HandleStopRotate();
            _canAutoCorrect = false;

            TimerManager.Add(new TimerData
            {
                Time = CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup) * 5,
                OnEndAction = () => { _canAutoCorrect = true; }
            });
            
            TimerManager.Add(new TimerData
            {
                Time = CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup) * 30,
                OnEndAction = () => { _isPlayerInputEnable = false; }
            });
        }

        private IEnumerator Coroutine_RotateTowardsNearestMeteor()
        {
            _meteorDetector.CheckForMeteor(_movement.GetPosition());
            var meteorSlot = _meteorDetector.GetMeteorAngleSlot();

            if (meteorSlot > -1)
            {
                _movement.SetSpeedMultiplier(0.5f);
            
                while (meteorSlot != _movement.GetCurrentSlot())
                {
                    _movement.HandleMove(1, CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
                
                    yield return null;
                }
            
                _movement.HandleMove(0, 0);
                _movement.SetSpeedMultiplier(1);
            }
            
            CustomTime.SetChannelTimeScale(UpdateGroup.Ability, 1);
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

        #region Handlers

        private void Detector_OnTargetLostHandler()
        {
            _isPlayerInputEnable = false;
        }

        private void Detector_OnTargetFoundHandler(bool doesCheckForMore)
        {
            if (doesCheckForMore)
            {
                if (_canAutoCorrect)
                {
                    StartCoroutine(Coroutine_AutoCorrection());
                }
            }
        }

        #endregion
    }
}