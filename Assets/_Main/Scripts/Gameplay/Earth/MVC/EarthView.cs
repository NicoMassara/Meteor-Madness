using System;
using System.Collections;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Observer;
using _Main.Scripts.ScriptableObjects;
using _Main.Scripts.Shaker;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthView : ManagedBehavior, IObserver, IUpdatable
    {
        [Header("Model Components")]
        [SerializeField] private GameObject modelContainer;
        [SerializeField] private GameObject planeMeshContainer;
        [SerializeField] private EarthSlicer earthMeshSlicer;
        [Space]
        [Header("Sounds")]
        [SerializeField] private SoundBehavior collisionSound;
        [SerializeField] private SoundBehavior deathSound;
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

        public UnityAction OnHealed;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Earth;

        private void Awake()
        {
            _earthMaterialController = GetComponent<EarthMaterialController>();
            _earthRotator = new EarthRotator(modelContainer.transform, planeMeshContainer.transform, rotationSpeed);
            _shakerController = new ShakerController(modelContainer.transform);
        }

        private void Start()
        {
            _restartTimeValues = GameConfigManager.Instance.GetGameplayData().EarthTimeData.Restart;
            _shakerController.SetShakeData(healthShakeData);
            SetShakeMultiplier(1f);
        }

        public void ManagedUpdate()
        {
            var dt = CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup);
            
            _shakerController.HandleShake(dt);
            
            if (_canRotate == true)
            {
                _earthRotator.Rotate(dt,_isDead);
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
            }
        }

        #region Health
        
        private void HandleCollision(float healthAmount, Vector3 position, Quaternion rotation, Vector2 direction)
        {
            collisionSound?.PlaySound();
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
        }

        private void HandleHeal(float currentHealth, float lastHealth)
        {
            var restartHealthTime = _restartTimeValues.RestartHealth;
            
            var tempActions = new ActionData[]
            {
                new (() =>
                {
                    CustomTime.SetChannelTimeScale(new[]
                    {
                        UpdateGroup.Gameplay, UpdateGroup.Ability, 
                        UpdateGroup.Effects
                    }, 0f);
                    
                    StartCoroutine(Coroutine_HandleHeal(currentHealth, lastHealth, 
                        restartHealthTime));
                }),
                new (() =>
                {
                    CustomTime.SetChannelTimeScale(new[]
                    {
                        UpdateGroup.Gameplay, UpdateGroup.Ability, 
                        UpdateGroup.Effects
                    }, 1f);
                    
                },restartHealthTime),
            };
            
            ActionManager.Add(new ActionQueue(tempActions),SelfUpdateGroup);
        }
        
        private IEnumerator Coroutine_HandleHeal(float targetHealth, float lastHealth, float duration)
        {
            var elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                elapsedTime += CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup);
                var t = elapsedTime / duration;
                var value  = Mathf.Lerp(lastHealth, targetHealth, t);
                SetShakeMultiplier(value);
                UpdateColorByHealth(value);
                SetRotationSpeed(value);
                
                yield return null;
            }
            
            SetShakeMultiplier(targetHealth);
            UpdateColorByHealth(targetHealth);
            SetRotationSpeed(targetHealth);
        }

        private void HandleRestartHealth(float currentHealth)
        {
            ActionData[] tempActions;
            
            if (currentHealth >= 1)
            {
                tempActions = new ActionData[]
                {
                    new (() =>  HandleSetRotation(false)),
                    new (() => 
                            StartCoroutine(
                            Coroutine_RestartRotation(_restartTimeValues.RestartYRotation, modelContainer.transform)), 
                        _restartTimeValues.TimeBeforeRotateY),
                    new (() =>
                    {
                        SetShakeMultiplier(1);
                        _earthRotator.SetRotationSpeed(rotationSpeed);
                        _shakerController.SetShakeData(healthShakeData);
                        _isDead = false;
                        HandleSetRotation(true);
                        OnHealed?.Invoke();
                        EarthEventCaller.RestartFinished();
                    }, _restartTimeValues.FinishRestart),
                };
            }
            else if(currentHealth < 1 && currentHealth > 0)
            {
                tempActions = new ActionData[]
                {
                    new (() =>  HandleSetRotation(false)),
                    new (() => StartCoroutine(
                            Coroutine_RestartRotation(_restartTimeValues.RestartYRotation, modelContainer.transform)), 
                        _restartTimeValues.TimeBeforeRotateY),
                    new (() => StartCoroutine(
                            Coroutine_RestartHealthColor(_restartTimeValues.RestartHealth, currentHealth)), 
                        _restartTimeValues.RestartYRotation),
                    new (() =>
                    {
                        SetShakeMultiplier(1);
                        _earthRotator.SetRotationSpeed(rotationSpeed);
                        _shakerController.SetShakeData(healthShakeData);
                        _isDead = false;
                    
                    }, _restartTimeValues.RestartHealth),
                    new ActionData(() =>
                    {
                        HandleSetRotation(true);
                        OnHealed?.Invoke();
                        EarthEventCaller.RestartFinished();
                    }, _restartTimeValues.FinishRestart),
                };
            }
            else
            {
                tempActions = new ActionData[]
                {
                    new (() =>  HandleSetRotation(false)),
                    new (() => StartCoroutine(
                            Coroutine_RestartRotation(_restartTimeValues.RestartZRotation, 
                                planeMeshContainer.transform)), 
                        _restartTimeValues.TimeBeforeRotateZ),
                    new (() => earthMeshSlicer?.StartUnite(), 
                        _restartTimeValues.RestartZRotation),
                    new (() => StartCoroutine(
                            Coroutine_RestartRotation(_restartTimeValues.RestartYRotation, modelContainer.transform)), 
                        _restartTimeValues.TimeBeforeRotateY),
                    new (() => StartCoroutine(
                            Coroutine_RestartHealthColor(_restartTimeValues.RestartHealth, currentHealth)), 
                        _restartTimeValues.RestartYRotation),
                    new (() =>
                    {
                        SetShakeMultiplier(1);
                        _earthRotator.SetRotationSpeed(rotationSpeed);
                        _shakerController.SetShakeData(healthShakeData);
                        _isDead = false;
                    
                    }, _restartTimeValues.RestartHealth),
                    new (() =>
                    {
                        HandleSetRotation(true);
                        OnHealed?.Invoke();
                        EarthEventCaller.RestartFinished();
                    }, _restartTimeValues.FinishRestart),
                };
            }
            
            ActionManager.Add(new ActionQueue(tempActions),SelfUpdateGroup);
        }


        private IEnumerator Coroutine_RestartRotation(float timeToRestart, Transform objectToRotate)
        {
            Quaternion startRotation = objectToRotate.rotation;
            Quaternion targetRotation = Quaternion.identity;
            float elapsed = 0f;

            while (elapsed < timeToRestart)
            {
                elapsed += CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup);
                float t = elapsed / timeToRestart;

                objectToRotate.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                yield return null;
            }

            objectToRotate.rotation = targetRotation; // ensure exact final rotation
        }


        private IEnumerator Coroutine_RestartHealthColor(float timeToIncrease, float currentHealth)
        {
            float elapsedTime = 0;
            
            while (elapsedTime < timeToIncrease)
            {
                elapsedTime += CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup);
                var t = elapsedTime/timeToIncrease;
                var healthValue = Mathf.Lerp(currentHealth, 1f, t);
                UpdateColorByHealth(healthValue);
                
                yield return null;
            }

            UpdateColorByHealth(1);
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
            earthMeshSlicer.StartSlicing();
            _isDead = true;
            deathSound?.PlaySound();
            _earthRotator.SetRotationSpeed(rotationSpeed/2);
        }

        private void HandleDeath()
        {
            UpdateColorByHealth(0);
            _shakerController.SetMultiplier(0);
            _shakerController.SetShakeData(deathShakeData);
            GameModeEventCaller.Finish();
        }
        
        private void TriggerEndDestruction()
        {
            EarthEventCaller.DestructionFinished();
        }

        #endregion
        
        private void SetRotationSpeed(float healAmount)
        {
            var rotationMultiplier = rotationSpeedCurve.Evaluate(healAmount);
            _earthRotator.SetRotationSpeed(rotationSpeed * rotationMultiplier);
        }
        
        private void HandleSetRotation(bool canRotate)
        {
            _canRotate = canRotate;
        }

        private void SetShakeMultiplier(float currentHealth)
        {
            _shakerController.SetMultiplier(shakeMultiplier.Evaluate(currentHealth));
        }

        private void UpdateColorByHealth(float currentHealth)
        {
            _earthMaterialController.SetMaterialHealth(currentHealth);
        }
    }
}