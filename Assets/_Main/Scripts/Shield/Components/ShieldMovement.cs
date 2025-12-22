using System;
using System.Collections;
using _Main.Scripts.AutoTarget;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Shield.Rotation;
using _Main.Scripts.Utilities;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Shield
{
    public class ShieldMovement : ManagedBehavior, IUpdatable, IFixedUpdatable
    {
        [Header("Components")] 
        [SerializeField] private GameObject spriteContainer;
        [Header("Scriptable Objects")]
        [SerializeField] private RotationDataSo movementData;
        [Header("Values")]
        [SerializeField] private ProjectileDetectorData detectorData;
        [SerializeField] private ShieldSpeeder.ShieldSpeederData speederData;
        
        private ProjectileDetector _projectileDetector;
        private IShieldMovement _movement;
        private bool _isPlayerInputDisable;
        private bool _canAutoCheck;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Shield;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

        public ShieldSpeeder ShieldSpeeder { get; private set; }
        
        public event Action OnProjectileDetected;
        
        public event Action<int> OnStartMoving;
        public event Action OnStopped;
        public event Action OnStartStop;
        public event Action<int> OnDirectionChange;
        
        private void Awake()
        {
            var shieldMovement = new RotationMovement(movementData, spriteContainer.transform);
            _movement = shieldMovement;
            ShieldSpeeder = new ShieldSpeeder(_movement,speederData);
            _projectileDetector = new ProjectileDetector(detectorData,shieldMovement);
            
            _movement.OnStartMoving += OnStartMoving;
            _movement.OnStopped += OnStopped;
            _movement.OnStartStop += OnStartStop;
            _movement.OnDirectionChange += OnDirectionChange;
        }
        
        private void Start()
        {
            _projectileDetector.OnTargetFound += Detector_OnTargetFoundHandler;
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _movement.ExecuteMovement(deltaTime);
        }
        
        public void ExecuteFixedUpdate(float fixedDeltaTime)
        {
            if (_canAutoCheck)
            {
                _projectileDetector?.CheckForProjectile();
            }
        }
        
        public void SetAutomaticEnable(bool automaticEnable)
        {
            _canAutoCheck = automaticEnable;
        }
        
        #region Movement

        public bool TryRotate(int direction)
        {
            if (_isPlayerInputDisable) return false;
            
            _movement.SetDirection(direction);
            return true;
        }

        public void ForceStop()
        {
            _movement.ForceStop();
        }

        public void RestartPosition()
        {
            _movement.Restart();
        }

        #endregion

        #region Speeder

        public void IncreaseSpeed()
        {
            ShieldSpeeder.IncreaseSpeed();
        }

        public void DecreaseSpeed()
        {
            ShieldSpeeder.DecreaseSpeed();
        }

        public void RestartSpeedValues()
        {
            ShieldSpeeder.Reset();
        }

        #endregion
        
        #region Coroutine

        public void AutoCorrection()
        {
            StartCoroutine(Coroutine_AutoCorrection());
        }

        public void RotateTowardsNearestProjectileSlot()
        {
            StartCoroutine(Coroutine_RotateTowardsNearestProjectileSlot());
        }

        private IEnumerator Coroutine_AutoCorrection()
        {
            _isPlayerInputDisable = true;
            var currentDirection = _projectileDetector.GetSlotDirection();
            var initialDiff = _projectileDetector.GetSlotDiff();

            while (currentDirection != 0 && _isPlayerInputDisable == true)
            {
                var distanceRatio = (float)_projectileDetector.GetSlotDiff() / initialDiff;
                distanceRatio = 1 -distanceRatio;
                var multiplier = MathfCalculations.Remap(distanceRatio, 0, 1f, 1, 1.75f);
                currentDirection = _projectileDetector.GetSlotDirection() * 10;
                _movement.SetDirection(currentDirection * multiplier);
                
                yield return null;
            }
            
            ForceStop();
            
            _isPlayerInputDisable = false;
        }
        
        private IEnumerator Coroutine_RotateTowardsNearestProjectileSlot()
        {
            var meteorSlot = _projectileDetector.GetNearestProjectileSlot();
            var movement = (IMovement)_movement;
            
            if (meteorSlot > -1)
            {
                while (meteorSlot != movement.GetCurrentSlot())
                {
                    _movement.SetDirection(0.5f);
                
                    yield return null;
                }
            
                _movement.SetDirection(0);
                ForceStop();
            }

            Debug.Log($"Projectile Found at: {meteorSlot}], Shield in Slot: {movement.GetCurrentSlot()}");
            OnProjectileDetected?.Invoke();
        }
        

        #endregion
        
        #region Handlers

        private void Detector_OnTargetFoundHandler()
        {
            AutoCorrection();
        }

        #endregion
        
        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectorData.CheckRadius);
        }

        #endregion
    }
}