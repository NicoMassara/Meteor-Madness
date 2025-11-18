using System;
using System.Collections;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using _Main.Scripts.ScriptableObjects;
using _Main.Scripts.Shaker;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthView : ManagedBehavior, IObserver, IUpdatable
    {
        [Header("Model Components")]
        [SerializeField] private GameObject modelContainer;
        [SerializeField] private GameObject planeMeshContainer;
        [SerializeField] private EarthSlicer earthMeshSlicer;
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
        
        private EarthMaterialController _earthMaterialController;
        private ShakerController _shakerController;
        private GameObject _currentSprite;
        private EarthRotator _earthRotator;
        private IEarthRestart _restartTimeValues;
        private bool _canRotate;
        private bool _isDead;
        private float _deltaTime;

        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Earth;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

        public event Action OnHealed;
        public event Action OnCollision;
        public event Action OnDestruction;
        public event Action OnPreDestruction;
        public event Action<bool> OnLowHealth;
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private EarthDebugData _debugData;
        
#endif
        
        private void Awake()
        {
            _earthMaterialController = GetComponent<EarthMaterialController>();
            _earthRotator = new EarthRotator(modelContainer.transform, planeMeshContainer.transform, rotationSpeed);
            _shakerController = new ShakerController(modelContainer.transform);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData = new EarthDebugData();
#endif
        }

        private void Start()
        {
            _restartTimeValues = GameConfigManager.Instance.GetGameplayData().EarthTimeData.Restart;
            _shakerController.SetShakeData(healthShakeData);
            SetShakeMultiplier(1f);
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _deltaTime = deltaTime;
            _shakerController.HandleShake(deltaTime);
            
            if (_canRotate == true)
            {
                _earthRotator.Rotate(deltaTime,_isDead);
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
                case EarthObserverMessage.SetRotation:
                    HandleSetRotation((bool)args[0]);
                    break;
                case EarthObserverMessage.TriggerEndDestruction:
                    TriggerEndDestruction();
                    break;
                case EarthObserverMessage.SetLowHealth:
                    HandleSetLowHealth((bool)args[0]);
                    break;
            }
        }

        private void HandleSetLowHealth(bool isLowHealth)
        {
            OnLowHealth?.Invoke(isLowHealth);
        }

        #region Health
        
        private void HandleCollision(float healthAmount, Vector3 position, Quaternion rotation, Vector2 direction)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.EarthHealth = healthAmount;
#endif
            SetShakeMultiplier(healthAmount);
            UpdateColorByHealth(healthAmount);
            SetRotationSpeed(healthAmount);
            
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
            private readonly Action<float> setRotationSpeed;

            private float _elapsed;

            public ActionStatus CurrentStatus { get; private set; } = ActionStatus.Idle;

            // Constructor
            public HealAction(float lastHealth, float targetHealth, float duration,
                Action<float> setShakeMultiplier, 
                Action<float> updateColorByHealth, 
                Action<float> setRotationSpeed)
            {
                _lastHealth = lastHealth;
                _targetHealth = targetHealth;
                _duration = duration;
                this.setShakeMultiplier = setShakeMultiplier;
                this.updateColorByHealth = updateColorByHealth;
                this.setRotationSpeed = setRotationSpeed;
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
                SetRotationSpeed(value);
                
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

            private void SetRotationSpeed(float value)
            {
                setRotationSpeed?.Invoke(value);
            }
        }

        private void HandleHeal(float currentHealth, float lastHealth)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.EarthHealth = currentHealth;
#endif
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
                    SetShakeMultiplier,UpdateColorByHealth,SetRotationSpeed))
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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.EarthHealth = currentHealth;
#endif
            var action = ActionBuilder.Start().Do(new WaitFramesAction(1));
                // Only executes when is dead 

                #region Slieces Unity
                
                if (currentHealth <= 0)
                {
                    action
                        .Then(new RestartRotationAction(_restartTimeValues.RestartZRotation, planeMeshContainer.transform))
                        // Unites the pieces and waits to end it
                        .Then(new InstantAction(() => { earthMeshSlicer?.StartUnite(); }))
                        .Then(new WaitForEventAction(
                            subscribe: callback => earthMeshSlicer.OnEndUnite += callback,
                            unsubscribe: callback => earthMeshSlicer.OnEndUnite -= callback
                        ));
                }
                
                #endregion
                
                action
                .Then(new InstantAction(() => HandleSetRotation(false)))
                .Then(new RestartRotationAction(_restartTimeValues.RestartYRotation, modelContainer.transform))
                
                //Only Executes if it has damage
                .Then(new RestartHealthColor(_restartTimeValues.RestartHealth,currentHealth,UpdateColorByHealth))
                .WrapLast(inner => new ConditionalWrapperAction(inner, ()=> currentHealth  < 1))
                //
                
                .Then(new SetFloatAction(1, SetShakeMultiplier))
                .Then(new SetFloatAction(rotationSpeed, _earthRotator.SetRotationSpeed))
                .Then(new InstantAction(() =>
                {
                    _shakerController.SetShakeData(healthShakeData);
                    _isDead = false;
                }))
                .Then(new WaitSecondsAction(_restartTimeValues.FinishRestart))
                .Then(new InstantAction(() =>
                {
                    HandleSetRotation(true);
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
            OnDestruction?.Invoke();
            earthMeshSlicer.StartSlicing();
            _isDead = true;
            _earthRotator.SetRotationSpeed(rotationSpeed/2);
        }

        private void HandleDeath()
        {
            UpdateColorByHealth(0);
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
        
        private void SetRotationSpeed(float healAmount)
        {
            var rotationMultiplier = rotationSpeedCurve.Evaluate(healAmount);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.RotationSpeed = rotationMultiplier;
#endif
            _earthRotator.SetRotationSpeed(rotationSpeed * rotationMultiplier);
        }
        
        private void HandleSetRotation(bool canRotate)
        {
            _canRotate = canRotate;
        }

        private void SetShakeMultiplier(float currentHealth)
        {
            var multiplier = shakeMultiplier.Evaluate(currentHealth);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.ShakeIntensity = multiplier;
#endif
            _shakerController.SetMultiplier(multiplier);
        }

        private void UpdateColorByHealth(float currentHealth)
        {
            _earthMaterialController.SetMaterialHealth(currentHealth);
        }
    }
}