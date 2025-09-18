using System.Collections;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Observer;
using _Main.Scripts.Particles;
using _Main.Scripts.Shaker;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthView : ManagedBehavior, IObserver, IUpdatable
    {
        [Header("Model Components")]
        [SerializeField] private GameObject modelContainer;
        [SerializeField] private MeshRenderer modelMeshRenderer;
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
        [SerializeField] private ParticleDataSo collisionParticleData;
        
        private Material _modelMaterial;
        private EarthController _controller;
        private ShakerController _shakerController;
        private GameObject _currentSprite;
        private EarthRotator _earthRotator;
        private bool _canRotate;
        private bool _isDead;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Earth;

        private void Awake()
        {
            _earthRotator = new EarthRotator(modelContainer.transform, planeMeshContainer.transform, rotationSpeed);
            _shakerController = new ShakerController(modelContainer.transform);
            //Model Material Settings
            _modelMaterial = modelMeshRenderer.materials[0];
        }

        public void ManagedUpdate()
        {
            var dt = CustomTime.GetChannel(SelfUpdateGroup).DeltaTime;
            
            _shakerController.HandleShake(dt);
            
            if (_canRotate == true)
            {
                _earthRotator.Rotate(dt,_isDead);
            }
        }

        public void OnNotify(string message, params object[] args)
        {
            switch (message)
            {
                case EarthObserverMessage.RestartHealth:
                    HandleRestartHealth();
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
                    HandleHeal((float)args[0]);
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
            _earthRotator.SetRotationSpeed(rotationSpeed * healthAmount);
            
            GameManager.Instance.EventManager.Publish
            (
                new SpawnParticle
                {
                    ParticleData = collisionParticleData,
                    Position = position,
                    Rotation = rotation,
                    MoveDirection = -direction
                }
            );
            
            GameManager.Instance.EventManager.Publish(new CameraShake{ShakeData = cameraShakeData});
        }
        
        private void HandleHeal(float healthAmount)
        {
            SetShakeMultiplier(healthAmount);
            UpdateColorByHealth(healthAmount);
        }
        private void HandleRestartHealth()
        {
            StartCoroutine(Coroutine_RestartHealth());
        }

        private IEnumerator Coroutine_RestartHealth()
        {
            var tempActions = new ActionData[]
            {
                new ActionData(() =>  HandleSetRotation(false)),
                new ActionData(() => StartCoroutine(
                    Coroutine_RestartRotation(EarthRestartTimeValues.RestartZRotation, 
                    planeMeshContainer.transform)), 
                    EarthRestartTimeValues.TimeBeforeRotateZ),
                new ActionData(() => earthMeshSlicer?.StartUnite(), 
                    EarthRestartTimeValues.RestartZRotation),
                new ActionData(() => StartCoroutine(
                    Coroutine_RestartRotation(EarthRestartTimeValues.RestartYRotation, modelContainer.transform)), 
                    EarthRestartTimeValues.TimeBeforeRotateY),
                new ActionData(() => StartCoroutine(
                    Coroutine_RestartHealthColor(EarthRestartTimeValues.RestartHealth)), 
                    EarthRestartTimeValues.RestartYRotation),
                new ActionData(() =>
                {
                    SetShakeMultiplier(1);
                    _earthRotator.SetRotationSpeed(rotationSpeed);
                    _shakerController.SetShakeData(healthShakeData);
                    _isDead = false;
                    
                }, EarthRestartTimeValues.RestartHealth),
                new ActionData(() =>
                {
                    HandleSetRotation(true);
                    GameManager.Instance.EventManager.Publish(new EarthRestartFinish());
                    
                }, EarthRestartTimeValues.FinishRestart),
                
            };
            var tempQueue = new ActionQueue(tempActions);

            while (!tempQueue.IsEmpty)
            {
                tempQueue.Run(CustomTime.GetChannel(SelfUpdateGroup).DeltaTime);
                
                yield return null;
            }
        }

        private IEnumerator Coroutine_RestartRotation(float timeToRestart, Transform objectToRotate)
        {
            Quaternion startRotation = objectToRotate.rotation;
            Quaternion targetRotation = Quaternion.identity;
            float elapsed = 0f;

            while (elapsed < timeToRestart)
            {
                elapsed += CustomTime.GetChannel(SelfUpdateGroup).DeltaTime;
                float t = elapsed / timeToRestart;

                objectToRotate.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                yield return null;
            }

            objectToRotate.rotation = targetRotation; // ensure exact final rotation
        }


        private IEnumerator Coroutine_RestartHealthColor(float timeToIncrease)
        {
            float healthValue = 0;
            float elapsedTime = 0;
            
            while (elapsedTime < timeToIncrease)
            {
                elapsedTime += CustomTime.GetChannel(SelfUpdateGroup).DeltaTime;
                UpdateColorByHealth(healthValue);
                healthValue = elapsedTime/timeToIncrease;
                
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
                GameManager.Instance.EventManager.Publish(new EarthShake());
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
            GameManager.Instance.EventManager.Publish(new GameFinished());
        }
        
        private void TriggerEndDestruction()
        {
            GameManager.Instance.EventManager.Publish(new EarthEndDestruction());
        }

        #endregion
        
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
            _modelMaterial.EnableKeyword("_EMISSION");
            
            _modelMaterial.SetColor("_EmissionColor", new Color(1, currentHealth, currentHealth)); 
        }

        public void SetController(EarthController controller)
        {
            _controller = controller;
        }
    }
}