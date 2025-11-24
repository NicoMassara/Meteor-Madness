using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using _Main.Scripts.ScriptableObjects;
using _Main.Scripts.Shaker;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthView : ManagedBehavior, IObserver, IUpdatable, IEarthSounds, IEarthSkin
    {
        [Header("Model Components")]
        [SerializeField] private GameObject planeMeshContainer;
        [SerializeField] private GameObject destroyedEarthContainer;
        [Space]
        [Header("Shake Values")]
        [SerializeField] private AnimationCurve shakeMultiplier;
        [SerializeField] private ShakeDataSo healthShakeData;
        [SerializeField] private ShakeDataSo deathShakeData;
        [SerializeField] private ShakeDataSo cameraShakeData;
        [Space]
        [Header("Values")] 
        [Range(0, 100)] 
        [SerializeField] private float rotationSpeed = 25;
        [SerializeField] private AnimationCurve rotationSpeedCurve;
        [SerializeField] private ParticleDataSo collisionParticleData;
        
        private EarthSlicer _slicer;
        private ShakerController _shakerController;
        private GameObject _currentSprite;
        private Rotator _planeRotator;
        private IEarthRestart _restartTimeValues;
        private bool _isDead;
        private float _deltaTime;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Earth;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

        public event Action<float> OnHealthChanged;
        public event Action OnHealed;
        public event Action OnHealing;
        public event Action OnCollision;
        public event Action OnDestruction;
        public event Action OnPreDestruction;
        public event Action<bool> OnLowHealth;
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private EarthDebugData _debugData;
        
#endif
        
        private void Awake()
        {
            _slicer = GetComponent<EarthSlicer>();
            _planeRotator = new Rotator(destroyedEarthContainer.transform, Vector3.up, rotationSpeed/2);
            _shakerController = new ShakerController(planeMeshContainer.transform);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData = new EarthDebugData();
#endif
        }

        private void Start()
        {
            _restartTimeValues = GameConfigManager.Instance.GetGameplayData().EarthTimeData.Restart;
            _shakerController.SetShakeData(healthShakeData);
            SetShakeMultiplier(1f);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.RotationSpeed = 1;
            _debugData.EarthHealth = 1;
            _debugData.ShakeIntensity = 0;
#endif
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _deltaTime = deltaTime;
            //
            _shakerController.HandleShake(_deltaTime);
            
            if (_isDead)
            {
                _planeRotator.Rotate(_deltaTime);
            }
        }

        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case EarthObserverMessage.RestartHealth:
                    HandleRestartHealth((float)args[0]);
                    break;
                case EarthObserverMessage.EarthCollision:
                    HandleCollision((float)args[0],
                        (Vector3)args[1],
                        (Quaternion)args[2],
                        (Vector2)args[3]);
                    break;
                case EarthObserverMessage.DeclareDeath:
                    HandleDeath();
                    break;
                case EarthObserverMessage.TriggerDestruction:
                    HandleDestruction();
                    break;
                case EarthObserverMessage.SetActiveDeathShake:
                    SetDeathShake((bool)args[0]);
                    break;
                case EarthObserverMessage.Heal:
                    HandleHeal((float)args[0],(float)args[1]);
                    break;
                case EarthObserverMessage.TriggerEndDestruction:
                    TriggerEndDestruction();
                    break;
                case EarthObserverMessage.SetLowHealth:
                    HandleSetLowHealth((bool)args[0]);
                    break;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                case EarthObserverMessage.Debug_UpdateHealth:
                    _debugData.EarthHealth = (float)args[0];
                    break;
#endif
                
            }
        }

        private void HandleSetLowHealth(bool isLowHealth)
        {
            OnLowHealth?.Invoke(isLowHealth);
        }

        #region Health
        
        private void HandleCollision(float healthAmount, Vector3 position, Quaternion rotation, Vector2 direction)
        {
            SetShakeMultiplier(healthAmount);
            UpdateHealth(healthAmount);
            
            ParticleEventCaller.Spawn(new ParticleSpawnData
            {
                ParticleData = collisionParticleData,
                Position = position,
                Rotation = rotation,
                MoveDirection = -direction
            });
            
            CameraEventCaller.Shake(cameraShakeData);
            OnCollision?.Invoke();
        }
        
        private class HealAction : IQueueAction
        {
            private readonly float _targetHealth;
            private readonly float _lastHealth;
            private readonly float _duration;
            private readonly Action<float> setShakeMultiplier;
            private readonly Action<float> updateColorByHealth;

            private float _elapsed;

            public ActionStatus CurrentStatus { get; private set; } = ActionStatus.Idle;

            // Constructor
            public HealAction(float lastHealth, float targetHealth, float duration,
                Action<float> setShakeMultiplier, 
                Action<float> updateColorByHealth)
            {
                _lastHealth = lastHealth;
                _targetHealth = targetHealth;
                _duration = duration;
                this.setShakeMultiplier = setShakeMultiplier;
                this.updateColorByHealth = updateColorByHealth;
            }

            public void OnStart()
            {
                _elapsed = 0f;
                CurrentStatus = ActionStatus.Running;
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                if (CurrentStatus != ActionStatus.Running)
                    return CurrentStatus;
                
                _elapsed += deltaTime;

                var t = Mathf.Clamp01(_elapsed / _duration);
                var value = Mathf.Lerp(_lastHealth, _targetHealth, t);
                
                SetShakeMultiplier(value);
                UpdateColorByHealth(value);
                
                if (_elapsed >= _duration)
                    CurrentStatus = ActionStatus.Success;

                return CurrentStatus;
            }

            public void OnInterrupt()
            {
                CurrentStatus = ActionStatus.Failure;
            }

            public IQueueAction Copy()
            {
                return null;
            }
            
            private void SetShakeMultiplier(float value)
            {
                setShakeMultiplier?.Invoke(value);
            }

            private void UpdateColorByHealth(float value)
            {
                updateColorByHealth?.Invoke(value);
            }
        }

        private void HandleHeal(float currentHealth, float lastHealth)
        {
            var restartHealthTime = _restartTimeValues.RestartHealth;

            var action = ActionBuilder.Start()
                .Do(new InstantAction(() =>
                {
                    CustomTime.SetChannelTimeScale(new[]
                    {
                        UpdateGroup.Gameplay, UpdateGroup.Ability, 
                        UpdateGroup.Effects
                    }, 0f);
                }))
                .Then(new HealAction(lastHealth,currentHealth,restartHealthTime,
                    SetShakeMultiplier,UpdateHealth))
                .Then(new InstantAction(() =>
                {
                    CustomTime.SetChannelTimeScale(new[]
                    {
                        UpdateGroup.Gameplay, UpdateGroup.Ability, 
                        UpdateGroup.Effects
                    }, 1f);
                }))
                .Build();
            
            ActionManager.Add(action);
        }
        
        private class RestartRotationAction : IQueueAction
        {
            private readonly float _targetTime;
            private readonly Transform _targetToRotate;
            private float _elapsed;
            private Quaternion _startRotation;
            private Quaternion _targetRotation;

            public ActionStatus CurrentStatus { get; private set; } = ActionStatus.Idle;

            public RestartRotationAction(float targetTime, Transform targetToRotate)
            {
                _targetTime = targetTime;
                _targetToRotate = targetToRotate;
            }

            public void OnStart()
            {
                _startRotation = _targetToRotate.rotation;
                _targetRotation = Quaternion.identity;
                _elapsed = 0;
                CurrentStatus = ActionStatus.Running;
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                if (_elapsed < _targetTime)
                {
                    _elapsed += deltaTime;
                    float t = _elapsed / _targetTime;
                    _targetToRotate.rotation = Quaternion.Slerp(_startRotation, _targetRotation, t);
                    
                    if (_elapsed >= _targetTime)
                    {
                        _targetToRotate.rotation = _targetRotation;
                        CurrentStatus = ActionStatus.Success;
                    }
                }

                return CurrentStatus;
            }

            public void OnInterrupt()
            {
                CurrentStatus = ActionStatus.Failure;
                _targetToRotate.rotation = _targetRotation;
            }
            public IQueueAction Copy()
            {
                return new RestartRotationAction(_targetTime, _targetToRotate);
            }
        }
        private class RestartHealthColor : IQueueAction
        {
            private readonly float _timeToIncrease;
            private readonly float _currentHealth;
            private readonly Action<float> _updateColorByHealth;
            private float _elapsed;
            public ActionStatus CurrentStatus { get; private set; } = ActionStatus.Idle;

            public RestartHealthColor(float timeToIncrease, float currentHealth, Action<float> updateColorByHealth)
            {
                _timeToIncrease = timeToIncrease;
                _currentHealth = currentHealth;
                _updateColorByHealth = updateColorByHealth;
            }

            public void OnStart()
            {
                CurrentStatus = ActionStatus.Running;
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                if (_elapsed < _timeToIncrease)
                {
                    _elapsed += deltaTime;
                    float t = _elapsed / _timeToIncrease;
                    var healthValue = Mathf.Lerp(_currentHealth, 1, t);
                    _updateColorByHealth?.Invoke(healthValue);
                    
                    if (t >= 1)
                    {
                        _updateColorByHealth?.Invoke(1);
                        CurrentStatus = ActionStatus.Success;
                    }
                }
                
                return CurrentStatus;
            }

            public void OnInterrupt()
            {
                CurrentStatus = ActionStatus.Failure;
                _updateColorByHealth?.Invoke(1);
            }

            public IQueueAction Copy()
            {
                return new RestartHealthColor(_timeToIncrease, _currentHealth, _updateColorByHealth);
            }
        }

        private void HandleRestartHealth(float currentHealth)
        {
            var action = ActionBuilder.Start().Do(new InstantAction(()=> _isDead = false));
                // Only executes when is dead 

                #region Slieces Unity
                
                if (currentHealth <= 0)
                {
                    action
                        .Then(new RestartRotationAction(_restartTimeValues.RestartZRotation,
                            destroyedEarthContainer.transform))
                        .Then(new InstantAction(() => _slicer.StartUnite()))
                        .Then(new WaitForEventAction(
                            subscribe: callback => _slicer.OnEndUnite += callback,
                            unsubscribe: callback => _slicer.OnEndUnite -= callback
                        ))
                        .Then(new InstantAction(()=> planeMeshContainer.gameObject.SetActive(true)))
                        .Then(new WaitFramesAction(3))
                        .Then(new InstantAction(()=> _slicer.UniteMeshes()));
                }
                
                #endregion
            
                
                //Only Executes if it has damage

                if (currentHealth  < 1)
                {
                    action
                        .Then(new RestartHealthColor(_restartTimeValues.RestartHealth, currentHealth,
                            UpdateHealth))
                        .WrapLast(a => new CallbackWrapperAction(a, OnHealing, null));
                }
                
                action
                .Then(new SetFloatAction(1, SetShakeMultiplier))
                .Then(new InstantAction(() =>
                {
                    _shakerController.SetShakeData(healthShakeData);
                }))
                .Then(new WaitSecondsAction(_restartTimeValues.FinishRestart))
                .Then(new InstantAction(() =>
                {
                    OnHealed?.Invoke();
                    EarthEventCaller.RestartFinished();
                }));
            
            ActionManager.Add(action.Build());
        }

        #endregion

        #region Death

        // ReSharper disable Unity.PerformanceAnalysis
        private void SetDeathShake(bool isShaking)
        {
            if (isShaking)
            {
                _shakerController.SetMultiplier(1);
                EarthEventCaller.ShakeStart();
            }
            else
            {
                _shakerController.SetMultiplier(0);
            }
        }

        private void HandleDestruction()
        {
            planeMeshContainer.gameObject.SetActive(false);
            _slicer.StartSlicing();
            _isDead = true;
            OnDestruction?.Invoke();
        }

        private void HandleDeath()
        {
            UpdateHealth(0);
            _shakerController.SetMultiplier(0);
            _shakerController.SetShakeData(deathShakeData);
            OnPreDestruction?.Invoke();
            EarthEventCaller.Death();
        }
        
        private void TriggerEndDestruction()
        {
            EarthEventCaller.DestructionFinished();
        }

        #endregion
        
        private void SetShakeMultiplier(float currentHealth)
        {
            var multiplier = shakeMultiplier.Evaluate(currentHealth);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.ShakeIntensity = multiplier;
#endif
            _shakerController.SetMultiplier(multiplier);
        }

        private void UpdateHealth(float currentHealth)
        {
            OnHealthChanged?.Invoke(currentHealth);
        }
    }
}